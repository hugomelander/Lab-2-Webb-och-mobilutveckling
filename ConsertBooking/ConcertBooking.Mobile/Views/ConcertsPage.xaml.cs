using ConcertBooking.Mobile.ViewModels;

namespace ConcertBooking.Mobile.Views;

public partial class ConcertsPage : ContentPage
{

    public ConcertsPage(ConcertsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        vm.SetNavigation(Navigation); // enda “wire-up”-raden
    }

    protected override async void OnAppearing()
    {   
        base.OnAppearing();

        if (BindingContext is ConcertsViewModel vm)
            await vm.LoadAsync();
    }
}
