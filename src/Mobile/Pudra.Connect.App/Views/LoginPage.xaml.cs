using Pudra.Connect.App.ViewModels;

namespace Pudra.Connect.App.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    
    private void OnLoginClicked(object sender, EventArgs e)
    {
        if (BindingContext is LoginViewModel vm && vm.LoginCommand.CanExecute(null))
        {
            vm.LoginCommand.Execute(null);
        }
    }
    
    private async void OnConnectionTestClicked(object sender, EventArgs e)
    {
        // Tüm DI ve servis katmanlarını atlayarak,
        // burada sıfırdan bir HttpClient oluşturuyoruz.
        var handler = new HttpClientHandler();

        // FİZİKSEL CİHAZDA TEST İÇİN BU IP'Yİ KENDİ BİLGİSAYARININ GÜNCEL IP'Sİ İLE DEĞİŞTİR
        string devMachineIp = "192.168.1.137";
        string targetUrl = "";

#if ANDROID
            // Gerçek cihazda http daha sorunsuz çalışabilir, onu deneyelim
            targetUrl = $"http://{devMachineIp}:5147"; 
#elif IOS
        // Gerçek iOS cihaz için
        targetUrl = $"http://{devMachineIp}:5147";
#endif

        // Sertifika doğrulamasını tamamen devre dışı bırakan callback
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        var httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        try
        {
            // Backend'in ana adresine basit bir GET isteği atıyoruz.
            // Swagger sayfasının yüklenmesi bile başarılı bir bağlantı demektir.
            await DisplayAlert("Test Başladı", $"Şu adrese bağlanılıyor: {targetUrl}", "Tamam");
            
            var response = await httpClient.GetAsync(targetUrl);

            // Eğer buraya kadar geldiyse ve bir exception fırlatmadıysa, bağlantı başarılıdır.
            await DisplayAlert("BAŞARILI!",
                $"Sunucuya ulaşıldı! Cevap Status Kodu: {(int)response.StatusCode} ({response.ReasonPhrase})",
                "Harika!");
        }
        catch (Exception ex)
        {
            // Eğer bir hata olursa, hatanın tüm detaylarını ekranda görelim.
            await DisplayAlert("BAŞARISIZ!",
                $"Sunucuya ulaşılamadı. Hata: {ex.Message}\n\nDetaylar: {ex}",
                "Tamam");
        }
    }
    
    
    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }
}