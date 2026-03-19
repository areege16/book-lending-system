using BookLending.Application.Abstractions;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLending.Infrastructure.Services
{
    public class OverdueBookJob : IOverdueCheckService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OverdueBookJob> _logger;
        private readonly IEmailService _emailService;

        public OverdueBookJob(IUnitOfWork unitOfWork,
                              ILogger<OverdueBookJob> logger,
                              IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task CheckAndNotifyOverdueBooks()
        {
            var now = DateTimeOffset.UtcNow;

            var overdueRecords = await _unitOfWork.Repository<BorrowingRecord>()
                .GetAllAsNoTracking()
                .Include(br => br.User)
                .Include(br => br.Book)
                .Where(br => br.ReturnDate == null && br.DueDate < now)
                .ToListAsync();

            if (!overdueRecords.Any())
            {
                _logger.LogInformation("No overdue books found.");
                return;
            }

            foreach (var record in overdueRecords)
            {

                await _emailService.SendOverdueReminderAsync(
                record.User.Email,
                record.User.UserName,
                record.Book.Title,
                record.DueDate);

                _logger.LogWarning(
                    "Overdue book: '{BookTitle}' borrowed by {UserName}. Due: {DueDate}",
                    record.Book.Title,
                    record.User.UserName,
                    record.DueDate);
            }

            _logger.LogInformation("Overdue check completed. Found {Count} overdue records.", overdueRecords.Count);
        }
    }
}