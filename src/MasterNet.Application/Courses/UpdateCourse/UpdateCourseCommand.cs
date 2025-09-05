using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Domain;
using MasterNet.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Courses.UpdateCourse;

public class UpdateCourseCommand
{

    public record UpdateCourseCommandRequest(
        UpdateCourseRequest UpdateCourseRequest, 
        Guid? CourseId
    ): IRequest<Result<Guid>>, IBaseCommand;

    internal class UpdateCourseCommandHandler(MasterNetDbContext context, IPhotoService photoService)
        : IRequestHandler<UpdateCourseCommandRequest, Result<Guid>>
    {
        private readonly MasterNetDbContext _context = context;
        private readonly IPhotoService _photoService = photoService;

        public async Task<Result<Guid>> Handle(
            UpdateCourseCommandRequest request, 
            CancellationToken cancellationToken
        )
        {
            var courseId = request.CourseId;

            // ✅ CARGAR SOLO EL CURSO PRINCIPAL (sin Include para evitar concurrency issues)
            var course = await _context.Courses!
                .FirstOrDefaultAsync(x => x.Id == courseId, cancellationToken);
            
            if (course is null)
            {
                return Result<Guid>.Failure("Course does not exist");
            }

            // ✅ ACTUALIZACIÓN PARCIAL - Solo actualizar campos proporcionados
            var hasUpdates = false; // Track if any updates were made
            
            if (!string.IsNullOrWhiteSpace(request.UpdateCourseRequest.Title))
            {
                course.Title = request.UpdateCourseRequest.Title.Trim();
                hasUpdates = true;
            }

            if (!string.IsNullOrWhiteSpace(request.UpdateCourseRequest.Description))
            {
                course.Description = request.UpdateCourseRequest.Description.Trim();
                hasUpdates = true;
            }

            if (request.UpdateCourseRequest.PublicationDate.HasValue)
            {
                course.PublicationDate = request.UpdateCourseRequest.PublicationDate;
                hasUpdates = true;
            }

            // ✅ MANEJO DE FOTO - Usar operaciones directas en DbContext
            if (request.UpdateCourseRequest.Photo is not null)
            {
                // Eliminar fotos anteriores de Cloudinary y base de datos
                var existingPhotos = await _context.Set<Photo>()
                    .Where(p => p.CourseId == courseId)
                    .ToListAsync(cancellationToken);

                foreach (var oldPhoto in existingPhotos)
                {
                    if (!string.IsNullOrEmpty(oldPhoto.PublicId))
                    {
                        await _photoService.DeletePhoto(oldPhoto.PublicId);
                    }
                }

                // Remover fotos de la base de datos
                if (existingPhotos.Any())
                {
                    _context.Set<Photo>().RemoveRange(existingPhotos);
                }

                // Subir nueva foto
                var photoUploadResult = await _photoService.AddPhoto(request.UpdateCourseRequest.Photo);
                var newPhoto = new Photo
                {
                    Id = Guid.NewGuid(),
                    Url = photoUploadResult.Url,
                    PublicId = photoUploadResult.PublicId,
                    CourseId = course.Id,
                };
                
                // Agregar nueva foto directamente al contexto
                await _context.Set<Photo>().AddAsync(newPhoto, cancellationToken);
                hasUpdates = true;

                /* 
                // 📚 IMPLEMENTACIÓN ALTERNATIVA: MÚLTIPLES FOTOS (COMENTADA)
                // Si un curso pudiera tener múltiples fotos, sería así:
                
                var photoUploadResult = await _photoService.AddPhoto(request.UpdateCourseRequest.Photo);
                var newPhoto = new Photo
                {
                    Id = Guid.NewGuid(),
                    Url = photoUploadResult.Url,
                    PublicId = photoUploadResult.PublicId,
                    CourseId = course.Id,
                };
                
                // Solo agregar sin borrar fotos existentes
                await _context.Set<Photo>().AddAsync(newPhoto, cancellationToken);
                */
            }

            // ✅ ACTUALIZAR INSTRUCTORES - Usar operaciones directas en DbContext
            if (request.UpdateCourseRequest.InstructorIds?.Count > 0)
            {
                // Validar que todos los instructores existen
                var instructors = await _context.Instructors!
                    .Where(i => request.UpdateCourseRequest.InstructorIds.Contains(i.Id))
                    .ToListAsync(cancellationToken);

                if (instructors.Count != request.UpdateCourseRequest.InstructorIds.Count)
                {
                    var foundIds = instructors.Select(i => i.Id).ToList();
                    var missingIds = request.UpdateCourseRequest.InstructorIds.Except(foundIds).ToList();
                    return Result<Guid>.Failure($"Instructors not found with IDs: {string.Join(", ", missingIds)}");
                }

                // Remover relaciones existentes
                var existingInstructorRelations = await _context.Set<CourseInstructor>()
                    .Where(ci => ci.CourseId == courseId)
                    .ToListAsync(cancellationToken);

                if (existingInstructorRelations.Any())
                {
                    _context.Set<CourseInstructor>().RemoveRange(existingInstructorRelations);
                }

                // Agregar nuevas relaciones
                var newInstructorRelations = request.UpdateCourseRequest.InstructorIds.Select(instructorId => 
                    new CourseInstructor 
                    { 
                        CourseId = course.Id, 
                        InstructorId = instructorId 
                    }).ToList();

                await _context.Set<CourseInstructor>().AddRangeAsync(newInstructorRelations, cancellationToken);
                hasUpdates = true;
            }

            // ✅ ACTUALIZAR PRECIOS - Usar operaciones directas en DbContext
            if (request.UpdateCourseRequest.PriceIds?.Count > 0)
            {
                // Validar que todos los precios existen
                var prices = await _context.Prices!
                    .Where(p => request.UpdateCourseRequest.PriceIds.Contains(p.Id))
                    .ToListAsync(cancellationToken);

                if (prices.Count != request.UpdateCourseRequest.PriceIds.Count)
                {
                    var foundIds = prices.Select(p => p.Id).ToList();
                    var missingIds = request.UpdateCourseRequest.PriceIds.Except(foundIds).ToList();
                    return Result<Guid>.Failure($"Prices not found with IDs: {string.Join(", ", missingIds)}");
                }

                // Remover relaciones existentes
                var existingPriceRelations = await _context.Set<CoursePrice>()
                    .Where(cp => cp.CourseId == courseId)
                    .ToListAsync(cancellationToken);

                if (existingPriceRelations.Any())
                {
                    _context.Set<CoursePrice>().RemoveRange(existingPriceRelations);
                }

                // Agregar nuevas relaciones
                var newPriceRelations = request.UpdateCourseRequest.PriceIds.Select(priceId => 
                    new CoursePrice 
                    { 
                        CourseId = course.Id, 
                        PriceId = priceId 
                    }).ToList();

                await _context.Set<CoursePrice>().AddRangeAsync(newPriceRelations, cancellationToken);
                hasUpdates = true;
            }

            // ✅ VERIFICAR QUE SE HICIERON CAMBIOS ANTES DE GUARDAR
            if (!hasUpdates)
            {
                // Lanzar ValidationException para que el ExceptionMiddleware la formatee correctamente
                var errors = new List<ValidationError>
                {
                    new("UpdateRequest", "No valid fields provided for update. Title and Description cannot be empty or whitespace-only.")
                };
                throw new MasterNet.Application.Core.ValidationException(errors);
            }

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result 
                        ? Result<Guid>.Success(course.Id)
                        : Result<Guid>.Failure("Errors in updating the course");
        }
    }

    public class UpdateCourseCommandRequestValidator
    : AbstractValidator<UpdateCourseCommandRequest>
    {
        public UpdateCourseCommandRequestValidator()
        {
            RuleFor(x => x.UpdateCourseRequest).SetValidator(new UpdateCourseValidator());
            RuleFor(x => x.CourseId).NotNull();
        }
    }

}