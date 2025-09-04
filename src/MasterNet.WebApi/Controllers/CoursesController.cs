using MasterNet.Application.Core;
using MasterNet.Application.Courses.CreateCourse;
using MasterNet.Application.Courses.GetCourse;
using MasterNet.Application.Courses.GetCourses;
using MasterNet.Application.Courses.UpdateCourse;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Courses.CourseExcelReport.CourseExcelReportQuery;
using static MasterNet.Application.Courses.CreateCourse.CreateCourseCommand;
using static MasterNet.Application.Courses.DeleteCourse.DeleteCourseCommand;
using static MasterNet.Application.Courses.GetCourse.GetCourseQuery;
using static MasterNet.Application.Courses.GetCurses.GetCoursesQuery;
using static MasterNet.Application.Courses.UpdateCourse.UpdateCourseCommand;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<ActionResult<PagedList<CourseDTO>>> CoursesPagination(
        [FromQuery] GetCoursesRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = new GetCoursesQueryRequest { CoursesRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        // ✅ Para colecciones vacías, devolver 200 OK con array vacío (no 404)
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDTO>> GetCourse(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var query = new GetCourseQueryRequest { Id = id };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("report")]
    public async Task<IActionResult> ReportCSV(CancellationToken cancellationToken)
    {
        var query = new CourseExcelReportQueryRequest();
        var result = await _sender.Send(query, cancellationToken);
        byte[] excelBytes = result.ToArray();
        return File(excelBytes, "text/csv", "courses.csv");
    }
    [HttpPost]
    public async Task<ActionResult<Result<Guid>>> CreateCourse(
        [FromForm] CreateCourseRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateCourseCommandRequest(request);
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            // ✅ 201 Created con Location header apuntando al nuevo recurso
            return CreatedAtAction(
                nameof(GetCourse), 
                new { id = result.Value }, 
                result.Value
            );
        }
        
        return BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<Guid>>> UpdateCourse(
        [FromForm] UpdateCourseRequest request,  // ✅ FromForm para manejar archivos
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateCourseCommandRequest(request, id);
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value); // ✅ 200 OK para actualizaciones exitosas
        }
        
        // ✅ Diferenciar entre "no encontrado" y "bad request"
        if (result.Error?.Contains("does not exist") == true)
        {
            return NotFound(result.Error);
        }
        
        return BadRequest(result.Error);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCourse(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteCourseCommandRequest(id);
        var result = await _sender.Send(command, cancellationToken);
        
        if (result.IsSuccess)
        {
            return NoContent(); // ✅ 204 No Content para eliminaciones exitosas
        }
        
        // ✅ Diferenciar entre "no encontrado" y "bad request"
        if (result.Error?.Contains("does not exist") == true)
        {
            return NotFound(result.Error);
        }
        
        return BadRequest(result.Error);
    }


}