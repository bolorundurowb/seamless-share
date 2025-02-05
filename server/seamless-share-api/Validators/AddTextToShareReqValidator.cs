using FluentValidation;
using SeamlessShareApi.Models.Request;

namespace SeamlessShareApi.Validators;

public class AddTextToShareReqValidator : AbstractValidator<AddTextToShareReq>
{
    public AddTextToShareReqValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .MaximumLength(Constants.MaxTextLength)
            .WithMessage($"Content cannot exceed {Constants.MaxTextLength} characters.");
    }
}
