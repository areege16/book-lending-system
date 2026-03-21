
using BookLending.Api.Middlewares;
using BookLending.Api.Seed;
using BookLending.Application.Abstractions;
using BookLending.Application.Account.Register;
using BookLending.Application.AutoMapperProfile;
using BookLending.Application.Common.Behaviors;
using BookLending.Application.RepositoryContract;
using BookLending.Application.Setting;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Models;
using BookLending.Infrastructure.Context;
using BookLending.Infrastructure.RepositoryImplementation;
using BookLending.Infrastructure.Services;
using BookLending.Infrastructure.UnitOfWorkImplementation;
using CloudinaryDotNet;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace BookLending.Api
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            builder.Services.AddControllers();

            builder.Services.AddDbContext<ApplicationContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                if (builder.Environment.IsDevelopment())
                {
                    option.LogTo(
                        message => Console.WriteLine($"\n[EF] {message}\n"),
                        new[] { DbLoggerCategory.Database.Command.Name },
                        LogLevel.Information);
                }
            });

            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Book Lending System API",
                    Description = "A RESTful API for managing book catalog"
                });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token in the text input below\nExample: \"Bearer eyJhbGciOi...\""
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

          .AddJwtBearer(options =>
          {
              options.SaveToken = true;
              options.RequireHttpsMetadata = false;
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidIssuer = builder.Configuration["JWT:Issuer"],
                  ValidateAudience = true,
                  ValidAudience = builder.Configuration["JWT:Audience"],
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                  RoleClaimType = ClaimTypes.Role,
              };
          });

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<AdminSeeder>();
            builder.Services.AddScoped<IOverdueCheckService, OverdueBookJob>();
            builder.Services.AddScoped<IEmailService, MailKitEmailService>();

            builder.Services.AddSingleton<Cloudinary>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<CloudinarySetting>>().Value;

                var account = new Account(settings.Cloud, settings.ApiKey, settings.ApiSecret);
                return new Cloudinary(account);
            });

            builder.Services.Configure<AdminSeedSettings>(builder.Configuration.GetSection("AdminSeed"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("Cloudinary"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(RegisterHandler).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            builder.Services.AddAutoMapper(typeof(BookLending_Profile).Assembly);

            builder.Services.AddFluentValidation();
            builder.Services.AddValidatorsFromAssembly(BookLending.Application.AssemblyReference.Assembly, includeInternalTypes: true);
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHangfireServer();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                await RoleSeeder.SeedRoles(scopedServices);
                var adminSeeder = scopedServices.GetRequiredService<AdminSeeder>();
                await adminSeeder.SeedAdminAsync(scopedServices);
            }


                app.UseSwagger();
                app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseMiddleware<ExceptionHandlingMiddleware>();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire");

            RecurringJob.AddOrUpdate<IOverdueCheckService>(
                 "overdue-books-check",
                 job => job.CheckAndNotifyOverdueBooks(),
                 Cron.Daily);

            app.MapControllers();

            app.Run();
        }
    }
}
