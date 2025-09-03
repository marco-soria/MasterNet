namespace MasterNet.Application.Prices.GetPrices;

public record PriceDTO(
    Guid? Id,
    string? Name,
    decimal? CurrentPrice,
    decimal? PromotionalPrice
)
{
    public PriceDTO(): this(null, null, null, null)
    {
    }
}