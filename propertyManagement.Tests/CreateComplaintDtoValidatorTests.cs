using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="CreateComplaintDtoValidator"/> class.
/// </summary>
[TestFixture]
public class CreateComplaintDtoValidatorTests
{
    private CreateComplaintDtoValidator _validator;

    /// <summary>Sets up the validator under test.</summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new CreateComplaintDtoValidator();
    }

    private static CreateComplaintDto ValidDto() => new()
    {
        LeaseId = Guid.NewGuid(),
        CategoryId = 1,
        PriorityId = 2,
        Subject = "Leaking tap in kitchen",
        Description = "The tap has been leaking for two days and is causing water damage."
    };

    /// <summary>Verifies that a valid DTO passes.</summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        var result = await _validator.ValidateAsync(ValidDto());
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>Verifies that an empty LeaseId fails.</summary>
    [Test]
    public async Task Validate_EmptyLeaseId_Fails()
    {
        var dto = ValidDto();
        dto.LeaseId = Guid.Empty;

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Lease ID is required.")), Is.True);
    }

    /// <summary>Verifies that CategoryId = 0 fails.</summary>
    [Test]
    public async Task Validate_CategoryIdBelowRange_Fails()
    {
        var dto = ValidDto();
        dto.CategoryId = 0;

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Category must be between 1 and 8.")), Is.True);
    }

    /// <summary>Verifies that CategoryId = 9 fails.</summary>
    [Test]
    public async Task Validate_CategoryIdAboveRange_Fails()
    {
        var dto = ValidDto();
        dto.CategoryId = 9;

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
    }

    /// <summary>Verifies that PriorityId = 5 fails.</summary>
    [Test]
    public async Task Validate_PriorityIdAboveRange_Fails()
    {
        var dto = ValidDto();
        dto.PriorityId = 5;

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Priority must be between 1 and 4.")), Is.True);
    }

    /// <summary>Verifies that a Subject shorter than 5 chars fails.</summary>
    [Test]
    public async Task Validate_SubjectTooShort_Fails()
    {
        var dto = ValidDto();
        dto.Subject = "Tap";

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Subject must be at least 5 characters.")), Is.True);
    }

    /// <summary>Verifies that a Subject longer than 150 chars fails.</summary>
    [Test]
    public async Task Validate_SubjectTooLong_Fails()
    {
        var dto = ValidDto();
        dto.Subject = new string('A', 151);

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Subject must not exceed 150 characters.")), Is.True);
    }

    /// <summary>Verifies that a Description shorter than 10 chars fails.</summary>
    [Test]
    public async Task Validate_DescriptionTooShort_Fails()
    {
        var dto = ValidDto();
        dto.Description = "Short";

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Description must be at least 10 characters.")), Is.True);
    }

    /// <summary>Verifies that a Description longer than 2000 chars fails.</summary>
    [Test]
    public async Task Validate_DescriptionTooLong_Fails()
    {
        var dto = ValidDto();
        dto.Description = new string('A', 2001);

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
    }

    /// <summary>Verifies that a relative AttachmentUrl fails.</summary>
    [Test]
    public async Task Validate_RelativeAttachmentUrl_Fails()
    {
        var dto = ValidDto();
        dto.AttachmentUrl = "/uploads/file.pdf";

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Attachment URL must be a valid absolute URL.")), Is.True);
    }

    /// <summary>Verifies that a null AttachmentUrl passes (optional field).</summary>
    [Test]
    public async Task Validate_NullAttachmentUrl_Passes()
    {
        var dto = ValidDto();
        dto.AttachmentUrl = null;

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>Verifies that a valid absolute URL passes.</summary>
    [Test]
    public async Task Validate_ValidAbsoluteAttachmentUrl_Passes()
    {
        var dto = ValidDto();
        dto.AttachmentUrl = "http://localhost:5104/uploads/complaintdocs/file.pdf";

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.True);
    }
}
