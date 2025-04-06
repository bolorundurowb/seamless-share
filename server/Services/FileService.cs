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

    public async Task<(string, FileMetadata)?> Upload(string shareCode, string subDirectoryName, IFormFile file)
    {
        _logger.LogDebug("Uploading a file. {ShareCode}", shareCode);

        try
        {
            await using var stream = file.OpenReadStream();
            var fileCreateRequest = new FileCreateRequest
            {
                fileName = file.FileName,
                useUniqueFileName = true,
                tags = ["shares", shareCode, subDirectoryName],
                folder = $"seamless_share/{shareCode}/{subDirectoryName}",
                isPrivateFile = true,
                file = await StreamToByteArrayAsync(stream)
            };

            var uploadResponse = await _imagekit.UploadAsync(fileCreateRequest);

            if (uploadResponse is null || uploadResponse.HttpStatusCode is < 200 or >= 300)
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

    public async Task Delete(string externalFileId)
    {
        _logger.LogDebug("Deleting a file from the external provider. {ExternalFileId}", externalFileId);

        var deleteResult = await _imagekit.DeleteFileAsync(externalFileId);

        if (deleteResult.HttpStatusCode is < 200 or >= 300)
        {
            _logger.LogError("An error occurred while deleting a file.{ExternalFileId}", externalFileId);
            throw new Exception($"An error occurred while deleting a file. {deleteResult.Raw}");
        }

        _logger.LogInformation("File successfully deleted from external provider. {ExternalFileId}",
            externalFileId);
    }

    public string GetSignedUrl(string url) => _imagekit.Url(new Transformation())
        .Src(url)
        .Signed()
        .ExpireSeconds(Constants.MaxFileExpiryInSecs)
        .Generate();

    private string? CalculateChecksum(Stream stream)
    {
        if (!stream.CanSeek)
        {
            _logger.LogError("File stream does not support seeking.");
            return null;
        }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating file checksum.");
            return null;
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }
    
    private static async Task<byte[]> StreamToByteArrayAsync(Stream stream)
    {
        var buffer = new byte[16 * 1024];
        using var ms = new MemoryStream();
        int read;
        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0) 
            await ms.WriteAsync(buffer, 0, read);
        return ms.ToArray();
    }
}