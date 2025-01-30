using System.Security.Cryptography;
using System.Text;
using Imagekit.Sdk;
using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class FileService
{
    private readonly ILogger<FileService> _logger;
    private readonly ImagekitClient _imagekit;

    public FileService(ILogger<FileService> logger, IConfiguration configuration)
    {
        var imgKitSettings = configuration.GetSection("ImageKit");
        var publicKey = imgKitSettings["PublicKey"];
        var privateKey = imgKitSettings["PrivateKey"];
        var urlEndpoint = imgKitSettings["UrlEndpoint"];

        if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(urlEndpoint))
            throw new Exception(
                "Imagekit configuration settings are missing. Please configure 'Imagekit:PublicKey', 'Imagekit:PrivateKey', and 'Imagekit:UrlEndpoint' in appsettings.json.");

        _logger = logger;
        _imagekit = new ImagekitClient(publicKey, privateKey, urlEndpoint);
    }

    public async Task<(string, FileMetadata)?> Upload(string shareCode, IFormFile file)
    {
        _logger.LogDebug("Uploading a file. {ShareCode}", shareCode);

        try
        {
            await using var stream = file.OpenReadStream();
            var fileCreateRequest = new FileCreateRequest
            {
                fileName = file.FileName,
                useUniqueFileName = true,
                tags = ["files", "shares", shareCode],
                folder = $"/seamless_share/{shareCode}",
                file = stream
            };

            var uploadResponse = await _imagekit.UploadAsync(fileCreateRequest);

            if (uploadResponse is null)
            {
                _logger.LogError("An error occurred while uploading a file. {ShareCode}", shareCode);
                return null;
            }

            var metadata = new FileMetadata(
                uploadResponse.fileId,
                uploadResponse.name,
                Path.GetExtension(file.FileName),
                uploadResponse.size,
                file.ContentType,
                CalculateChecksum(stream)
            );
            return (uploadResponse.url, metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while uploading a file. {ShareCode}", shareCode);
            return null;
        }
    }

    public async Task<FileSchema> Create(Guid ownerId, string fileUrl, FileMetadata fileMetadata)
    {
        _logger.LogDebug("Creating a new file. {OwnerId} {FileUrl} {FileMetadata}", ownerId, fileUrl, fileMetadata);

        var file = new FileSchema(ownerId, fileMetadata, fileUrl);
        await file.SaveAsync();

        _logger.LogDebug("File successfully persisted. {FileId} {FileUrl} {FileMetadata}", file.Id, fileUrl,
            fileMetadata);

        return file;
    }

    private static string CalculateChecksum(Stream stream)
    {
        if (!stream.CanSeek)
            throw new ArgumentException("The stream must support seeking.");

        var originalPosition = stream.Position;
        stream.Position = 0;

        try
        {
            using var hashAlgorithm = MD5.Create();
            var hashBytes = hashAlgorithm.ComputeHash(stream);

            var builder = new StringBuilder();
            foreach (var t in hashBytes)
                builder.Append(t.ToString("x2")); // "x2" for lowercase hex

            return builder.ToString();
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }
}
