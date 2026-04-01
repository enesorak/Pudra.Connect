using Pudra.Connect.App.ViewModels;
using System.Diagnostics;
using BarcodeScanning;

namespace Pudra.Connect.App.Views;

public partial class ScanPage : ContentPage
{
  
    private readonly ScanViewModel _viewModel;
    private readonly BarcodeDrawable _drawable = new(); // Çizim için yardımcı sınıf


    public ScanPage(ScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
        GraphicsView.Drawable = _drawable; // GraphicsView'e neyi çizeceğini söylüyoruz.
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckAndRequestCameraPermissionAsync();

        // Kamerayı başlatıyoruz
        CameraView.CameraEnabled = true;
        Debug.WriteLine("[ScanPage] Camera Enabled.");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Kamerayı kapatıyoruz
        CameraView.CameraEnabled = false;
        Debug.WriteLine("[ScanPage] Camera Disabled.");
    }

    private void CameraView_OnDetectionFinished(object sender, OnDetectionFinishedEventArg e)
    {
        // 1. Bulunan barkodları çizim sınıfımıza gönder
        _drawable.BarcodeResults = e.BarcodeResults;
        // 2. Ekranda çerçevenin görünmesi için GraphicsView'i yeniden çizmeye zorla
        GraphicsView.Invalidate();
        
        var firstResult = e.BarcodeResults.FirstOrDefault();
        if (firstResult is not null && _viewModel.BarcodeDetectedCommand.CanExecute(null))
        {
            // 3. Barkod değerini ViewModel'deki komuta gönder
            _viewModel.BarcodeDetectedCommand.Execute(firstResult.DisplayValue);
        }
    }

    private class BarcodeDrawable : IDrawable
    {
        public IReadOnlyCollection<BarcodeResult> BarcodeResults { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (BarcodeResults is not null)
            {
                canvas.StrokeSize = 5;
                canvas.StrokeColor = Colors.Yellow;
                
                foreach (var barcode in BarcodeResults)
                {
                    // Bulunan her barkodun etrafına bir dikdörtgen çiz.
                    canvas.DrawRectangle(barcode.PreviewBoundingBox);
                }
            }
        }
    }

    /// <summary>
    /// Kamera iznini kontrol eder ve eğer izin verilmemişse, kullanıcıdan izin ister.
    /// </summary>
    private async Task CheckAndRequestCameraPermissionAsync()
    {
        // 1. Adım: Mevcut izin durumunu kontrol et.
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        // 2. Adım: Eğer izin zaten verilmişse, metottan çık, her şey yolunda.
        if (status == PermissionStatus.Granted)
        {
            return;
        }

        // 3. Adım: Eğer izin verilmemişse veya reddedilmişse, kullanıcıdan izin iste.
        // Bu komut, işletim sisteminin standart "İzin ver / Reddet" pop-up'ını gösterir.
        status = await Permissions.RequestAsync<Permissions.Camera>();

        // 4. Adım: Kullanıcının verdiği cevabı kontrol et.
        if (status != PermissionStatus.Granted)
        {
            // Kullanıcı izni vermediyse, onu bilgilendir.
            await DisplayAlert("İzin Gerekli",
                "Barkod tarama özelliği için kamera izni vermeniz gerekmektedir. Lütfen telefonun Ayarlar menüsünden uygulama izinlerini kontrol edin.",
                "Tamam");
        }
    }
}