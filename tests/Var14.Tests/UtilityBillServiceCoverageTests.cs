using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using Var14.Core.Abstractions;
using Var14.Core.Domain;
using Var14.Core.Errors;
using Var14.Core.Services;
using Xunit;

namespace Var14.Tests;

public class UtilityBillServiceCoverageTests
{
    private static UtilityBillService BuildService(
        IUtilityBillRepository? repo = null, decimal price = 10m)
        => new(
            repo ?? new FakeUtilityBillRepository(),
            new StubPricingPolicy(price),
            NullLogger<UtilityBillService>.Instance);

    [Fact]
    public void RegisterBill_DuplicateId_LogsAndRethrows()
    {
        // Arrange
        var repository = new FakeUtilityBillRepository();
        var service = BuildService(repository);
        var bill = new UtilityBill(1, "SUB-1", new DateOnly(2026, 1, 1));
        bill.AddLine(new BillLine("Вода", 1m, 5m));
        repository.Add(bill);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.RegisterBill(bill));
    }

    [Fact]
    public void CalculateTotal_AmountAbove5000_LogsWarningBranch()
    {
        // Arrange
        var bill = new UtilityBill(2, "SUB-2", new DateOnly(2026, 1, 1));
        bill.AddLine(new BillLine("Газ", 1m, 1m));
        var service = BuildService(price: 6000m);

        // Act
        decimal total = service.CalculateTotal(bill);

        // Assert
        Assert.Equal(6000m, total);
    }

    [Fact]
    public void CancelBill_UnknownId_ThrowsDomainRuleException()
    {
        // Arrange
        var service = BuildService();

        // Act & Assert
        var ex = Assert.Throws<DomainRuleException>(() => service.CancelBill(999));
        Assert.Equal("bill.exists", ex.Rule);
    }

    [Fact]
    public void CancelBill_RepositoryThrowsInvalidOperation_Rethrows()
    {
        // Arrange
        var service = BuildService(new ThrowingRepository());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.CancelBill(1));
    }

    [Theory]
    [InlineData(new[] { "1", "abc", "5", "-1", "10" }, 3)]
    [InlineData(new string[0], 0)]
    [InlineData(new[] { "0", "-5", "abc" }, 0)]
    public void CountValidVolumes_MixedInput_ReturnsCountOfPositives(
        string[] rows, int expected)
    {
        // Act
        int actual = UtilityBillService.CountValidVolumes(rows);

        // Assert
        Assert.Equal(expected, actual);
    }
}

/// <summary>
/// Стаб-заглушка, яка кидає виняток при GetById.
/// </summary>
internal sealed class ThrowingRepository : IUtilityBillRepository
{
    public void Add(UtilityBill bill) { }

    public UtilityBill? GetById(int id) =>
        throw new InvalidOperationException("repo failure");

    public IReadOnlyList<UtilityBill> GetAll() => Array.Empty<UtilityBill>();
}
