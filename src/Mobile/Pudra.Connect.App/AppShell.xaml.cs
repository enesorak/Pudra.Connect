using Pudra.Connect.App.Views;

namespace Pudra.Connect.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

  
        Routing.RegisterRoute(nameof(ProductDetailPage), typeof(ProductDetailPage));
        Routing.RegisterRoute(nameof(DetailedReportPage), typeof(DetailedReportPage));
        Routing.RegisterRoute(nameof(UserManagementPage), typeof(UserManagementPage));
   
        Routing.RegisterRoute(nameof(UserDetailPage), typeof(UserDetailPage));
        
        Routing.RegisterRoute(nameof(MyProfilePage), typeof(MyProfilePage));

        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
    }

   

    /// <summary>
    /// Güvenli depolamadan kullanıcının rolünü okur ve Dashboard sekmesini
    /// sadece Admin rolü için görünür hale getirir.
    /// </summary>
    private async Task SetAdminTabVisibility()
    {
        var userRole = await SecureStorage.Default.GetAsync("user_role");
        var isAdmin = userRole == "Admin";
        
        if (DashboardTab != null)
        {
            DashboardTab.IsVisible = isAdmin;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Token var mı diye kontrol et
        var token = await SecureStorage.Default.GetAsync("auth_token");

        if (string.IsNullOrEmpty(token))
        {
            // Eğer token yoksa veya boşsa, kullanıcının login sayfasında olduğundan emin ol.
            // Zaten başlangıç sayfamız olduğu için ekstra bir şey yapmaya gerek olmayabilir,
            // ama bu garanti bir yöntemdir.
            await GoToAsync("//LoginPage");
        }
        else
        {
            // Eğer token VARSA, kullanıcının ana TabBar'ı görmesini sağla.
            // Ayrıca, Admin sekmesinin görünürlüğünü de burada kontrol edelim.
            await SetAdminTabVisibility();
            await GoToAsync("//Main");
        }
    }
    
    /// <summary>
    /// Login veya Çıkış yapma gibi navigasyon olaylarından sonra da bu kontrolü tetikleyebiliriz.
    /// </summary>
    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);
        
        // Login veya Logout sonrası ana sayfaya gelinip gelinmediğini kontrol et
        if (args.Current.Location.OriginalString.Contains("//Main") || args.Current.Location.OriginalString.Contains("//LoginPage"))
        {
            await SetAdminTabVisibility();
        }
    }
}