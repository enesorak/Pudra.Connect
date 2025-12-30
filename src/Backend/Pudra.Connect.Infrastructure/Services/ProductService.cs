using Pudra.Connect.Application.Dtos.Products;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Infrastructure.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public Task<ProductResultDto?> GetByBarcodeAsync(string barcode)
    {
        // İşi doğrudan Repository'ye paslar.
        return productRepository.GetByBarcodeAsync(barcode);
    }

    public Task<IEnumerable<ProductResultDto>> SearchByNameAsync(string name)
    {
        // İşi doğrudan Repository'ye paslar.
        return productRepository.SearchByNameAsync(name);
    }
    
    public Task<ProductResultDto?> GetDetailByCodeAsync(string productCode)
    {
        return productRepository.GetDetailByCodeAsync(productCode);
    }
}