using FluentValidation;

namespace MasterNet.Application.Courses.CreateCourse;

public class CreateCourseValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseValidator()
    {
        RuleFor(x => x.Title).NotEmpty(); //with message to specify message
        RuleFor(x => x.Description).NotEmpty();
    }
}