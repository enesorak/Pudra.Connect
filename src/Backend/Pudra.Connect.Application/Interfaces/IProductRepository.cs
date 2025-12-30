using Pudra.Connect.Application.Dtos.Products;

namespace Pudra.Connect.Application.Interfaces;

public interface IProductRepository
{
  

    Task<string?> GetRandomBarcodeAsync(); // YENİ METOT

    // Arama sayfası için, ürün bazında gruplanmış liste
    Task<IEnumerable<ProductResultDto>> SearchByNameAsync(string name);

    // Barkod okutulduğunda, o barkoda ait ürünün temel bilgilerini getirmek için
    Task<ProductResultDto?> GetByBarcodeAsync(string barcode);
        
    // --- YENİDEN EKLENEN KRİTİK METOT ---
    // Detay sayfasının, ürün koduna göre tam ürün detayını çekmesi için
    Task<ProductResultDto?> GetDetailByCodeAsync(string productCode);
}