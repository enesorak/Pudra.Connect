using Pudra.Connect.App.Models;
using Pudra.Connect.App.Models.Products;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.Services;

public class ProductService : IProductService
{
    private readonly IApiService _apiService;
    public ProductService(IApiService apiService) { _apiService = apiService; }

    public Task<ProductResultDto?> GetByBarcodeAsync(string barcode)
    {
        return _apiService.GetAsync<ProductResultDto>($"/api/products/barcode/{barcode}");
    }

    public Task<List<ProductResultDto>?> SearchByNameAsync(string name)
    {
        return _apiService.GetAsync<List<ProductResultDto>>($"/api/products/search?name={name}");
    }
}