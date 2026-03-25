using AutoMapper;
using BookLending.Application.DTOs.Book;
using BookLending.Application.DTOs.Borrowing;
using BookLending.Domain.Models;

namespace BookLending.Application.AutoMapperProfile
{
    public class BookLending_Profile : Profile
    {
        public BookLending_Profile()
        {
            #region Book
            CreateMap<CreateBookDto, Book>();

            CreateMap<UpdateBookDto, Book>()
                 .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
                 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Book, BookDetailsDto>();

            CreateMap<Book, BookSummaryDto>();
            #endregion

            #region Borrowing
            CreateMap<BorrowingRecord, BorrowingRecordDto>()
                 .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.Book.Id))
                 .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                 .ForMember(dest => dest.BookAuthor, opt => opt.MapFrom(src => src.Book.Author))
                 .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.ReturnDate == null && src.DueDate < DateTimeOffset.UtcNow));


            CreateMap<BorrowingRecord, AdminBorrowingRecordDto>()
              .IncludeBase<BorrowingRecord, BorrowingRecordDto>()
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
              .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
            #endregion
        }
    }
}
