using System;
using System.Collections.Generic;
using Var14.Core;
using Var14.Core.Legacy;
using Xunit;

namespace Var14.Tests;

public class BillCalculatorTests
{
    private static readonly DateOnly Monday = new(2023, 10, 2);
    private static readonly DateOnly Sunday = new(2023, 10, 1);

    [Fact]
    public void Calculate_VolumeBelowTen_NoVolumeDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 9, Tariff = 100m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 0m);

        // Assert
        Assert.Equal(900m, actual);
    }

    [Fact]
    public void Calculate_VolumeAtTen_AppliesVolumeDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 10, Tariff = 100m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 0m);

        // Assert
        Assert.Equal(950m, actual);
    }

    [Fact]
    public void Calculate_TotalAtStandardThreshold_NoThresholdDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 2000m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 0m);

        // Assert
        Assert.Equal(2000m, actual);
    }

    [Fact]
    public void Calculate_TotalJustAboveStandard_AppliesStandardDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 2001m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 0m);

        // Assert
        Assert.Equal(1900.95m, actual);
    }

    [Fact]
    public void Calculate_TotalAtPremiumThreshold_AppliesStandardDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 5000m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 0m);

        // Assert
        Assert.Equal(4750m, actual);
    }

    [Fact]
    public void Calculate_TotalJustAbovePremium_AppliesPremiumDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 5001m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 0m);

        // Assert
        Assert.Equal(4500.90m, actual);
    }

    [Fact]
    public void Calculate_CouponSale10_AppliesTenPercentDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 1500m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "SALE10", Monday, 0m);

        // Assert
        Assert.Equal(1350m, actual);
    }

    [Fact]
    public void Calculate_CouponMinus200AboveThreshold_Subtracts200()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 1500m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "MINUS200", Monday, 0m);

        // Assert
        Assert.Equal(1300m, actual);
    }

    [Fact]
    public void Calculate_CouponMinus200BelowThreshold_DoesNothing()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 500m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "MINUS200", Monday, 0m);

        // Assert
        Assert.Equal(500m, actual);
    }

    [Fact]
    public void Calculate_RegularClient_AppliesRegularDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 1000m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, true, "", Monday, 0m);

        // Assert
        Assert.Equal(930m, actual);
    }

    [Fact]
    public void Calculate_Sunday_AppliesWeekendDiscount()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 1000m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Sunday, 0m);

        // Assert
        Assert.Equal(980m, actual);
    }

    [Fact]
    public void Calculate_SmallTotal_AddsServiceFee()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 500m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 50m);

        // Assert
        Assert.Equal(550m, actual);
    }

    [Fact]
    public void Calculate_EmptyIndicators_ReturnsZero()
    {
        // Arrange
        var indicators = new List<Indicator>();

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 0m);

        // Assert
        Assert.Equal(0m, actual);
    }
}