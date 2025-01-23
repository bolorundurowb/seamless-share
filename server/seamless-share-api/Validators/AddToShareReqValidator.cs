using FluentValidation;
using SeamlessShareApi.Models.Request;

namespace SeamlessShareApi.Validators;

public class AddToShareReqValidator : AbstractValidator<AddToShareReq>
{
    public AddToShareReqValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.");
    }
}
