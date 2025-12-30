using IronBarCode;
using Microsoft.AspNetCore.Mvc;
using Pudra.Connect.Application.Interfaces;

namespace Pudra.Connect.Api.Controllers;
 

[ApiController]
[Route("api/[controller]")]
public class TestToolsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public TestToolsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet("random-barcode-image")]
    public async Task<IActionResult> GetRandomBarcodeImage()
    {
        // Veritabanından rastgele bir barkod alıyoruz.
        var barcodeValue = await _productRepository.GetRandomBarcodeAsync();

        if (string.IsNullOrEmpty(barcodeValue))
        {
            return NotFound("Could not find any valid barcode in the system.");
        }

        try
        {
            // Barkodu oluşturuyoruz.
            GeneratedBarcode barcodeObject = BarcodeWriter.CreateBarcode(barcodeValue, BarcodeWriterEncoding.EAN13);
            barcodeObject.AddBarcodeValueTextBelowBarcode();
            barcodeObject.ResizeTo(400, 150);

            // --- DEĞİŞİKLİK BURADA ---
            // Resmin byte dizisi halini, PNG formatında üreten doğru metodu çağırıyoruz.
            byte[] barcodeAsBytes = barcodeObject.ToPngBinaryData();

            // Byte dizisini bir dosya olarak tarayıcıya gönderiyoruz.
            return File(barcodeAsBytes, "image/png");
        }
        catch (Exception ex)
        {
            // Eğer kütüphane burada bir hata fırlatırsa, bunu görebiliriz.
            return StatusCode(500, $"Barcode generation failed for value '{barcodeValue}': {ex.Message}");
        }
    }
}