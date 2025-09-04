using MasterNet.Application.Core;
using MasterNet.Application.Courses.CreateCourse;
using MasterNet.Application.Courses.GetCourses;
using MasterNet.Application.Courses.UpdateCourse;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Courses.CourseExcelReport.CourseExcelReportQuery;
using static MasterNet.Application.Courses.CreateCourse.CreateCourseCommand;
using static MasterNet.Application.Courses.GetCourse.GetCourseQuery;
using static MasterNet.Application.Courses.GetCurses.GetCoursesQuery;
using static MasterNet.Application.Courses.UpdateCourse.UpdateCourseCommand;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ISender _sender;
    public CoursesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult> CoursesPagination(
        [FromQuery] GetCoursesRequest request,
        CancellationToken cancellationToken
    )
    {

        var query = new GetCoursesQueryRequest { CoursesRequest = request };
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(
        Guid id, 
        CancellationToken cancellationToken
    )
    {
        var query = new GetCourseQueryRequest { Id = id};
         var result = await _sender.Send(query, cancellationToken);

         return result.IsSuccess ? Ok(result.Value) : BadRequest();
    }
    
    [HttpGet("report")]
    public async Task<IActionResult> ReportCSV(CancellationToken cancellationToken)
    {
        var query = new CourseExcelReportQueryRequest();
        var result =  await _sender.Send(query, cancellationToken);
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
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
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
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }


}