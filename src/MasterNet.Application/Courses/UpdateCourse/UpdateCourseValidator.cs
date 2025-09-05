using FluentValidation;

namespace MasterNet.Application.Courses.UpdateCourse;

/// <summary>
/// Validador para actualizaciones parciales de curso.
/// Valida campos que se proporcionan, incluso si están vacíos (para dar errores específicos).
/// </summary>
public class UpdateCourseValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseValidator()
    {
        // Para actualizaciones parciales, solo validamos formato/estructura, no contenido
        // El command handler se encarga de verificar que al menos un campo válido se proporcione
        
        // Validar PublicationDate solo si se proporciona y no es DateTime.MinValue
        RuleFor(x => x.PublicationDate)
            .Must(ValidateDateTime)
            .WithMessage("Publication date must be a valid date if provided")
            .When(x => x.PublicationDate.HasValue);

        // Validar InstructorIds si se proporciona (incluso si está vacía)
        RuleFor(x => x.InstructorIds)
            .Must(ids => ids!.Count > 0)
            .WithMessage("InstructorIds cannot be empty if provided")
            .When(x => x.InstructorIds != null);

        // Validar PriceIds si se proporciona (incluso si está vacía)
        RuleFor(x => x.PriceIds)
            .Must(ids => ids!.Count > 0)
            .WithMessage("PriceIds cannot be empty if provided")
            .When(x => x.PriceIds != null);
    }

    private bool ValidateDateTime(DateTime? date)
    {
        // Si es null, es válido (campo opcional)
        if (date == null) return true;
        
        // Si tiene valor, debe ser una fecha válida (no DateTime.MinValue)
        return date != default(DateTime);
    }
}