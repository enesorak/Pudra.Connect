using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pudra.Connect.App.Models;
using Pudra.Connect.App.Models.Products;
using Pudra.Connect.App.Services;
using Pudra.Connect.App.Services.Interfaces;
using Pudra.Connect.App.Views;

namespace Pudra.Connect.App.ViewModels;

 public partial class SearchViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private bool _searchAttempted = false; // Arama yapılıp yapılmadığını takip eden bayrak

        [ObservableProperty]
        private string _searchText = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowNoResultsMessage))] // IsBusy değiştiğinde, mesajın görünürlüğü de değişebilir
        private bool _isBusy;

        /// <summary>
        /// Arama sonuçlarını tutan ve UI'a bağlı olan koleksiyon.
        /// </summary>
        public ObservableCollection<ProductResultDto> SearchResults { get; } = new();

        /// <summary>
        /// "Sonuç bulunamadı" mesajının görünüp görünmeyeceğini belirleyen özellik.
        /// Sadece arama yapıldıktan sonra ve sonuç yoksa true olur.
        /// </summary>
        public bool ShowNoResultsMessage => _searchAttempted && !IsBusy && !SearchResults.Any();

        public SearchViewModel(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// SearchBar'dan tetiklenen arama komutu.
        /// </summary>
        [RelayCommand]
        private async Task SearchAsync()
        {
            // Eğer zaten bir arama yapılıyorsa veya arama metni boşsa, işlemi başlatma.
            if (IsBusy || string.IsNullOrWhiteSpace(SearchText))
            {
                return;
            }

            IsBusy = true;
            _searchAttempted = false; // Yeni arama başlarken bayrağı sıfırla
            SearchResults.Clear();
            OnPropertyChanged(nameof(ShowNoResultsMessage)); // "Sonuç yok" mesajını hemen gizle

            try
            {
                var results = await _productService.SearchByNameAsync(SearchText);

                if (results != null)
                {
                    foreach (var product in results)
                    {
                        SearchResults.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SearchViewModel] Search failed: {ex.Message}");
                await Shell.Current.DisplayAlert("Hata", "Arama sırasında bir hata oluştu.", "Tamam");
            }
            finally
            {
                IsBusy = false;
                _searchAttempted = true; // Arama işlemi tamamlandı olarak işaretle
                OnPropertyChanged(nameof(ShowNoResultsMessage)); // Mesajın görünürlüğünü yeniden hesapla
            }
        }

        /// <summary>
        /// Listeden bir ürün seçildiğinde (tıklama veya kaydırma ile) tetiklenir.
        /// </summary>
        [RelayCommand]
        private async Task SelectProductAsync(ProductResultDto? product)
        {
            if (product == null) return;

            // Seçilen ürün nesnesini doğrudan Detay Sayfası'na parametre olarak gönderiyoruz.
            await Shell.Current.GoToAsync(nameof(ProductDetailPage), new Dictionary<string, object>
            {
                { "Product", product }
            });
        }
    }