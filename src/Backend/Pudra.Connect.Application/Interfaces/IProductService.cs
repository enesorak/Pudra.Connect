using Pudra.Connect.Application.Dtos.Products;

namespace Pudra.Connect.Application.Interfaces;

public interface IProductService
{
    /// <summary>
    /// Verilen barkoda ait temel ürün bilgilerini getirir.
    /// </summary>
    Task<ProductResultDto?> GetByBarcodeAsync(string barcode);

    /// <summary>
    /// Verilen isme göre arama yapar ve temel ürün bilgilerini liste olarak döndürür.
    /// </summary>
    Task<IEnumerable<ProductResultDto>> SearchByNameAsync(string name);

    Task<ProductResultDto?> GetDetailByCodeAsync(string productCode);
}