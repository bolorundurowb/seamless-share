using System.Security.Cryptography;
using System.Text;
using Imagekit.Sdk;
using meerkat;
using MongoDB.Driver.Linq;
using SeamlessShareApi.Models.Data;

namespace SeamlessShareApi.Services;

public class DocumentService(ILogger<DocumentService> logger)
{
    public async Task<DocumentSchema> Create(Guid ownerId, string documentUrl, FileMetadata documentMetadata,
        string? appVersion,
        AppSource? appSource)
    {
        logger.LogDebug("Creating a new document. {OwnerId} {DocumentUrl} {DocumentMetadata}", ownerId, documentUrl, documentMetadata);

        var document = new DocumentSchema(ownerId, documentMetadata, documentUrl, appVersion, appSource);
        await document.SaveAsync();

        logger.LogDebug("Document successfully persisted. {DocumentId} {DocumentUrl} {DocumentMetadata}", document.Id, documentUrl,
            documentMetadata);

        return document;
    }

    public Task<List<DocumentSchema>> GetAll(Guid shareId) => Meerkat.Query<DocumentSchema>()
        .Where(x => x.ShareId == shareId && x.IsArchived == false)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();

    public async Task DeleteOne(Guid shareId, Guid documentId)
    {
        logger.LogDebug("Request to delete a document. {ShareId} {DocumentId}", shareId, documentId);

        var document = await Meerkat.Query<DocumentSchema>()
            .FirstOrDefaultAsync(x => x.ShareId == shareId && x.Id == (object)documentId);

        if (document is null)
        {
            logger.LogWarning("Document to be deleted does not exist. {ShareId} {DocumentId}", shareId, documentId);
            return;
        }

        await Meerkat.RemoveByIdAsync<DocumentSchema>(documentId);

        logger.LogDebug("Document successfully deleted. {ShareId} {DocumentId}", shareId, documentId);
    }

    public async Task ArchiveOne(Guid shareId, Guid documentId)
    {
        logger.LogDebug("Request to archive a document. {ShareId} {DocumentId}", shareId, documentId);

        var document = await Meerkat.Query<DocumentSchema>()
            .FirstOrDefaultAsync(x => x.ShareId == shareId && x.Id == (object)documentId);

        if (document is null)
        {
            logger.LogWarning("Document to be archived does not exist. {ShareId} {DocumentId}", shareId, documentId);
            return;
        }

        document.Archive();
        await document.SaveAsync();

        logger.LogDebug("Document successfully archived. {ShareId} {DocumentId}", shareId, documentId);
    }
}