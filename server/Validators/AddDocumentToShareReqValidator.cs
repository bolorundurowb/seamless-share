using FluentValidation;
using SeamlessShareApi.Models.Request;

namespace SeamlessShareApi.Validators;

public class AddDocumentToShareReqValidator : AbstractValidator<AddDocumentToShareReq>
{
    public AddDocumentToShareReqValidator()
    {
        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("Please select a document to upload.")
            .Must(BeAValidDocument)
            .WithMessage("Please upload a valid document.")
            .Must(SatisfiesFileSizeLimit)
            .WithMessage("Document size must be less than 10MiB.");
    }

    private bool BeAValidDocument(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return false;

        var allowedExtensions = new[]
        {
            ".pdf", ".txt", ".docx", ".xlsx", ".doc", ".xls", ".rtf", ".odt", ".pptx", ".ppt", ".csv", ".md"
        };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        return allowedExtensions.Contains(fileExtension);
    }

    private bool SatisfiesFileSizeLimit(IFormFile? file)
    {
        if (file == null)
            return true;

        return file.Length <= Constants.MaxFileSizeInBytes;
    }
}
