using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="UpdateComplaintStatusDtoValidator"/> class.
/// </summary>
[TestFixture]
public class UpdateComplaintStatusDtoValidatorTests
{
    private UpdateComplaintStatusDtoValidator _validator;

    /// <summary>Sets up the validator under test.</summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new UpdateComplaintStatusDtoValidator();
    }

    /// <summary>Verifies that a valid status without a note passes.</summary>
    [Test]
    public async Task Validate_ValidStatusNoNote_Passes()
    {
        var result = await _validator.ValidateAsync(new UpdateComplaintStatusDto { StatusId = 2 });
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>Verifies that a valid status with a note under 500 chars passes.</summary>
    [Test]
    public async Task Validate_ValidStatusWithShortNote_Passes()
    {
        var result = await _validator.ValidateAsync(new UpdateComplaintStatusDto { StatusId = 3, Note = "Fixed the issue." });
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>Verifies that StatusId = 0 fails.</summary>
    [Test]
    public async Task Validate_StatusIdBelowRange_Fails()
    {
        var result = await _validator.ValidateAsync(new UpdateComplaintStatusDto { StatusId = 0 });
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Status must be between 1 and 5.")), Is.True);
    }

    /// <summary>Verifies that StatusId = 6 fails.</summary>
    [Test]
    public async Task Validate_StatusIdAboveRange_Fails()
    {
        var result = await _validator.ValidateAsync(new UpdateComplaintStatusDto { StatusId = 6 });
        Assert.That(result.IsValid, Is.False);
    }

    /// <summary>Verifies that a note exceeding 500 chars fails.</summary>
    [Test]
    public async Task Validate_NoteTooLong_Fails()
    {
        var result = await _validator.ValidateAsync(new UpdateComplaintStatusDto
        {
            StatusId = 2,
            Note = new string('A', 501)
        });
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Note must not exceed 500 characters.")), Is.True);
    }

    /// <summary>Verifies that a null note passes (optional field).</summary>
    [Test]
    public async Task Validate_NullNote_Passes()
    {
        var result = await _validator.ValidateAsync(new UpdateComplaintStatusDto { StatusId = 1, Note = null });
        Assert.That(result.IsValid, Is.True);
    }
}
