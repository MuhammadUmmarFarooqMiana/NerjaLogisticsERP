using NerjaLogisticsERP.Application.Fines.Commands.CreateFine;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Fines;

public class CreateFineCommandValidatorTests
{
    private readonly CreateFineCommandValidator _validator = new();

    private static CreateFineCommand Valid() => new()
    {
        EmployeeId = Guid.NewGuid(),
        Amount = 50m,
        Reason = "Late for shift.",
        FineDate = DateOnly.FromDateTime(DateTime.UtcNow)
    };

    [Test]
    public void ShouldHaveError_WhenEmployeeIdIsEmpty()
        => _validator.Validate(Valid() with { EmployeeId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(CreateFineCommand.EmployeeId));

    [TestCase(0)]
    [TestCase(-1)]
    public void ShouldHaveError_WhenAmountIsNotPositive(decimal amount)
        => _validator.Validate(Valid() with { Amount = amount })
            .ShouldHaveErrorFor(nameof(CreateFineCommand.Amount));

    [Test]
    public void ShouldHaveError_WhenReasonIsEmpty()
        => _validator.Validate(Valid() with { Reason = "" })
            .ShouldHaveErrorFor(nameof(CreateFineCommand.Reason));

    [Test]
    public void ShouldHaveError_WhenReasonExceedsMaxLength()
        => _validator.Validate(Valid() with { Reason = new string('x', 256) })
            .ShouldHaveErrorFor(nameof(CreateFineCommand.Reason));

    [Test]
    public void ShouldHaveError_WhenFineDateIsInTheFuture()
        => _validator.Validate(Valid() with { FineDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1) })
            .ShouldHaveErrorFor(nameof(CreateFineCommand.FineDate));

    [Test]
    public void ShouldNotHaveErrors_ForAValidFine()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
