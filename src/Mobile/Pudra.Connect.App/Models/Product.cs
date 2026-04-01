namespace Pudra.Connect.App.Models;

public class Product
{
    public string ProductCode { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string Size { get; set; }
    public decimal WholesalePrice { get; set; }
    public decimal RetailPrice { get; set; }
    public string Barcode { get; set; }
    public DateTime OperationDate { get; set; }
}