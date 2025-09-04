using FluentValidation;

namespace MasterNet.Application.Courses.UpdateCourse;

/// <summary>
/// Validador para actualizaciones parciales de curso.
/// Solo valida los campos que se proporcionan (no son null).
/// </summary>
public class UpdateCourseValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseValidator()
    {
        // Validar Title solo si se proporciona (actualización parcial)
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title cannot be empty if provided")
            .When(x => x.Title != null);

        // Validar Description solo si se proporciona (actualización parcial)
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description cannot be empty if provided")
            .When(x => x.Description != null);

        // PublicationDate puede ser null (curso en borrador), no requiere validación

        // Validar InstructorIds solo si se proporciona (actualización parcial)
        RuleFor(x => x.InstructorIds)
            .Must(ids => ids!.Count > 0)
            .WithMessage("InstructorIds cannot be empty if provided")
            .When(x => x.InstructorIds != null);

        // Validar PriceIds solo si se proporciona (actualización parcial)
        RuleFor(x => x.PriceIds)
            .Must(ids => ids!.Count > 0)
            .WithMessage("PriceIds cannot be empty if provided")
            .When(x => x.PriceIds != null);
    }
}