using MasterNet.Application.Core;
using MasterNet.Application.Instructors.GetInstructors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Instructors.GetInstructors.GetInstructorsQuery;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/instructors")]
public class InstructorsController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<PagedList<InstructorDTO>>> InstructorPagination
    (
        [FromQuery] GetInstructorsRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new GetInstructorsQueryRequest {
            InstructorRequest = request
        };
        var results = await _sender.Send(query, cancellationToken);
        
        // ✅ Para colecciones vacías, devolver 200 OK con array vacío (no 404)
        return results.IsSuccess ? Ok(results.Value) : BadRequest(results.Error);
    }

}