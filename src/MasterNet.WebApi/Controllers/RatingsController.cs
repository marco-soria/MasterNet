using MasterNet.Application.Ratings.GetRatings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Ratings.GetRatings.GetRatingsQuery;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/ratings")]
public class RatingsController : ControllerBase
{
    private readonly ISender _sender;

    public RatingsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult> RatingPagination
    (
        [FromQuery] GetRatingsRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new GetRatingsQueryRequest {
            RatingsRequest = request
        };
        var results =  await _sender.Send(query, cancellationToken);
        return results.IsSuccess ? Ok(results.Value) : NotFound();
    }

}