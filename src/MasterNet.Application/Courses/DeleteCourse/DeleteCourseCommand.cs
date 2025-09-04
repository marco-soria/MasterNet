using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.DeleteCourse;

/// <summary>
/// Comando para eliminación física de un curso y todas sus dependencias.
/// Incluye eliminación de fotos en Cloudinary y manejo de relaciones many-to-many.
/// </summary>
public class DeleteCourseCommand
{
    public record DeleteCourseCommandRequest(Guid? CourseId)
        : IRequest<Result<Unit>>;

    internal class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommandRequest, Result<Unit>>
    {
        private readonly MasterNetDbContext _context;
        private readonly IPhotoService _photoService;

        public DeleteCourseCommandHandler(MasterNetDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        public async Task<Result<Unit>> Handle(
            DeleteCourseCommandRequest request, 
            CancellationToken cancellationToken
        )
        {
            // ✅ CARGAR SOLO EL CURSO PRINCIPAL (sin Include para evitar warnings de múltiples colecciones)
            var course = await _context.Courses!
                .FirstOrDefaultAsync(x => x.Id == request.CourseId, cancellationToken);

            if (course is null)
            {
                return Result<Unit>.Failure("The course does not exist");
            }

            // ✅ ELIMINAR FOTOS DE CLOUDINARY PRIMERO
            var photos = await _context.Set<Photo>()
                .Where(p => p.CourseId == request.CourseId)
                .ToListAsync(cancellationToken);

            foreach (var photo in photos)
            {
                if (!string.IsNullOrEmpty(photo.PublicId))
                {
                    await _photoService.DeletePhoto(photo.PublicId);
                }
            }

            // ✅ ELIMINAR RELACIONES MANY-TO-MANY MANUALMENTE (evita warnings y es más explícito)
            var courseInstructors = await _context.Set<CourseInstructor>()
                .Where(ci => ci.CourseId == request.CourseId)
                .ToListAsync(cancellationToken);

            if (courseInstructors.Any())
            {
                _context.Set<CourseInstructor>().RemoveRange(courseInstructors);
            }

            var coursePrices = await _context.Set<CoursePrice>()
                .Where(cp => cp.CourseId == request.CourseId)
                .ToListAsync(cancellationToken);

            if (coursePrices.Any())
            {
                _context.Set<CoursePrice>().RemoveRange(coursePrices);
            }

            // ✅ LAS RELACIONES ONE-TO-MANY SE MANEJAN AUTOMÁTICAMENTE POR CASCADE DELETE
            // - Photos: DeleteBehavior.Cascade configurado en el DbContext
            // - Ratings: DeleteBehavior.Restrict configurado (si hay ratings, fallará con error claro)

            // ✅ ELIMINAR EL CURSO (esto eliminará automáticamente Photos por cascade)
            _context.Courses!.Remove(course);

            try
            {
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                return result 
                    ? Result<Unit>.Success(Unit.Value) 
                    : Result<Unit>.Failure("Error deleting the course");
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FOREIGN KEY constraint failed") == true)
            {
                // ✅ MANEJAR ERROR DE CONSTRAINT (ej: si hay ratings que impiden la eliminación)
                return Result<Unit>.Failure("Cannot delete course. It has related ratings that must be removed first");
            }
            catch (Exception)
            {
                return Result<Unit>.Failure("Error deleting the course");
            }
        }
    }


    public class DeleteCourseCommandRequestValidator
    : AbstractValidator<DeleteCourseCommandRequest>
    {
        public DeleteCourseCommandRequestValidator()
        {
            RuleFor(x => x.CourseId).NotNull().WithMessage("There is no course id");
        }
    }


}