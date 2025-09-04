using MasterNet.Application.Instructors.GetInstructors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Instructors.GetInstructors.GetInstructorsQuery;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/instructors")]
public class InstructorsController : ControllerBase
{
    private readonly ISender _sender;

    public InstructorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult> InstructorPagination
    (
        [FromQuery] GetInstructorsRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new GetInstructorsQueryRequest {
            InstructorRequest = request
        };
        var results =  await _sender.Send(query, cancellationToken);
        return results.IsSuccess ? Ok(results.Value) : NotFound();
    }

}