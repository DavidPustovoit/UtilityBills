using System;
using Var14.Core.Domain;
using Var14.Core.Pricing;
using Var14.Core.Services;
using Var14.Core.Storage;
using Xunit;

namespace Var14.Tests;

public class DomainInvariantTests
{
    // Тест 1: Порушення інваріанта позиції
    [Fact]
    public void BillLineWithZeroVolumeThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BillLine("Електроенергія", 0, 4.32m));
    }

    // Тест 2: Заборонений перехід стану документа
    [Fact]
    public void UtilityBillAfterIssueRejectsNewLines()
    {
        // Arrange
        var bill = new UtilityBill(1, "SUB-123", DateOnly.FromDateTime(DateTime.Today));
        bill.AddLine(new BillLine("Вода", 10, 30m));
        bill.Issue(); // Змінюємо статус на Issued

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => bill.AddLine(new BillLine("Газ", 5, 8m)));
    }

    // Тест 3: Розрахунок суми з підставленою політикою (без доступу до файлів)
    [Fact]
    public void CalculateTotalWithTenPercentDiscountReturnsCorrectSum()
    {
        // Arrange
        var service = new UtilityBillService(
            new InMemoryUtilityBillRepository(),
            new DiscountPricingPolicy(0.10m)); // Знижка 10%

        var bill = new UtilityBill(2, "SUB-456", DateOnly.FromDateTime(DateTime.Today));
        bill.AddLine(new BillLine("Електроенергія", 200, 4m)); // 200 * 4 = 800 грн

        // Act
        decimal total = service.CalculateTotal(bill);

        // Assert
        // Очікуємо 800 - 10% = 720 грн
        Assert.Equal(720m, total);
    }
}
