using FluentValidation;
using Tijori.Application.DTOs.Auth;

namespace Tijori.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(phone => ValidationPatterns.PhoneDigitsRegex().IsMatch(phone.Replace(" ", string.Empty)))
            .WithMessage("Phone number format is invalid.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(email => ValidationPatterns.EmailRegex().IsMatch(email))
            .WithMessage("Email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Must(PasswordRules.IsValid)
            .WithMessage(PasswordRules.GetErrorMessage());

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");

        RuleFor(x => x.AcceptTerms)
            .Equal(true)
            .WithMessage("You must accept the terms and conditions.");
    }
}

public class VerifyRegistrationOtpRequestValidator : AbstractValidator<VerifyRegistrationOtpRequest>
{
    public VerifyRegistrationOtpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(email => ValidationPatterns.EmailRegex().IsMatch(email))
            .WithMessage("Email format is invalid.");

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .Matches(@"^\d{4}$")
            .WithMessage("OTP must be a 4-digit code.");
    }
}

public class ResendRegistrationOtpByEmailRequestValidator : AbstractValidator<ResendRegistrationOtpByEmailRequest>
{
    public ResendRegistrationOtpByEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(email => ValidationPatterns.EmailRegex().IsMatch(email))
            .WithMessage("Email format is invalid.");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

public class SocialLoginRequestValidator : AbstractValidator<SocialLoginRequest>
{
    public SocialLoginRequestValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty();
    }
}

public class SendForgotPasswordOtpByEmailRequestValidator : AbstractValidator<SendForgotPasswordOtpByEmailRequest>
{
    public SendForgotPasswordOtpByEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(email => ValidationPatterns.EmailRegex().IsMatch(email))
            .WithMessage("Email format is invalid.");
    }
}

public class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
{
    public VerifyOtpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(email => ValidationPatterns.EmailRegex().IsMatch(email))
            .WithMessage("Email format is invalid.");

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .Matches(@"^\d{4}$")
            .WithMessage("OTP must be a 4-digit code.");
    }
}

public class ResendOtpByEmailRequestValidator : AbstractValidator<ResendOtpByEmailRequest>
{
    public ResendOtpByEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(email => ValidationPatterns.EmailRegex().IsMatch(email))
            .WithMessage("Email format is invalid.");
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.ResetToken)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Must(PasswordRules.IsValid)
            .WithMessage(PasswordRules.GetErrorMessage());

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}
