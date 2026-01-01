using ConcertBooking.Mobile.ViewModels;

namespace ConcertBooking.Mobile.Views;

public partial class PerformancesPage : ContentPage
{
    private readonly PerformancesViewModel _vm;

    public PerformancesPage(PerformancesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    public async Task InitializeAsync(int concertId)
    {
        await _vm.LoadAsync(concertId);
    }

}
