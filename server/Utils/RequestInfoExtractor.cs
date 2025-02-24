using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Utils;

internal static class RequestInfoExtractor
{
    public static (string? IpAddress, string? UserAgent) ExtractIpAddressAndUserAgent(HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        return (ipAddress, userAgent);
    }

    public static (string? AppVersion, AppSource? Source) ExtractAppVersionAndSource(HttpContext httpContext)
    {
        // Try to get the app version from the X-App-Version header
        if (httpContext.Request.Headers.TryGetValue("X-App-Version", out var headerValue))
        {
            var parts = headerValue.ToString()
                .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var appVersion = parts.Length > 0 ? parts[0] : null;
            AppSource? source = parts.Length > 1 && Enum.TryParse(parts[1], out AppSource parsedSource)
                ? parsedSource
                : null;

            return (appVersion, source);
        }

        // Fall back to the User-Agent header if X-App-Version is not defined
        if (httpContext.Request.Headers.TryGetValue("User-Agent", out var userAgent))
        {
            var userAgentString = userAgent.ToString();

            AppSource? source;
            if (userAgentString.Contains("Android", StringComparison.OrdinalIgnoreCase))
                source = AppSource.Android;
            else if (userAgentString.Contains("iPhone") || userAgentString.Contains("iPad") ||
                     userAgentString.Contains("iOS", StringComparison.OrdinalIgnoreCase))
                source = AppSource.iOS;
            else if (userAgentString.Contains("Mozilla") || userAgentString.Contains("Safari") ||
                     userAgentString.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                source = AppSource.Web;
            else
                source = AppSource.Unknown;

            return (null, source);
        }

        return (null, AppSource.Unknown);
    }
}
