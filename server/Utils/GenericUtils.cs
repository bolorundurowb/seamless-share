namespace SeamlessShareApi.Utils;

internal static class GenericUtils
{
    public static DateTime GetExpirationDate(DateTime? date, bool isAuthenticated)
    {
        var expiryTime = isAuthenticated ? TimeSpan.FromDays(30) : TimeSpan.FromDays(7);
        return date.GetValueOrDefault().Add(expiryTime);
    }
}
