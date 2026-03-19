using AutoMapper;
using BookLending.Application.DTOs.Book;
using BookLending.Domain.Models;

namespace BookLending.Application.AutoMapperProfile
{
    public class BookLending_Profile : Profile
    {
        public BookLending_Profile()
        {
            CreateMap<CreateBookDto, Book>();

            CreateMap<UpdateBookDto, Book>()
                  .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
                 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


        }
    }
}
