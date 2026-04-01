using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IUserService _userService;
    private readonly IScanAnalyticsService _analyticsService;

    public ProductsController(
        IProductService productService, 
        IUserService userService,
        IScanAnalyticsService analyticsService)
    {
        _productService = productService;
        _userService = userService;
        _analyticsService = analyticsService;
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // Tarama limitini kontrol et
        var canScan = await _userService.CanUserScanAsync(userId);
        if (!canScan)
        {
            return StatusCode(403, new { message = "Trial limit reached. Please wait for account approval or contact support." });
        }

        var product = await _productService.GetByBarcodeAsync(barcode);
        
        // Scan'i logla
        await _analyticsService.LogScanAsync(
            userId, 
            barcode, 
            product?.ProductCode, 
            product?.Name, 
            product != null
        );

        // Tarama sayacını artır
        await _userService.IncrementScanCountAsync(userId);
        
        return product != null ? Ok(product) : NotFound();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        var products = await _productService.SearchByNameAsync(name);
        return Ok(products);
    }
    
    [HttpGet("{productCode}")]
    public async Task<IActionResult> GetProductDetail(string productCode)
    {
        var productDetail = await _productService.GetDetailByCodeAsync(productCode);
        if (productDetail == null)
        {
            return NotFound();
        }
        return Ok(productDetail);
    }
}