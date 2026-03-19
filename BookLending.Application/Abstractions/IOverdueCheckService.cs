namespace BookLending.Application.Abstractions
{
    public interface IOverdueCheckService
    {
        Task CheckAndNotifyOverdueBooks();
    }
}