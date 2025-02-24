using System.Text.RegularExpressions;

namespace SeamlessShareApi.Utils;

public static partial class TextCategorizer
{
    public static bool IsLink(string input) => !string.IsNullOrWhiteSpace(input) && UrlRegex().IsMatch(input);

    [GeneratedRegex(@"^(http|https|ftp):\/\/[^\s/$.?#].[^\s]*$", RegexOptions.IgnoreCase, "en-NG")]
    private static partial Regex UrlRegex();
}
