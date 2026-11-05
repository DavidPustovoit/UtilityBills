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

    [Theory]
    [InlineData(9, 100.0, "", 900.0)]           // обсяг < 10 — без знижки
    [InlineData(10, 100.0, "", 950.0)]          // обсяг = 10 — знижка 5%
    [InlineData(1, 2000.0, "", 2000.0)]         // сума = 2000 — без порогової
    [InlineData(1, 2001.0, "", 1900.95)]        // сума > 2000 — 5%
    [InlineData(1, 5000.0, "", 4750.0)]         // сума = 5000 — 5% (не 10%)
    [InlineData(1, 5001.0, "", 4500.90)]        // сума > 5000 — 10%
    [InlineData(1, 1500.0, "SALE10", 1350.0)]   // купон −10%
    [InlineData(1, 1500.0, "MINUS200", 1300.0)] // купон −200 при сумі > 1000
    [InlineData(1, 500.0, "MINUS200", 500.0)]   // купон не діє при сумі < 1000
    public void Calculate_Boundaries_ReturnsExpectedTotal(
        int volume, double tariff, string coupon, double expected)
    {
        // Arrange
        var indicators = new List<Indicator>
        {
            new() { Volume = volume, Tariff = (decimal)tariff }
        };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, coupon, Monday, 0m);

        // Assert
        Assert.Equal((decimal)expected, actual);
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

    [Fact]
    public void Calculate_NullIndicators_ThrowsArgumentNullException()
    {
        // Arrange
        List<Indicator>? indicators = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(
            () => BillCalculator.Calculate(indicators!, false, "", Monday, 0m));
        Assert.Equal("indicators", ex.ParamName);
    }

    [Fact]
    public void Calculate_FractionalTotal_RoundsUpToTwoDecimals()
    {
        // Arrange
        var indicators = new List<Indicator> { new() { Volume = 1, Tariff = 100.001m } };

        // Act
        decimal actual = BillCalculator.Calculate(indicators, false, "", Monday, 0m);

        // Assert
        Assert.Equal(100.01m, actual);
    }
}
