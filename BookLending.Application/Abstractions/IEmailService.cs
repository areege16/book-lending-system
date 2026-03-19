namespace BookLending.Application.Abstractions
{
    public interface IEmailService
    {
        Task SendOverdueReminderAsync(string toEmail, string toName, string bookTitle, DateTimeOffset dueDate);
    }
}