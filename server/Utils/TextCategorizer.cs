using System.Text.RegularExpressions;

namespace SeamlessShareApi.Utils;

public static partial class TextCategorizer
{
    public static bool IsLink(string input) => !string.IsNullOrWhiteSpace(input) && UrlRegex().IsMatch(input);

    [GeneratedRegex(
        @"^(?:(https?|ftp|ws):\/\/)?(?:([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}|localhost|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(?::\d+)?(?:\/[^\s?#]*)?(?:\?[^#\s]*)?(?:#[^\s]*)?$",
        RegexOptions.IgnoreCase, "en-NG")]
    private static partial Regex UrlRegex();
}
