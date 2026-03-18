using System.ComponentModel.DataAnnotations.Schema;

namespace BookLending.Domain.Models
{
    public class BorrowingRecord
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTimeOffset BorrowDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset DueDate { get; set; }
        public DateTimeOffset? ReturnDate { get; set; }
    }
}
