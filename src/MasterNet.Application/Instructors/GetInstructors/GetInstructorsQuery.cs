using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Core;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;

namespace MasterNet.Application.Instructors.GetInstructors;

public class GetInstructorsQuery
{

    public record GetInstructorsQueryRequest
    : IRequest<Result<PagedList<InstructorDTO>>>
    {
        public GetInstructorsRequest? InstructorRequest {get;set;}

    }


    internal class GetInstructorsQueryHandler(MasterNetDbContext context, IMapper mapper)
        : IRequestHandler<GetInstructorsQueryRequest, Result<PagedList<InstructorDTO>>>
    {
        private readonly MasterNetDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<PagedList<InstructorDTO>>> Handle(
            GetInstructorsQueryRequest request, 
            CancellationToken cancellationToken
        )
        {
           
            IQueryable<Instructor> queryable = _context.Instructors!;

            var predicate = ExpressionBuilder.New<Instructor>();
            if(!string.IsNullOrEmpty(request.InstructorRequest!.FirstName))
            {
                predicate = predicate
                .And(y => y.FirstName!.Contains(request.InstructorRequest!.FirstName));
            }

            if(!string.IsNullOrEmpty(request.InstructorRequest!.LastName))
            {
                predicate = predicate
                .And(y => y.LastName!.Contains(request.InstructorRequest!.LastName));
            }

            queryable = queryable.Where(predicate);

            // Aplicar ordenamiento ANTES de la paginación para evitar warning Skip/Take
            if(!string.IsNullOrEmpty(request.InstructorRequest.OrderBy))
            {
                Expression<Func<Instructor, object>>? orderBySelector = 
                request.InstructorRequest.OrderBy.ToLower() switch 
                {
                    "firstName" => instructor => instructor.FirstName!,
                    "lastName" => instructor => instructor.LastName!,
                    "degree" => instructor => instructor.Degree!,
                    _ => instructor => instructor.FirstName!
                };

                bool orderAsc = request.InstructorRequest.OrderAsc.HasValue 
                            ? request.InstructorRequest.OrderAsc.Value
                            : true;

                queryable = orderAsc 
                            ? queryable.OrderBy(orderBySelector)
                            : queryable.OrderByDescending(orderBySelector);
            }
            else
            {
                // OrderBy por defecto para evitar warning Skip/Take sin OrderBy
                queryable = queryable.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
            }

            var instructorsQuery = queryable
                        .ProjectTo<InstructorDTO>(_mapper.ConfigurationProvider)
                        .AsQueryable();

            var pagination = await PagedList<InstructorDTO>
                .CreateAsync(instructorsQuery, 
                request.InstructorRequest.PageNumber,
                request.InstructorRequest.PageSize
                );

            return Result<PagedList<InstructorDTO>>.Success(pagination);
        }
    }
}


public record InstructorDTO(
    Guid? Id,
    string? FirstName,
    string? LastName,
    string? Degree
)

{
    public InstructorDTO() : this(null, null, null, null)
    {
    }
}