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
    public async Task CancelBookingByEmail(PerformanceDto performance)
    {
        if (performance == null) return;

        if (performance.Bookings == null || !performance.Bookings.Any())
        {
            await _dialogs.AlertAsync("Info", "Det finns inga bokningar att ta bort för denna föreställning.");
            return;
        }

        // Be användaren ange e-post
        var inputEmail = await _dialogs.PromptAsync(
            "Avboka biljetter",
            "Ange e-postadressen som användes vid bokningen:");

        if (string.IsNullOrWhiteSpace(inputEmail))
            return;

        var normalizedEmail = inputEmail.Trim().ToLower();

        // Kontrollera om det finns bokningar för JUST DENNA föreställning
        var matchingBookings = performance.Bookings
            .Where(b => b.Email.ToLower() == normalizedEmail)
            .ToList();

        if (!matchingBookings.Any())
        {
            await _dialogs.AlertAsync(
                "Fel",
                "Ingen bokning hittades för angiven e-postadress på just denna föreställning.");
            return;
        }

        //  Bekräftelse
        var confirm = await _dialogs.ConfirmAsync(
            "Bekräfta avbokning",
            $"Vill du avboka {matchingBookings.Count} bokning(ar) för föreställningen den {performance.DateTime:yyyy-MM-dd HH:mm}?");

        if (!confirm)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = "";

            // Anropa API:et med BÅDE e-post och PerformanceId
            await _api.DeleteBookingsByEmailAsync(normalizedEmail, performance.Id);

            await _dialogs.AlertAsync(
                "Klart",
                $"Dina {matchingBookings.Count} bokningar för denna föreställning har tagits bort.");

            // Laddar om listan för att uppdatera BookingsCount och listan
            await LoadAsync(ConcertId);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await _dialogs.AlertAsync(
                "Fel",
                "Kunde inte avboka: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
