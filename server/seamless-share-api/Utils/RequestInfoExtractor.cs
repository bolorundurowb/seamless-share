namespace SeamlessShareApi.Utils;

public class RequestInfoExtractor
{
    public static (string? IpAddress, string? UserAgent) ExtractIpAddressAndUserAgent(HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        return (ipAddress, userAgent);
    }
    
    public static (string? AppVersion, string? Source) ExtractAppVersionAndSource(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-App-Version", out var headerValue))
        {
            var parts = headerValue.ToString().Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var appVersion = parts.Length > 0 ? parts[0] : null;
            var source = parts.Length > 1 ? parts[1] : null;

            return (appVersion, source);
        }

        return (null, null);
    }
}
