namespace StyleCart.Application.DTOs.ProductImages;

public class ProductImageResponse
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? AltText { get; set; }

    public int DisplayOrder { get; set; }
}