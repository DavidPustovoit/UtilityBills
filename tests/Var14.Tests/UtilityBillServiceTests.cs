using System;
using Microsoft.Extensions.Logging.Abstractions;
using Var14.Core.Domain;
using Var14.Core.Services;
using Xunit;

namespace Var14.Tests;

public class UtilityBillServiceTests
{
    [Fact]
    public void CalculateTotal_TwoLines_SumsPricesFromPolicy()
    {
        // Arrange
        var repository = new FakeUtilityBillRepository();
        var bill = new UtilityBill(7, "SUB-1", new DateOnly(2026, 1, 1));
        bill.AddLine(new BillLine("Вода", 10, 30m));
        bill.AddLine(new BillLine("Газ", 5, 8m));

        var service = new UtilityBillService(
            repository,
            new StubPricingPolicy(25m),
            NullLogger<UtilityBillService>.Instance);

        // Act
        decimal actual = service.CalculateTotal(bill);

        // Assert
        Assert.Equal(50m, actual);
    }

    [Fact]
    public void RegisterBill_ValidBill_StoresAndSavesOnce()
    {
        // Arrange
        var repository = new FakeUtilityBillRepository();
        var service = new UtilityBillService(
            repository,
            new StubPricingPolicy(1m),
            NullLogger<UtilityBillService>.Instance);
        var bill = new UtilityBill(1, "SUB-1", new DateOnly(2026, 1, 1));
        bill.AddLine(new BillLine("Вода", 1, 5m));

        // Act
        service.RegisterBill(bill);

        // Assert
        Assert.NotNull(repository.GetById(1));
        Assert.Equal(1, repository.AddCalls);
    }

    [Fact]
    public void LoadBill_UnknownId_ReturnsNull()
    {
        // Arrange
        var repository = new FakeUtilityBillRepository();
        var service = new UtilityBillService(
            repository,
            new StubPricingPolicy(1m),
            NullLogger<UtilityBillService>.Instance);

        // Act
        var actual = service.LoadBill(999);

        // Assert
        Assert.Null(actual);
    }
}
