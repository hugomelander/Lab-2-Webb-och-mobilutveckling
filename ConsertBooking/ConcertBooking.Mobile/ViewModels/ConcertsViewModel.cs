using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConcertBooking.Mobile.Models;
using ConcertBooking.Mobile.Services;
using ConcertBooking.Mobile.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ConcertBooking.Mobile.ViewModels;

public partial class ConcertsViewModel : ObservableObject
{
    private readonly ConcertApi _api;
    private readonly IServiceProvider _services;
    private readonly IDialogService _dialogs;

    private INavigation? _navigation;

    public ObservableCollection<ConcertDto> Concerts { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private ConcertDto? selectedConcert;

    // FIX 1: Lagt till IDialogService i konstruktorn
    public ConcertsViewModel(ConcertApi api, IServiceProvider services, IDialogService dialogs)
    {
        _api = api;
        _services = services;
        _dialogs = dialogs;
    }

    public void SetNavigation(INavigation navigation)
        => _navigation = navigation;

    [RelayCommand]
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

    partial void OnSelectedConcertChanged(ConcertDto? value)
    {
        if (value is not null)
            _ = OpenPerformancesAsync(value);
    }

    private async Task OpenPerformancesAsync(ConcertDto concert)
    {
        try
        {
            if (_navigation is null) return;

            var page = _services.GetRequiredService<PerformancesPage>();
            await page.InitializeAsync(concert.Id);

            await _navigation.PushAsync(page);
        }
        finally
        {
            SelectedConcert = null;
        }
    }


}