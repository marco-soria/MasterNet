namespace MasterNet.Application.Instructors.GetInstructors;

public record InstructorDTO(
    Guid? Id,
    string? FirstName,
    string? LastName,
    string? Degree
)
    
{
    public InstructorDTO() : this(null, null, null, null)
    {
    }
}