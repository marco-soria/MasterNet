using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Ratings.GetRatings;
using MasterNet.Application.Core;
using MasterNet.Application.Instructors.GetInstructors;
using MasterNet.Application.Photos.GetPhoto;
using MasterNet.Application.Prices.GetPrices;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.GetCourse;

public class GetCourseQuery
{

    public record GetCourseQueryRequest 
    : IRequest<Result<CourseDTO>>
    {
        public Guid Id {get;set;}
    }

    internal class GetCourseQueryHandler
    : IRequestHandler<GetCourseQueryRequest, Result<CourseDTO>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetCourseQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<CourseDTO>> Handle(
            GetCourseQueryRequest request, 
            CancellationToken cancellationToken
        )
        {
            var course = await _context.Courses!
                .Where(x => x.Id == request.Id)
                .ProjectTo<CourseDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (course == null)
            {
                return Result<CourseDTO>.Failure("Course not found");
            }

            return Result<CourseDTO>.Success(course);
        }
    }



}

public record CourseDTO(
    Guid Id,
    string Title,
    string Description,
    DateTime? PublicationDate,
    List<InstructorDTO> Instructors,
    List<RatingDTO> Ratings,
    List<PriceDTO> Prices,
    List<PhotoDTO> Photos
);