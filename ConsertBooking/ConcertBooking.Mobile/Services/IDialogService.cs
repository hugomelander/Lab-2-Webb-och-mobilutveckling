namespace ConcertBooking.Mobile.Services;

public interface IDialogService
{
    Task<string?> PromptAsync(string title, string message, string ok = "OK", string cancel = "Avbryt");
    Task AlertAsync(string title, string message, string ok = "OK");
    Task<bool> ConfirmAsync(string title, string message);

}
