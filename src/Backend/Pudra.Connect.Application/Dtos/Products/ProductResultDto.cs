namespace Pudra.Connect.Application.Dtos.Products;

public class ProductResultDto
{
    public string ProductCode { get; set; }
    public string Name { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal WholesalePrice { get; set; }
}