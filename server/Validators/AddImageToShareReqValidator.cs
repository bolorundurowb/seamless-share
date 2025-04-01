using FluentValidation;
using SeamlessShareApi.Models.Request;

namespace SeamlessShareApi.Validators;

public class AddImageToShareReqValidator : AbstractValidator<AddImageToShareReq>
{
    public AddImageToShareReqValidator()
    {
        RuleFor(x => x.Content)
            .NotNull()
            .WithMessage("Please select a image to upload.")
            .Must(BeAValidImage)
            .WithMessage("Please upload a valid image.")
            .Must(SatisfiesFileSizeLimit)
            .WithMessage("Image size must be less than 10MiB.");
    }

    private bool BeAValidImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return false;

        var allowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".svg", ".gif", ".bmp", ".webp"
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
