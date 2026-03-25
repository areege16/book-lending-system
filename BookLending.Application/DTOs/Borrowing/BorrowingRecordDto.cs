namespace BookLending.Application.DTOs.Borrowing
{
    public class BorrowingRecordDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public DateTimeOffset BorrowDate { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public DateTimeOffset? ReturnDate { get; set; }
        public bool IsOverdue { get; set; }
    }
}