using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ConcertBooking.Mobile.Models;
using ConcertBooking.Mobile.Services;

namespace ConcertBooking.Mobile.ViewModels;

public class ConcertsViewModel : INotifyPropertyChanged
{
    private readonly ConcertApi _api;

    public ObservableCollection<ConcertDto> Concerts { get; } = new();

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public Command LoadCommand { get; }

    public ConcertsViewModel(ConcertApi api)
    {
        _api = api;
        LoadCommand = new Command(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = "";

            var items = await _api.GetConcertsAsync();

            Concerts.Clear();
            foreach (var c in items)
                Concerts.Add(c);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
