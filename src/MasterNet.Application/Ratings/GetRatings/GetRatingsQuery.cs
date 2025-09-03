namespace MasterNet.Application.Ratings.GetRatings;

public record RatingDTO(
    string? Student,
    int? Score,
    string? Comment,
    string? CourseName
)
{
    public RatingDTO(): this(null, null, null, null)
    {
    }
}