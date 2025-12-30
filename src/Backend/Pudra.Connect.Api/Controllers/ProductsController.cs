using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Bu Controller'a sadece giriş yapmış kullanıcılar erişebilir.
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode)
    {
        var product = await productService.GetByBarcodeAsync(barcode);
        return product != null ? Ok(product) : NotFound();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        var products = await productService.SearchByNameAsync(name);
        return Ok(products);
    }
    
    [HttpGet("{productCode}")]
    public async Task<IActionResult> GetProductDetail(string productCode)
    {
        var productDetail = await productService.GetDetailByCodeAsync(productCode);
        if (productDetail == null)
        {
            return NotFound();
        }
        return Ok(productDetail);
    }
    
}