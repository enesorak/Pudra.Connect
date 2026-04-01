using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Models.Products;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;
using Pudra.Connect.App.Views;


namespace Pudra.Connect.App.ViewModels;

public partial class ScanViewModel : ObservableObject
{
    private readonly IProductService _service;
    private readonly IConnectivityService _connectivityService;

    [ObservableProperty] private bool isBusy;


    public ScanViewModel(IProductService service, IConnectivityService connectivityService)
    {
        _service = service;
        _connectivityService = connectivityService;
    }


    [RelayCommand]
    private async Task BarcodeDetected(string barcode)
    {
        if (IsBusy || string.IsNullOrEmpty(barcode)) return;

        IsBusy = true;
        Debug.WriteLine($"[ScanViewModel] Barcode Detected: {barcode}. Getting ProductCode...");

        try
        {
            // 1. ADIM: Barkoddan sadece ürün kodunu al. Bu çok hafif bir API çağrısıdır.
            var product = await _service.GetByBarcodeAsync(barcode);
            if (product != null)
            {
                Debug.WriteLine($"[ScanViewModel] ProductCode found: {product}. Navigating...");
                // 2. ADIM: Detay sayfasına ürün kodu ile git.
                await Shell.Current.GoToAsync(nameof(ProductDetailPage), new Dictionary<string, object>
                {
                    { "Product", product }
                });
            }
            else
            {
                Debug.WriteLine("[ScanViewModel] No ProductCode found for this barcode.");
                await Shell.Current.DisplayAlert("Bulunamadı", "Bu barkoda ait bir ürün bulunamadı.", "Tamam");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ScanViewModel] CRITICAL ERROR: {ex}");
            await Shell.Current.DisplayAlert("Hata", "İşlem sırasında bir hata oluştu.", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CloseAsync()
    {
        // Shell'in navigasyon yığınında bir önceki sayfaya geri dönmesini sağlar.
        await Shell.Current.GoToAsync("..");
    }
}