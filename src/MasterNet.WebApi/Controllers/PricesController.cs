using MasterNet.Application.Prices.GetPrices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Prices.GetPrices.GetPricesQuery;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/prices")]
public class PricesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult> PricePagination
    (
        [FromQuery] GetPricesRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new GetPricesQueryRequest {
            PricesRequest = request
        };
        var results = await _sender.Send(query, cancellationToken);
        
        // ✅ Para colecciones vacías, devolver 200 OK con array vacío (no 404)
        return results.IsSuccess ? Ok(results.Value) : BadRequest(results.Error);
    }

}