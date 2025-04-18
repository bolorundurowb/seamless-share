using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace SeamlessShareApi.Background;

public class UrlMetadataService : BackgroundService
{
    private readonly ILogger<UrlMetadataService> _logger;
    private readonly ConcurrentQueue<Guid> _processingQueue = new();
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore = new(5); // Limit concurrent requests

    public UrlMetadataService(ILogger<UrlMetadataService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("MetadataService");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MetadataScraper/1.0)");
    }

    // Call this method when data is entered into the database
    public void EnqueueProcessing(Guid id)
    {
        _processingQueue.Enqueue(id);
        _logger.LogInformation("Enqueued ID {Id} for processing", id);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("UrlMetadataService is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_processingQueue.TryDequeue(out var id))
                {
                    // Don't await here so we can process multiple items concurrently
                    _ = ProcessItemAsync(id, stoppingToken).ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Error processing ID {Id}", id);
                        }
                    }, stoppingToken);
                }
                else
                {
                    await Task.Delay(1000, stoppingToken); // Wait if queue is empty
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error in processing loop");
                await Task.Delay(5000, stoppingToken); // Wait before retrying after error
            }
        }

        _logger.LogInformation("UrlMetadataService is stopping");
    }

    private async Task ProcessItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Processing started for ID {Id}", id);

            // In a real implementation, you would fetch the URL from your database using the ID
            // string url = await _dbContext.GetUrlByIdAsync(id, cancellationToken);
            // For this example, we'll simulate getting a URL
            string url = "https://example.com"; // Replace with your actual URL retrieval logic

            if (!string.IsNullOrWhiteSpace(url))
            {
                var metadata = await ExtractUrlMetadataAsync(url, cancellationToken);
                _logger.LogInformation("Extracted metadata for {Url}: {@Metadata}", url, metadata);

                // Save metadata to database or process it further
                // await _dbContext.SaveMetadataAsync(id, metadata, cancellationToken);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<UrlMetadata> ExtractUrlMetadataAsync(string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format", nameof(url));

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            using var response = await _httpClient.SendAsync(request, 
                HttpCompletionOption.ResponseHeadersRead, 
                cancellationToken);

            response.EnsureSuccessStatusCode();

            if (!IsHtmlContent(response.Content.Headers.ContentType))
            {
                _logger.LogWarning("URL {Url} does not return HTML content", url);
                return new UrlMetadata { Url = url, ContentType = response.Content.Headers.ContentType?.MediaType };
            }

            // Read only the first 64KB to find meta tags (most sites have them in the first few KB)
            var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var buffer = new byte[65536]; // 64KB
            var bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
            var html = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

            return ParseHtmlMetadata(url, html);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to fetch URL {Url}", url);
            return new UrlMetadata { Url = url, Error = ex.Message };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing URL {Url}", url);
            return new UrlMetadata { Url = url, Error = "Unexpected processing error" };
        }
    }

    private bool IsHtmlContent(MediaTypeHeaderValue? contentType)
    {
        if (contentType == null) return false;
        
        return contentType.MediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase) ||
               contentType.MediaType.Equals("application/xhtml+xml", StringComparison.OrdinalIgnoreCase);
    }

    private UrlMetadata ParseHtmlMetadata(string url, string html)
    {
        var metadata = new UrlMetadata { Url = url };

        try
        {
            // Regex pattern to match meta tags (case-insensitive, with various attribute orders)
            var metaTagPattern = new Regex(
                @"<meta\s+(?:[^>]*?\b(?:property|name)\s*=\s*(?:""([^""]*)""|'([^']*)'|([^'\""\s>]+))\s+(?:[^>]*?\bcontent\s*=\s*(?:""([^""]*)""|'([^']*)'|([^'\""\s>]+)))[^>]*>",
                RegexOptions.IgnoreCase);

            var titleMatch = Regex.Match(html, @"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase);
            if (titleMatch.Success)
            {
                metadata.Title = WebUtility.HtmlDecode(titleMatch.Groups[1].Value.Trim());
            }

            foreach (Match match in metaTagPattern.Matches(html))
            {
                var propertyGroup = match.Groups[1].Success ? match.Groups[1] :
                                  match.Groups[2].Success ? match.Groups[2] :
                                  match.Groups[3];

                var contentGroup = match.Groups[4].Success ? match.Groups[4] :
                                 match.Groups[5].Success ? match.Groups[5] :
                                 match.Groups[6];

                if (!propertyGroup.Success || !contentGroup.Success) continue;

                var property = propertyGroup.Value.ToLower();
                var content = WebUtility.HtmlDecode(contentGroup.Value);

                switch (property)
                {
                    case "description":
                        metadata.Description = content;
                        break;
                    case "og:title":
                        metadata.OgTitle = content;
                        break;
                    case "og:description":
                        metadata.OgDescription = content;
                        break;
                    case "og:image":
                        metadata.OgImage = content;
                        break;
                    case "og:type":
                        metadata.OgType = content;
                        break;
                    case "og:url":
                        metadata.OgUrl = content;
                        break;
                    case "twitter:title":
                        metadata.TwitterTitle = content;
                        break;
                    case "twitter:description":
                        metadata.TwitterDescription = content;
                        break;
                    case "twitter:image":
                        metadata.TwitterImage = content;
                        break;
                    case "twitter:card":
                        metadata.TwitterCard = content;
                        break;
                }
            }

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing HTML metadata for {Url}", url);
            metadata.Error = "Metadata parsing error";
            return metadata;
        }
    }
}

public class UrlMetadata
{
    public string Url { get; set; }
    public string ContentType { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string OgTitle { get; set; }
    public string OgDescription { get; set; }
    public string OgImage { get; set; }
    public string OgType { get; set; }
    public string OgUrl { get; set; }
    public string TwitterTitle { get; set; }
    public string TwitterDescription { get; set; }
    public string TwitterImage { get; set; }
    public string TwitterCard { get; set; }
    public string Error { get; set; }
}
