using Microsoft.AspNetCore.Identity;

namespace BookLending.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public ICollection<BorrowingRecord>? BorrowingRecords { get; set; }
    }
}