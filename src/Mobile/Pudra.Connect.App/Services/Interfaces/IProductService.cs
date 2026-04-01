using Pudra.Connect.App.Models;
using Pudra.Connect.App.Models.Products;

namespace Pudra.Connect.App.Services.Interfaces;

public interface IProductService
{  
    
    Task<ProductResultDto?> GetByBarcodeAsync(string barcode);
    Task<List<ProductResultDto>?> SearchByNameAsync(string name);

}