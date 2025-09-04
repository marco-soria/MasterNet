using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.CreateCourse;

public class CreateCourseCommand
{
    public record CreateCourseCommandRequest(CreateCourseRequest createCourseRequest)
    : IRequest<Result<Guid>>;


    internal class CreateCourseCommandHandler

    : IRequestHandler<CreateCourseCommandRequest, Result<Guid>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IPhotoService _photoService;

        public CreateCourseCommandHandler(MasterNetDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        public async Task<Result<Guid>> Handle(
            CreateCourseCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var courseRequest = request.createCourseRequest;

            // Validar que datos requeridos estén presentes
            if (string.IsNullOrWhiteSpace(courseRequest.Title))
                throw new ArgumentException("Course title is required");

            if (string.IsNullOrWhiteSpace(courseRequest.Description))
                throw new ArgumentException("Course description is required");

            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Title = courseRequest.Title.Trim(),
                Description = courseRequest.Description.Trim(),
                PublicationDate = courseRequest.PublicationDate
            };

            if (request.createCourseRequest.Photo is not null)
            {
                var photoUploadResult = await _photoService.AddPhoto(request.createCourseRequest.Photo);
                var photo = new Photo
                {
                    Id = Guid.NewGuid(),
                    Url = photoUploadResult.Url,
                    PublicId = photoUploadResult.PublicId,
                    CourseId = course.Id,
                };
                
                course.Photos = new List<Photo> { photo };
            }

            _context.Add(course);

            // Asociar instructores si se proporcionaron
            if (courseRequest.InstructorIds?.Any() == true)
            {
                var instructors = await _context.Instructors!
                    .Where(i => courseRequest.InstructorIds.Contains(i.Id))
                    .ToListAsync(cancellationToken);

                course.Instructors = instructors;
            }

            // Asociar precios si se proporcionaron
            if (courseRequest.PriceIds?.Any() == true)
            {
                var prices = await _context.Prices!
                    .Where(p => courseRequest.PriceIds.Contains(p.Id))
                    .ToListAsync(cancellationToken);

                course.Prices = prices;
            }

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Guid>.Success(course.Id)
                : Result<Guid>.Failure("Failed to create course");
        }
    }

    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommandRequest>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.createCourseRequest).SetValidator(new CreateCourseValidator());
        }

    }
}
