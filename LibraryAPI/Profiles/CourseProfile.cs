using AutoMapper;
using LibraryAPI.Entites;
using LibraryAPI.Models;

namespace LibraryAPI.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CoursesDto>();
            CreateMap<CoursesForCreationDto, Course>();
            CreateMap<CourseForUpdateDto, Course>();
            CreateMap<CourseForUpdateDto, Course>().ReverseMap();
        }
    }
}
