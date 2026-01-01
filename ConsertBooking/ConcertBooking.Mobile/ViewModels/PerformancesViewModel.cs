using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConcertBooking.Mobile.Models;
using ConcertBooking.Mobile.Services;
using System.Collections.ObjectModel;

namespace ConcertBooking.Mobile.ViewModels;

public partial class PerformancesViewModel : ObservableObject
{
    private readonly ConcertApi _api;
    private readonly IDialogService _dialogs;

    public ObservableCollection<PerformanceDto> Performances { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private int concertId;

    [ObservableProperty]
    private PerformanceDto? selectedPerformance;

    public PerformancesViewModel(ConcertApi api, IDialogService dialogs)
    {
        _api = api;
        _dialogs = dialogs;
    }

    public async Task LoadAsync(int concertId)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = "";
            ConcertId = concertId;

            var items = await _api.GetPerformancesAsync(concertId);

            Performances.Clear();
            foreach (var p in items)
                Performances.Add(p);
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

    // triggas när användaren klickar en performance
    partial void OnSelectedPerformanceChanged(PerformanceDto? value)
    {
        if (value is not null)
            _ = BookAsync(value);
    }

    [RelayCommand]
    private async Task BookAsync(PerformanceDto performance)
    {
        try
        {
            // Avmarkera direkt så man kan klicka samma igen
            SelectedPerformance = null;

            var name = await _dialogs.PromptAsync("Boka", "Ange namn:");
            if (string.IsNullOrWhiteSpace(name)) return;

            var email = await _dialogs.PromptAsync("Boka", "Ange email:");
            if (string.IsNullOrWhiteSpace(email)) return;

            IsBusy = true;
            ErrorMessage = "";

            var bookingId = await _api.CreateBookingAsync(new CreateBookingDto
            {
                PerformanceId = performance.Id,
                Name = name.Trim(),
                Email = email.Trim()
            });

            await _dialogs.AlertAsync("Klart!", $"Bokning skapad (ID: {bookingId}).");

            // ladda om så BookingsCount uppdateras
            await LoadAsync(ConcertId);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await _dialogs.AlertAsync("Fel", "Kunde inte skapa bokning.\n\n" + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    public async Task CancelLatestBookingAsync(PerformanceDto performance)
    {
        if (performance.Bookings == null || !performance.Bookings.Any())
        {
            await _dialogs.AlertAsync("Info", "Det finns inga bokningar att ta bort.");
            return;
        }

        var lastBooking = performance.Bookings.Last();

        var inputEmail = await _dialogs.PromptAsync("Bekräfta avbokning",
            $"Ange e-postadressen som användes för bokningen av {lastBooking.Name}:");

        if (string.IsNullOrWhiteSpace(inputEmail)) return;

        // 4. Kontrollera om e-posten matchar (case-insensitive)
        if (inputEmail.Trim().ToLower() != lastBooking.Email.ToLower())
        {
            await _dialogs.AlertAsync("Fel", "E-postadressen matchar inte bokningen. Du kan bara avboka dina egna biljetter.");
            return;
        }

        // 5. Om det matchar, kör avbokningen
        try
        {
            IsBusy = true;
            await _api.DeleteBookingAsync(lastBooking.Id);

            await _dialogs.AlertAsync("Klart", "Bokningen har tagits bort.");

            // Uppdatera listan så siffran minskar
            await LoadAsync(ConcertId);
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("Fel", "Kunde inte avboka: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
