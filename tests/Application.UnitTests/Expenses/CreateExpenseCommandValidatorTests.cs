using NerjaLogisticsERP.Application.Expenses.Commands.CreateExpense;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Expenses;

public class CreateExpenseCommandValidatorTests
{
    private readonly CreateExpenseCommandValidator _validator = new();

    [TestCase(0)]
    [TestCase(-1)]
    public void ShouldHaveError_WhenAmountIsNotPositive(decimal amount)
        => _validator.Validate(new CreateExpenseCommand { Amount = amount, Category = ExpenseCategory.Fuel })
            .ShouldHaveErrorFor(nameof(CreateExpenseCommand.Amount));

    [Test]
    public void ShouldHaveError_WhenCategoryIsNotAValidEnumValue()
        => _validator.Validate(new CreateExpenseCommand { Amount = 100m, Category = (ExpenseCategory)999 })
            .ShouldHaveErrorFor(nameof(CreateExpenseCommand.Category));

    [Test]
    public void ShouldNotHaveErrors_ForAValidExpense()
        => _validator.Validate(new CreateExpenseCommand { Amount = 100m, Category = ExpenseCategory.Fuel })
            .IsValid.ShouldBeTrue();
}
