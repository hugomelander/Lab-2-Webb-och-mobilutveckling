namespace ConcertBooking.Mobile.Services;

public static class ApiConfig
{
#if ANDROID
    // Android emulator -> hostmaskinens localhost
    public const string BaseUrl = "http://10.0.2.2:5182";
#else
    // Windows / Mac
    public const string BaseUrl = "http://localhost:5182";
#endif
}
