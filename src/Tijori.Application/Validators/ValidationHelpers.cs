using System.Text.RegularExpressions;

namespace Tijori.Application.Validators;

public static class PasswordRules
{
    public const int MinimumLength = 8;

    public static bool IsValid(string password) =>
        password.Length >= MinimumLength &&
        password.Any(char.IsUpper) &&
        password.Any(char.IsLower) &&
        password.Any(char.IsDigit);

    public static string GetErrorMessage() =>
        "Password must be at least 8 characters and contain uppercase, lowercase, and numeric characters.";
}

public static partial class ValidationPatterns
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    public static partial Regex EmailRegex();

    [GeneratedRegex(@"^\d{7,15}$", RegexOptions.Compiled)]
    public static partial Regex PhoneDigitsRegex();
}

public static class CountryCodeDefaults
{
    public const string Default = "+92";
}

public static class IdentifierHelper
{
    public static bool IsEmail(string identifier) =>
        ValidationPatterns.EmailRegex().IsMatch(identifier.Trim());

    public static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    public static string NormalizeCountryCode(string? countryCode) =>
        string.IsNullOrWhiteSpace(countryCode) ? CountryCodeDefaults.Default : countryCode.Trim();

    public static (string CountryCode, string PhoneNumber) ParsePhoneIdentifier(string identifier)
    {
        var normalized = identifier.Trim().Replace(" ", string.Empty);

        if (normalized.StartsWith('+'))
        {
            if (normalized.StartsWith("+92") && normalized.Length > 3)
            {
                return ("+92", normalized[3..]);
            }

            if (normalized.StartsWith("+965") && normalized.Length > 4)
            {
                return ("+965", normalized[4..]);
            }

            var match = Regex.Match(normalized, @"^(\+\d{1,4})(\d+)$");
            if (match.Success)
            {
                return (match.Groups[1].Value, match.Groups[2].Value);
            }
        }

        return (CountryCodeDefaults.Default, normalized);
    }
}
