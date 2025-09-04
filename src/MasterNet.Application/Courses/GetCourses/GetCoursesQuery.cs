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

    internal class GetCoursesQueryHandler(MasterNetDbContext context, IMapper mapper)
        : IRequestHandler<GetCoursesQueryRequest, Result<PagedList<CourseDTO>>>
    {
        private readonly MasterNetDbContext _context = context;
        private readonly IMapper _mapper = mapper;

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

            queryable = queryable.Where(predicate);

            // Aplicar ordenamiento ANTES de la paginación para evitar warning Skip/Take
            if (!string.IsNullOrEmpty(request.CoursesRequest!.OrderBy))
            {
                Expression<Func<Course, object>>? orderBySelector =
                                request.CoursesRequest.OrderBy!.ToLower() switch
                                {
                                    "title" => course => course.Title!,
                                    "description" => course => course.Description!,
                                    "publicationDate" => course => course.PublicationDate!,
                                    _ => course => course.Title!
                                };

                bool orderAsc = request.CoursesRequest.OrderAsc.HasValue
                            ? request.CoursesRequest.OrderAsc.Value
                            : true;

                queryable = orderAsc
                            ? queryable.OrderBy(orderBySelector)
                            : queryable.OrderByDescending(orderBySelector);
            }
            else
            {
                // OrderBy por defecto para evitar warning Skip/Take sin OrderBy
                // Orden alfabético por título, luego por fecha de publicación (ASC = orden cronológico)
                queryable = queryable.OrderBy(x => x.Title).ThenBy(x => x.PublicationDate);
            }

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