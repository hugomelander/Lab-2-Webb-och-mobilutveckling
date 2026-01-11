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
    public ObservableCollection<MyBookingDto> MyBookings { get; } = new();
    private INavigation? _navigation;

    public ObservableCollection<ConcertDto> Concerts { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private ConcertDto? selectedConcert;

    public ConcertsViewModel(ConcertApi api, IServiceProvider services, IDialogService dialogs)
    {
        _api = api;
        _services = services;
        _dialogs = dialogs;
    }

    public void SetNavigation(INavigation navigation)
        => _navigation = navigation;

    [RelayCommand]
    public async Task LoadAsync() // Ladda konserter
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

    partial void OnSelectedConcertChanged(ConcertDto? value) // Hantera val av konsert
    {
        if (value is not null)
            _ = OpenPerformancesAsync(value);
    }

    private async Task OpenPerformancesAsync(ConcertDto concert) // Visa föreställningar för vald konsert
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
     
    [RelayCommand] // Visa mina bokningar
    private async Task ShowMyBookingsAsync()
    {
        var email = await _dialogs.PromptAsync("Mina bokningar", "Ange din e-post:");
        if (string.IsNullOrWhiteSpace(email)) return;

        try
        {
            IsBusy = true;
            var items = await _api.GetBookingsByEmailAsync(email.Trim());

            MyBookings.Clear();
            foreach (var b in items) MyBookings.Add(b);

            if (!MyBookings.Any())
                await _dialogs.AlertAsync("Info", "Inga bokningar hittades.");
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        finally { IsBusy = false; }
    }

    [RelayCommand] // Avboka bokning
    private async Task CancelBookingAsync(MyBookingDto booking)
    {
        if (booking == null) return;

        try
        {
            // Bekräfta avbokning
            bool confirm = await _dialogs.ConfirmAsync("Avboka", "Vill du ta bort bokningen?", "Ja", "Nej");
            if (!confirm) return;

            IsBusy = true;

            // Utför avbokningen via ID
            await _api.DeleteBookingAsync(booking.Id);

            // Uppdatera UI
            MyBookings.Remove(booking);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("Fel", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

}