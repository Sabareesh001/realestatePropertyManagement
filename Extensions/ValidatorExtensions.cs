using FluentValidation;

namespace propertyManagement.Extensions;

/// <summary>
/// Static class containing reusable validation rules for FluentValidation.
/// </summary>
public static class ValidatorExtensions
{
    /// <summary>
    /// Validates that a password matches standard security rules (e.g. minimum length).
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="minLength">The minimum required length for the password.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder, int minLength = 12)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(minLength).WithMessage($"Password must be at least {minLength} characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }

    /// <summary>
    /// Validates that a string is a valid email address.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email address is required.")
            .Must(email => !string.IsNullOrEmpty(email) && !email.Any(char.IsWhiteSpace))
            .WithMessage("Email address must not contain any whitespace.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }

    /// <summary>
    /// Validates that a phone number is in a valid international or domestic format.
    /// Accepts digits, optional leading +, spaces, hyphens, and parentheses. Length 7–15 digits.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> ValidPhone<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[\d\s\-\(\)]{7,15}$").WithMessage("Phone number must be 7–15 digits and may include +, spaces, hyphens, or parentheses.")
            .Must(phone =>
            {
                var digitsOnly = System.Text.RegularExpressions.Regex.Replace(phone ?? "", @"\D", "");
                return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
            }).WithMessage("Phone number must contain between 7 and 15 digits.");
    }

    /// <summary>
    /// Validates that a person's name contains only letters, spaces, hyphens, and apostrophes.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="fieldName">The display name of the field for error messages.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> ValidPersonName<T>(this IRuleBuilder<T, string> ruleBuilder, string fieldName = "Name")
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"{fieldName} is required.")
            .MinimumLength(2).WithMessage($"{fieldName} must be at least 2 characters.")
            .MaximumLength(100).WithMessage($"{fieldName} cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s'\-]+$").WithMessage($"{fieldName} can only contain letters, spaces, hyphens, and apostrophes.");
    }

    /// <summary>
    /// Validates a property title — allows letters, digits, spaces, hyphens, commas, and periods. No shell/script special characters.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> ValidPropertyTitle<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-,\.]+$").WithMessage("Title can only contain letters, digits, spaces, hyphens, commas, and periods.");
    }

    /// <summary>
    /// Validates an Indian IFSC code format: 4 uppercase letters, '0', then 6 alphanumeric characters.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> ValidIfscCode<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("IFSC code is required.")
            .Length(11).WithMessage("IFSC code must be exactly 11 characters.")
            .Matches(@"^[A-Z]{4}0[A-Z0-9]{6}$").WithMessage("IFSC code must follow the format: 4 uppercase letters, '0', then 6 alphanumeric characters (e.g., SBIN0001234).");
    }

    /// <summary>
    /// Validates a bank account number — digits only, between 9 and 18 digits.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<T, string> ValidBankAccountNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Account number is required.")
            .Matches(@"^\d{9,18}$").WithMessage("Account number must contain only digits and be between 9 and 18 digits long.");
    }
}
