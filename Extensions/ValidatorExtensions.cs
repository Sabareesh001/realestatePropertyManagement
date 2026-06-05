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
            .MinimumLength(minLength).WithMessage($"Password must be at least {minLength} characters long.");
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
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
