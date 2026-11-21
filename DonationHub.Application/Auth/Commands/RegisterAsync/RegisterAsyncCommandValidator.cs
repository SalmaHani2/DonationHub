using FluentValidation;

namespace DonationHub.Application.Auth.Commands.Register;

public class RegisterAsyncCommandValidator
    : AbstractValidator<RegisterAsyncCommand>
{
    public RegisterAsyncCommandValidator()
    {
        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .WithErrorCode("Validation.Failed");
    }
}