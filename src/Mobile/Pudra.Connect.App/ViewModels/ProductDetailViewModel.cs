using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Models;
using Pudra.Connect.App.Models.Products;
using Pudra.Connect.App.Services.Interfaces;

namespace Pudra.Connect.App.ViewModels;

/// <summary>
/// Ürün detay sayfasının ViewModel'i.
/// Tek görevi, navigasyonla gelen 'Product' nesnesini alıp,
/// XAML arayüzünün bu nesnenin özelliklerine bağlanmasını sağlamaktır.
/// </summary>
[QueryProperty(nameof(Product), "Product")]
public partial class ProductDetailViewModel : ObservableObject
{
    /// <summary>
    /// Scan veya Search sayfasından gönderilen ve tüm ürün detaylarını
    /// içeren ana veri nesnesi. XAML bu özelliğe bağlanır.
    /// CommunityToolkit.Mvvm sayesinde, bu 'product' alanı için otomatik olarak
    /// 'Product' adında public bir property oluşturulur.
    /// </summary>
    [ObservableProperty]
    ProductResultDto product;

    /// <summary>
    /// Bu ViewModel'in herhangi bir servise veya API çağrısına ihtiyacı olmadığı için
    /// constructor'ı boştur.
    /// </summary>
    public ProductDetailViewModel()
    {
        // Başlangıçta boş bir nesne atamak, null hatalarını önler.
        product = new ProductResultDto();
    }
}