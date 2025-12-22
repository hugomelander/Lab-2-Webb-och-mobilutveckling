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
}
