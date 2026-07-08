using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="AddCommentDtoValidator"/> class.
/// </summary>
[TestFixture]
public class AddCommentDtoValidatorTests
{
    private AddCommentDtoValidator _validator;

    /// <summary>Sets up the validator under test.</summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new AddCommentDtoValidator();
    }

    /// <summary>Verifies that a valid message passes.</summary>
    [Test]
    public async Task Validate_ValidMessage_Passes()
    {
        var result = await _validator.ValidateAsync(new AddCommentDto { Message = "Please check the pipes." });
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>Verifies that an empty message fails.</summary>
    [Test]
    public async Task Validate_EmptyMessage_Fails()
    {
        var result = await _validator.ValidateAsync(new AddCommentDto { Message = "" });
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Message is required.")), Is.True);
    }

    /// <summary>Verifies that a message exceeding 2000 chars fails.</summary>
    [Test]
    public async Task Validate_MessageTooLong_Fails()
    {
        var result = await _validator.ValidateAsync(new AddCommentDto { Message = new string('A', 2001) });
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Message must not exceed 2000 characters.")), Is.True);
    }

    /// <summary>Verifies that a single-character message passes (min length = 1).</summary>
    [Test]
    public async Task Validate_SingleCharMessage_Passes()
    {
        var result = await _validator.ValidateAsync(new AddCommentDto { Message = "X" });
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>Verifies that a 2000-character message passes (boundary).</summary>
    [Test]
    public async Task Validate_2000CharMessage_Passes()
    {
        var result = await _validator.ValidateAsync(new AddCommentDto { Message = new string('A', 2000) });
        Assert.That(result.IsValid, Is.True);
    }
}
