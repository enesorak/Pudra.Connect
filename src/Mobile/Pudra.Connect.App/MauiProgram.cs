using BarcodeScanning;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
 
using Pudra.Connect.App.Services;
using Pudra.Connect.App.ViewModels;
using Pudra.Connect.App.Views;
 
using Microcharts.Maui;
using Pudra.Connect.App.Services.Interfaces; // <-- Bu using'i ekle

namespace Pudra.Connect.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
		.UseMauiCommunityToolkit()  
			
			.UseMauiCommunityToolkitMarkup()
			.UseBarcodeScanning()
		 
			.UseMicrocharts()
			.ConfigureMauiHandlers(handlers =>
			{
				// --- YENİ EKLENEN BÖLÜM ---
				// SearchBar'ın varsayılan Handler'ına ek bir konfigürasyon ekliyoruz.
				SearchBarHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
				{
					// Bu kodun sadece iOS ve MacCatalyst üzerinde çalışmasını sağlıyoruz.
#if IOS || MACCATALYST
					// Platforma özel (native) kontrolün arkaplanını kaldırıyoruz.
					handler.PlatformView.BackgroundImage = new UIKit.UIImage();
					handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
				});
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbols");
			});
		
		// HttpClientFactory yapılandırması
		builder.Services.AddHttpClient("PudraApi", client =>
		{
			client.BaseAddress = new Uri("https://app.pudraa.de");
			client.Timeout = TimeSpan.FromSeconds(30);
		})
#if DEBUG
		.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
		})
#endif
		;

		builder.Services.AddSingleton<IApiService, ApiService>();
		builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
 
		builder.Services.AddSingleton<LoginPage>();
		builder.Services.AddSingleton<LoginViewModel>();

		builder.Services.AddTransient<ScanPage>(); // Bu sayfalar her seferinde yeniden oluşsun
		builder.Services.AddTransient<ScanViewModel>();

	 

		builder.Services.AddTransient<ProductDetailPage>();
		builder.Services.AddTransient<ProductDetailViewModel>();
		
		
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<SettingsViewModel>();
		
		builder.Services.AddTransient<SearchPage>();
		builder.Services.AddTransient<SearchViewModel>();
		
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<DashboardViewModel>();
		
		builder.Services.AddTransient<DetailedReportPage>();
		builder.Services.AddTransient<DetailedReportViewModel>();
		
		
		builder.Services.AddTransient<UserManagementPage>();
		builder.Services.AddTransient<UserManagementViewModel>();

		builder.Services.AddTransient<UserDetailPage>();
		builder.Services.AddTransient<UserDetailViewModel>();
		
		builder.Services.AddTransient<MyProfilePage>();
		builder.Services.AddTransient<MyProfileViewModel>();
		
	 

// Yüksek seviye, özelliğe özel servisler
		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<IUserService, UserService>();
		builder.Services.AddSingleton<IProductService, ProductService>();
		builder.Services.AddSingleton<IDashboardService, DashboardService>();
		builder.Services.AddTransient<IProfileService, ProfileService>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}