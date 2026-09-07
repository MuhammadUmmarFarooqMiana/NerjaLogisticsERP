using NerjaLogisticsERP.Application.Advances.Commands.CreateAdvance;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Advances;

public class CreateAdvanceCommandValidatorTests
{
    private readonly CreateAdvanceCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenEmployeeIdIsEmpty()
        => _validator.Validate(new CreateAdvanceCommand { EmployeeId = Guid.Empty, Amount = 100m })
            .ShouldHaveErrorFor(nameof(CreateAdvanceCommand.EmployeeId));

    [TestCase(0)]
    [TestCase(-1)]
    public void ShouldHaveError_WhenAmountIsNotPositive(decimal amount)
        => _validator.Validate(new CreateAdvanceCommand { EmployeeId = Guid.NewGuid(), Amount = amount })
            .ShouldHaveErrorFor(nameof(CreateAdvanceCommand.Amount));

    [Test]
    public void ShouldNotHaveErrors_ForAValidAdvance()
        => _validator.Validate(new CreateAdvanceCommand { EmployeeId = Guid.NewGuid(), Amount = 100m })
            .IsValid.ShouldBeTrue();
}
