
using AutoMapper;
using MasterNet.Application.Courses.GetCourse;
using MasterNet.Application.Instructors.GetInstructors;
using MasterNet.Application.Photos.GetPhoto;
using MasterNet.Application.Prices.GetPrices;
using MasterNet.Application.Ratings.GetRatings;
using MasterNet.Domain;

namespace MasterNet.Application.Core;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseDTO>();
        CreateMap<Photo, PhotoDTO>();
        CreateMap<Price, PriceDTO>();

        CreateMap<Instructor, InstructorDTO>();
        
        CreateMap<Rating, RatingDTO>()
            .ForMember(dest=> dest.CourseName, src=>src.MapFrom(doc => doc.Course!.Title));
    }
}