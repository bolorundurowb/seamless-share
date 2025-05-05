namespace SeamlessShareApi;

public static class Constants
{
    public const int MaxFileSizeInBytes = 10 * 1024 * 1024; // 10MiB

    public const int MaxTextLength = 5000;

    public const int MaxFileExpiryInSecs = 60 * 60 * 3; // 3 hours

    public const string DocumentsFolderName = "docs";

    public const string ImagesFolderName = "images";
    
    public const int MaxAnonymousShareAgeInDays = 7;
    
    public const int MaxOwnedShareAgeInDays = 30;
}
