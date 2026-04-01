namespace Pudra.Connect.App.Models.Products;

 
public class ProductResultDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal RetailPrice { get; set; }
    public decimal WholesalePrice { get; set; }
}