namespace StyleCart.Application.DTOs.ProductImages;

public class CreateProductImageRequest
{
    public int ProductId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? AltText { get; set; }

    public int DisplayOrder { get; set; }
}