using FluentValidation;
using SeamlessShareApi.Models.Request;

namespace SeamlessShareApi.Validators;

public class LoginReqValidator : AbstractValidator<LoginReq>
{
    public LoginReqValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Please enter your email address.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.")
            .MaximumLength(255)
            .WithMessage("Email address cannot exceed 255 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Please enter your password.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}
