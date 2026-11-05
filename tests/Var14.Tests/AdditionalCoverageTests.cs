using System;
using System.IO;
using Var14.Core;
using Var14.Core.Domain;
using Var14.Core.Errors;
using Var14.Core.Pricing;
using Var14.Core.Storage;
using Xunit;

namespace Var14.Tests;

public class AdditionalCoverageTests
{
    // ---------- Subscriber ----------

    [Fact]
    public void Subscriber_ValidData_CreatesSubscriber()
    {
        // Arrange & Act
        var subscriber = new Subscriber("SUB-1", "Іван Іваненко", "ivan@example.com");

        // Assert
        Assert.Equal("SUB-1", subscriber.Id);
        Assert.Equal("Іван Іваненко", subscriber.FullName);
        Assert.Equal("ivan@example.com", subscriber.Email);
    }

    [Theory]
    [InlineData("", "Name", "a@b.c")]
    [InlineData("id", "", "a@b.c")]
    [InlineData("id", "Name", "")]
    [InlineData("id", "Name", "no-at-sign")]
    public void Subscriber_InvalidData_ThrowsArgumentException(
        string id, string name, string email)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Subscriber(id, name, email));
    }

    // ---------- BillLine ----------

    [Fact]
    public void BillLine_ZeroVolume_ThrowsOutOfRange()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BillLine("Вода", 0m, 10m));
    }

    [Fact]
    public void BillLine_NegativeTariff_ThrowsOutOfRange()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BillLine("Вода", 1m, -1m));
    }

    [Fact]
    public void BillLine_EmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new BillLine("", 1m, 1m));
    }

    // ---------- UtilityBill ----------

    [Fact]
    public void UtilityBill_PayWhenIssued_ChangesStatusToPaid()
    {
        // Arrange
        var bill = new UtilityBill(1, "SUB-1", new DateOnly(2026, 1, 1));
        bill.AddLine(new BillLine("Вода", 1m, 10m));
        bill.Issue();

        // Act
        bill.Pay();

        // Assert
        Assert.Equal(UtilityBillStatus.Paid, bill.Status);
    }

    [Fact]
    public void UtilityBill_PayWhenDraft_ThrowsDomainRuleException()
    {
        // Arrange
        var bill = new UtilityBill(1, "SUB-1", new DateOnly(2026, 1, 1));

        // Act & Assert
        var ex = Assert.Throws<DomainRuleException>(() => bill.Pay());
        Assert.Equal("bill.pay", ex.Rule);
    }

    // ---------- InMemoryUtilityBillRepository ----------

    [Fact]
    public void InMemoryRepo_AddAndGetById_ReturnsStoredBill()
    {
        // Arrange
        var repo = new InMemoryUtilityBillRepository();
        var bill = new UtilityBill(1, "SUB-1", new DateOnly(2026, 1, 1));

        // Act
        repo.Add(bill);
        var actual = repo.GetById(1);

        // Assert
        Assert.Same(bill, actual);
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void InMemoryRepo_DuplicateId_ThrowsInvalidOperation()
    {
        // Arrange
        var repo = new InMemoryUtilityBillRepository();
        var bill = new UtilityBill(1, "SUB-1", new DateOnly(2026, 1, 1));
        repo.Add(bill);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => repo.Add(bill));
    }

    // ---------- BillStatusManager ----------

    [Theory]
    [InlineData("New", 100, "Paid")]
    [InlineData("Paid", 100, "Shipped")]
    [InlineData("Cancelled", 100, "Cancelled")]
    [InlineData("Unknown", 100, "Unknown")]
    public void BillStatusManager_UpdateTransitions_ReturnExpectedStatus(
        string startStatus, double total, string expectedStatus)
    {
        // Act
        decimal actualTotal = BillStatusManager.Update(
            startStatus, (decimal)total, out string actualStatus);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
        if (startStatus == "Cancelled")
        {
            Assert.Equal(0m, actualTotal);
        }
    }

    // ---------- Pricing ----------

    [Theory]
    [InlineData(-0.01)]
    [InlineData(0.6)]
    public void DiscountPricingPolicy_InvalidRate_Throws(double rate)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DiscountPricingPolicy((decimal)rate));
    }

    [Fact]
    public void StandardPricingPolicy_ReturnsLineAmount()
    {
        // Arrange
        var policy = new StandardPricingPolicy();
        var line = new BillLine("Вода", 3m, 10m);

        // Act
        decimal actual = policy.Calculate(line);

        // Assert
        Assert.Equal(30m, actual);
    }

    // ---------- Result<T> ----------

    [Fact]
    public void Result_OkAndFail_ExposeValues()
    {
        // Act
        var ok = Result<int>.Ok(42);
        var fail = Result<int>.Fail("boom");

        // Assert
        Assert.True(ok.IsSuccess);
        Assert.Equal(42, ok.Value);
        Assert.False(fail.IsSuccess);
        Assert.Equal("boom", fail.Error);
    }

    // ---------- ArchivedUtilityBill ----------

    [Fact]
    public void ArchivedUtilityBill_ExposesWrappedProperties()
    {
        // Arrange
        var original = new UtilityBill(5, "SUB-5", new DateOnly(2026, 1, 1));
        var archivedAt = new DateOnly(2026, 6, 1);

        // Act
        var archived = new ArchivedUtilityBill(original, archivedAt);

        // Assert
        Assert.Equal(5, archived.Id);
        Assert.Equal(UtilityBillStatus.Draft, archived.Status);
        Assert.Equal(archivedAt, archived.ArchivedAt);
    }

    // ---------- DomainRuleException ----------

    [Fact]
    public void DomainRuleException_WithInner_KeepsRule()
    {
        // Arrange
        var inner = new InvalidOperationException("boom");

        // Act
        var ex = new DomainRuleException("rule.x", "message", inner);

        // Assert
        Assert.Equal("rule.x", ex.Rule);
        Assert.Same(inner, ex.InnerException);
    }
}