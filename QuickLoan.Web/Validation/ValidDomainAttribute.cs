using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class ValidDomainAttribute : ValidationAttribute
{
    private static readonly Regex DomainRegex =
        new(@"^(?!-)(?:[a-zA-Z0-9-]{1,63}\.)+[a-zA-Z]{2,}$",
            RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success; // let [Required] handle this

        var domain = value.ToString()?.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(domain))
            return ValidationResult.Success;

        // Reject emails or @domains
        if (domain.Contains("@"))
            return new ValidationResult(ErrorMessage ?? "Please enter a valid domain (e.g. gmail.com).");

        if (!DomainRegex.IsMatch(domain))
            return new ValidationResult(ErrorMessage ?? "Please enter a valid domain (e.g. gmail.com).");

        return ValidationResult.Success;
    }
}
