namespace MasterNet.Application.Photos.GetPhoto;

public record PhotoDTO(
    Guid? Id,
    string? Url,
    Guid? CourseId
)
{
    public PhotoDTO(): this(null, null, null)
    {
    }
}