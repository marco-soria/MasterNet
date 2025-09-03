using MasterNet.Application.Interfaces;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.CourseExcelReport;


public class CourseExcelReportQuery
{

    public record CourseExcelReportQueryRequest 
        : IRequest<MemoryStream>;

    internal class CourseExcelReportQueryHandler
    : IRequestHandler<CourseExcelReportQueryRequest, MemoryStream>
    {
        private readonly MasterNetDbContext _context;
        private readonly IReportService<Course> _reportService;

        public CourseExcelReportQueryHandler(
            MasterNetDbContext context, 
            IReportService<Course> reportService
        )
        {
            _context = context;
            _reportService = reportService;
        }

        public async Task<MemoryStream> Handle(
            CourseExcelReportQueryRequest request, 
            CancellationToken cancellationToken
        )
        {
            var courses = await _context.Courses!
                .OrderBy(c => c.PublicationDate ?? DateTime.MaxValue)  // Nulls al final
                .ThenBy(c => c.Title)                                  // Orden alfabético como desempate
                .ThenBy(c => c.Id)                                     // Garantiza orden único
                .Take(10)
                .Skip(0)
                .ToListAsync(cancellationToken);

            return await _reportService.GetCsvReport(courses);
        }
    }
}