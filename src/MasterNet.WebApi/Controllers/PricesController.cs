using MasterNet.Application.Prices.GetPrices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Prices.GetPrices.GetPricesQuery;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/prices")]
public class PricesController : ControllerBase
{
    private readonly ISender _sender;

    public PricesController(ISender sender)
    {
        _sender = sender;
    }

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
        var results =  await _sender.Send(query, cancellationToken);
        return results.IsSuccess ? Ok(results.Value) : NotFound();
    }

}