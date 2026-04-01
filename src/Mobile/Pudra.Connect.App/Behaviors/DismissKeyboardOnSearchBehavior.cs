namespace Pudra.Connect.App.Behaviors;

/// <summary>
/// Bir SearchBar'a takıldığında, arama butonuna basıldığı anda
/// klavyenin otomatik olarak kapanmasını sağlayan bir davranıştır.
/// </summary>
public class DismissKeyboardOnSearchBehavior : Behavior<SearchBar>
{
    /// <summary>
    /// Bu behavior bir SearchBar'a takıldığında çalışır.
    /// </summary>
    protected override void OnAttachedTo(SearchBar bindable)
    {
        base.OnAttachedTo(bindable);
        // SearchBar'ın SearchButtonPressed olayına abone oluyoruz.
        bindable.SearchButtonPressed += OnSearchButtonPressed;
    }

    /// <summary>
    /// Bu behavior SearchBar'dan söküldüğünde çalışır.
    /// </summary>
    protected override void OnDetachingFrom(SearchBar bindable)
    {
        base.OnDetachingFrom(bindable);
        // Bellek sızıntılarını önlemek için abonelikten çıkıyoruz.
        bindable.SearchButtonPressed -= OnSearchButtonPressed;
    }

    /// <summary>
    /// Arama butonuna basıldığında, SearchBar'ın odağını kaldırarak klavyeyi kapat.
    /// </summary>
    private void OnSearchButtonPressed(object? sender, System.EventArgs e)
    {
        if (sender is SearchBar searchBar)
        {
            searchBar.Unfocus();
        }
    }
}