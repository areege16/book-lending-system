namespace BookLending.Application.DTOs.Borrowing
{
    public class AdminBorrowingRecordDto : BorrowingRecordDto
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}