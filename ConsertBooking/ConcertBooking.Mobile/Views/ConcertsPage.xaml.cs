using ConcertBooking.Mobile.ViewModels;

namespace ConcertBooking.Mobile.Views;

public partial class ConcertsPage : ContentPage
{
    private readonly ConcertsViewModel _vm;

    public ConcertsPage(ConcertsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
