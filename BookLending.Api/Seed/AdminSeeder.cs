using BookLending.Application.Setting;
using BookLending.Domain.Constants;
using BookLending.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BookLending.Api.Seed
{
    public class AdminSeeder
    {
        private readonly AdminSeedSettings _adminSeedSettings;
        private readonly ILogger<AdminSeeder> _logger;

        public AdminSeeder(IOptions<AdminSeedSettings> adminSeedSettings, ILogger<AdminSeeder> logger)
        {
            _adminSeedSettings = adminSeedSettings.Value;
            _logger = logger;
        }
        public async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (await userManager.FindByEmailAsync(_adminSeedSettings.Email) != null)
                return;

            var admin = new ApplicationUser
            {
                UserName = _adminSeedSettings.UserName,
                Email = _adminSeedSettings.Email,
                FullName = _adminSeedSettings.FullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, _adminSeedSettings.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
                _logger.LogInformation("Admin user {Email} seeded successfully.", _adminSeedSettings.Email);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to seed admin user. Errors: {Errors}", errors);
            }
        }
    }
}