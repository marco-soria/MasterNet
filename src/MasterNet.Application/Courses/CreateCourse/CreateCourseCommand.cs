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
    public record CreateCourseCommandRequest(CreateCourseRequest CreateCourseRequest)
    : IRequest<Result<Guid>>, IBaseCommand;


    internal class CreateCourseCommandHandler(MasterNetDbContext context, IPhotoService photoService)


    : IRequestHandler<CreateCourseCommandRequest, Result<Guid>>
    {
        private readonly MasterNetDbContext _context = context;
        private readonly IPhotoService _photoService = photoService;

        public async Task<Result<Guid>> Handle(
            CreateCourseCommandRequest request,
            CancellationToken cancellationToken
        )
        {
            var courseRequest = request.CreateCourseRequest;

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

            if (request.CreateCourseRequest.Photo is not null)
            {
                var photoUploadResult = await _photoService.AddPhoto(request.CreateCourseRequest.Photo);
                var photo = new Photo
                {
                    Id = Guid.NewGuid(),
                    Url = photoUploadResult.Url,
                    PublicId = photoUploadResult.PublicId,
                    CourseId = course.Id,
                };
                
                course.Photos = [photo];
            }

            _context.Add(course);

            // Associate instructors if provided and validate they exist
            if (courseRequest.InstructorIds?.Count > 0)
            {
                var instructors = await _context.Instructors!
                    .Where(i => courseRequest.InstructorIds.Contains(i.Id))
                    .ToListAsync(cancellationToken);

                // Validate that all requested instructors were found
                if (instructors.Count != courseRequest.InstructorIds.Count)
                {
                    var foundIds = instructors.Select(i => i.Id).ToList();
                    var missingIds = courseRequest.InstructorIds.Except(foundIds).ToList();
                    return Result<Guid>.Failure($"Instructors not found with IDs: {string.Join(", ", missingIds)}");
                }

                course.Instructors = instructors;
            }

            // Associate prices if provided and validate they exist
            if (courseRequest.PriceIds?.Count > 0)
            {
                var prices = await _context.Prices!
                    .Where(p => courseRequest.PriceIds.Contains(p.Id))
                    .ToListAsync(cancellationToken);

                // Validate that all requested prices were found
                if (prices.Count != courseRequest.PriceIds.Count)
                {
                    var foundIds = prices.Select(p => p.Id).ToList();
                    var missingIds = courseRequest.PriceIds.Except(foundIds).ToList();
                    return Result<Guid>.Failure($"Prices not found with IDs: {string.Join(", ", missingIds)}");
                }

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
            RuleFor(x => x.CreateCourseRequest).SetValidator(new CreateCourseValidator());
        }

    }
}
