namespace ConcertBooking.Mobile.Services;

public class DialogService : IDialogService
{
    public async Task<string?> PromptAsync(string title, string message, string ok = "OK", string cancel = "Avbryt")
    {
        var page = Application.Current?.MainPage;
        if (page is null) return null;

        return await page.DisplayPromptAsync(title, message, ok, cancel);
    }

    public async Task AlertAsync(string title, string message, string ok = "OK")
    {
        var page = Application.Current?.MainPage;
        if (page is null) return;

        await page.DisplayAlert(title, message, ok);
    }

    public async Task<bool> ConfirmAsync(string title, string message, string ok = "Ja", string cancel = "Nej")
    {
        var page = Application.Current?.Windows[0]?.Page;

        if (page == null) return false;

        return await page.DisplayAlert(title, message, ok, cancel);
    }
}
