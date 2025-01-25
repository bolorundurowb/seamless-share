using FluentValidation;
using SeamlessShareApi.Models.Request;

namespace SeamlessShareApi.Validators;

public class AddFileToShareReqValidator : AbstractValidator<AddFileToShareReq>
{
    public AddFileToShareReqValidator()
    {
        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("Please select a file to upload.")
            .Must(BeAValidFile)
            .WithMessage("Please upload a valid file.")
            .Must(SatisfiesFileSizeLimit)
            .WithMessage("File size must be less than 10MiB.");
    }

    private bool BeAValidFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return false;

        var allowedExtensions = new[]
        {
            ".pdf", ".txt", ".docx", ".xlsx", ".jpg", ".png", ".mp4", ".webm", ".avi", ".mov", ".wmv", ".flv", ".mpg",
            ".mpeg", ".gif", ".bmp", ".tif", ".tiff"
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
