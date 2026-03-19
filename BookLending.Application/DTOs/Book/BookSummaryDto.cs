namespace BookLending.Application.DTOs.Book
{
    public record BookSummaryDto
    (
      int Id,
      string Title,
      string Author,
      string? CoverImage,
      bool IsAvailable
    );
}