using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Core;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;

namespace MasterNet.Application.Ratings.GetRatings;

public class GetRatingsQuery
{

    public record GetRatingsQueryRequest 
    :IRequest<Result<PagedList<RatingDTO>>>
    {
        public GetRatingsRequest? RatingsRequest {get;set;}
    }

    internal class GetRatingsQueryHandler
    : IRequestHandler<GetRatingsQueryRequest, Result<PagedList<RatingDTO>>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetRatingsQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<RatingDTO>>> Handle(GetRatingsQueryRequest request, CancellationToken cancellationToken)
        {
            
            IQueryable<Rating> queryable = _context.Ratings!;
            

            var predicate = ExpressionBuilder.New<Rating>();
            if(!string.IsNullOrEmpty(request.RatingsRequest!.Student))
            {
                predicate = predicate
                .And(y => y.Student!.Contains(request.RatingsRequest.Student));
            }

            if(request.RatingsRequest.CourseId is not null)
            {
                predicate = predicate
                .And(y => y.CourseId== request.RatingsRequest.CourseId);
            }

            queryable = queryable.Where(predicate);

            // Aplicar ordenamiento ANTES de la paginación para evitar warning Skip/Take
            if(!string.IsNullOrEmpty(request.RatingsRequest.OrderBy))
            {
                Expression<Func<Rating, object>>? orderBySelector =
                    request.RatingsRequest.OrderBy.ToLower() switch
                    {
                        "student" => x => x.Student!,
                        "courseId" => x => x.CourseId!,
                        "score" => x => x.Score!,
                        _ => x => x.Student!
                    };

                    bool orderAsc = request.RatingsRequest.OrderAsc.HasValue
                                    ? request.RatingsRequest.OrderAsc.Value
                                    : true;

                    queryable = orderAsc 
                                ? queryable.OrderBy(orderBySelector)
                                : queryable.OrderByDescending(orderBySelector);
            }
            else
            {
                // OrderBy por defecto para evitar warning Skip/Take sin OrderBy
                queryable = queryable.OrderBy(x => x.Student).ThenByDescending(x => x.Score);
            }

            var RatingQuery = queryable
                                    .ProjectTo<RatingDTO>(_mapper.ConfigurationProvider)
                                    .AsQueryable();

            var pagination = await PagedList<RatingDTO>
                    .CreateAsync(
                        RatingQuery,
                        request.RatingsRequest.PageNumber,
                        request.RatingsRequest.PageSize
                    );


            return Result<PagedList<RatingDTO>>.Success(pagination);
        }
    }

}


public record RatingDTO(
    string? Student,
    int? Score,
    string? Comment,
    string? CourseName
)
{
    public RatingDTO() : this(null, null, null, null)
    {
    }
}