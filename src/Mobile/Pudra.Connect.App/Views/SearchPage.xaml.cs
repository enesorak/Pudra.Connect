using Pudra.Connect.App.ViewModels;

namespace Pudra.Connect.App.Views;

public partial class SearchPage : ContentPage
{
    public SearchPage(SearchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    
   
    
    /// <summary>
    /// Klavyedeki 'Ara' butonuna basıldığında tetiklenir.
    /// </summary>
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        // Klavyenin kapanması için SearchBar'ın odağını kaldır.
        SearchBarObject.Unfocus();
        

        if (BindingContext is SearchViewModel vm && vm.SearchCommand.CanExecute(null))
        {
            vm.SearchCommand.Execute(null);
        }
    }
 

    private void ItemsView_OnScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        SearchBarObject.Unfocus();
    }
    
    /// <summary>
    /// SearchBar dışındaki herhangi bir yere dokunulduğunda tetiklenir.
    /// </summary>
    private void OnPageAreaTapped(object sender, TappedEventArgs e)
    {
        // Klavyenin kapanması için SearchBar'ın odağını kaldır.
        SearchBarObject.Unfocus();
    }
}