using AutoMapper;
using LibraryAPI.Entites;
using LibraryAPI.Helper;
using LibraryAPI.Models;

namespace LibraryAPI.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(des => des.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(des => des.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));

            CreateMap<AuthorForCreationDto, Author>();
        }

    }
}
 