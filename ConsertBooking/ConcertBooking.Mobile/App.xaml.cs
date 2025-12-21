using ConcertBooking.Mobile.Views;

namespace ConcertBooking.Mobile;

public partial class App : Application
{
    public App(ConcertsPage page)
    {
        InitializeComponent();
        MainPage = new NavigationPage(page);
    }
}
