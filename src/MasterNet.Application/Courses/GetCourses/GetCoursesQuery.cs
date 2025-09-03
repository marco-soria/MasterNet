using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Core;
using MasterNet.Application.Courses.GetCourse;
using MasterNet.Application.Courses.GetCourses;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.GetCurses;

public class GetCoursesQuery
{

    public record GetCoursesQueryRequest : IRequest<Result<PagedList<CourseDTO>>>
    {
        public GetCoursesRequest? CoursesRequest { get; set; }
    }

    internal class GetCoursesQueryHandler
    : IRequestHandler<GetCoursesQueryRequest, Result<PagedList<CourseDTO>>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetCoursesQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<CourseDTO>>> Handle(
            GetCoursesQueryRequest request,
            CancellationToken cancellationToken
        )
        {

            IQueryable<Course> queryable = _context.Courses!;

            var predicate = ExpressionBuilder.New<Course>();
            if (!string.IsNullOrEmpty(request.CoursesRequest!.Title))
            {
                predicate = predicate
                .And(y => y.Title!.ToLower()
                .Contains(request.CoursesRequest.Title.ToLower()));
            }


            if (!string.IsNullOrEmpty(request.CoursesRequest!.Description))
            {
                predicate = predicate
                .And(y => y.Description!.ToLower()
                .Contains(request.CoursesRequest.Description.ToLower()));
            }

            if (!string.IsNullOrEmpty(request.CoursesRequest!.OrderBy))
            {
                Expression<Func<Course, object>>? orderBySelector =
                                request.CoursesRequest.OrderBy!.ToLower() switch
                                {
                                    "title" => course => course.Title!,
                                    "description" => course => course.Description!,
                                    _ => course => course.Title!
                                };

                bool orderBy = request.CoursesRequest.OrderAsc.HasValue
                            ? request.CoursesRequest.OrderAsc.Value
                            : true;

                queryable = orderBy
                            ? queryable.OrderBy(orderBySelector)
                            : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var coursesQuery = queryable
            .AsSplitQuery()
            .ProjectTo<CourseDTO>(_mapper.ConfigurationProvider)
            .AsQueryable();

            var pagination = await PagedList<CourseDTO>.CreateAsync(
                coursesQuery,
                request.CoursesRequest.PageNumber,
                request.CoursesRequest.PageSize
            );

            return Result<PagedList<CourseDTO>>.Success(pagination);

        }
    }
}