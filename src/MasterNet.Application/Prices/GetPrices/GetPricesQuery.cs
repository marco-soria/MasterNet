using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterNet.Application.Core;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;

namespace MasterNet.Application.Prices.GetPrices;

public class GetPricesQuery
{

    public record GetPricesQueryRequest 
    : IRequest<Result<PagedList<PriceDTO>>>
    {
        public GetPricesRequest? PricesRequest {get;set;} 
    }

    internal class GetPricesQueryHandler :
    IRequestHandler<GetPricesQueryRequest, Result<PagedList<PriceDTO>>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IMapper _mapper;

        public GetPricesQueryHandler(MasterNetDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<PriceDTO>>> Handle(
            GetPricesQueryRequest request, 
            CancellationToken cancellationToken
        )
        {
        
            IQueryable<Price> queryable = _context.Prices!;

            var predicate = ExpressionBuilder.New<Price>();

            if(!string.IsNullOrEmpty(request.PricesRequest!.Name))
            {   
                predicate  = predicate
                .And(y => y.Name!.Contains(request.PricesRequest!.Name));
            }

            queryable = queryable.Where(predicate);

            // Aplicar ordenamiento ANTES de la paginación para evitar warning Skip/Take
            if(!string.IsNullOrEmpty(request.PricesRequest!.OrderBy))
            {
                Expression<Func<Price, object>>? orderSelector = 
                    request.PricesRequest.OrderBy.ToLower() switch
                    {
                        "name" => x => x.Name!,
                        "currentPrice" => x => x.CurrentPrice,
                        "promotionalPrice" => x => x.PromotionalPrice!,
                        _ =>x => x.Name!
                    };

                    bool orderAsc = request.PricesRequest.OrderAsc.HasValue
                        ? request.PricesRequest.OrderAsc.Value
                        : true;
                    
                    queryable = orderAsc
                                ? queryable.OrderBy(orderSelector)
                                : queryable.OrderByDescending(orderSelector);
            }
            else
            {
                // OrderBy por defecto para evitar warning Skip/Take sin OrderBy
                queryable = queryable.OrderBy(x => x.Name);
            }

            var pricesQuery = queryable
                    .ProjectTo<PriceDTO>(_mapper.ConfigurationProvider)
                    .AsQueryable();
           

           var pagination = await PagedList<PriceDTO>
            .CreateAsync(pricesQuery, 
                request.PricesRequest.PageNumber, 
                request.PricesRequest.PageSize
           );

           return Result<PagedList<PriceDTO>>.Success(pagination);
        }
    }
}

public record PriceDTO(
    Guid? Id,
    string? Name,
    decimal? CurrentPrice,
    decimal? PromotionalPrice
)
{
    public PriceDTO() : this(null, null, null, null)
    {
    }
}