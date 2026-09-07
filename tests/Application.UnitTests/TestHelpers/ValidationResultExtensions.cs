using FluentValidation.Results;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.TestHelpers;

/// <summary>
/// A small local stand-in for FluentValidation.TestHelper's TestValidate()/
/// ShouldHaveValidationErrorFor() — written directly against FluentValidation's own core
/// Validate() API (already a resolved dependency here) rather than adding the TestHelper
/// package, since restoring a new package hit a real, repeated connectivity failure to
/// api.nuget.org on this machine (NU1301, ~4 minutes of retries then a hard failure).
/// </summary>
public static class ValidationResultExtensions
{
    public static void ShouldHaveErrorFor(this ValidationResult result, string propertyName)
        => result.Errors.ShouldContain(
            e => e.PropertyName == propertyName,
            $"Expected a validation error for '{propertyName}' but none was raised. " +
            $"Actual errors: [{string.Join(", ", result.Errors.Select(e => e.PropertyName))}]");

    public static void ShouldNotHaveErrorFor(this ValidationResult result, string propertyName)
        => result.Errors.ShouldNotContain(
            e => e.PropertyName == propertyName,
            $"Expected no validation error for '{propertyName}' but got: " +
            $"\"{result.Errors.FirstOrDefault(e => e.PropertyName == propertyName)?.ErrorMessage}\"");
}
