using System;
using System.Collections.Generic;
using Var14.Core;
using Var14.Core.Legacy;
using Xunit;

namespace Var14.Tests;

public class BillCalculatorTests
{
    private readonly DateOnly _monday = new(2023, 10, 2);
    private readonly DateOnly _sunday = new(2023, 10, 1);

    [Theory]
    [InlineData(1, 1000, 1000)] // Без знижки за обсяг
    [InlineData(10, 100, 950)]  // Знижка за обсяг >= 10
    public void CalculateVolumeReturnsCorrectBaseSum(int vol, decimal tariff, decimal expected)
    {
        var inds = new List<Indicator> { new() { Volume = vol, Tariff = tariff } };
        var actual = BillCalculator.Calculate(inds, false, "", _monday, 0);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(6000, 5400)] // > 5000 (*0.9)
    [InlineData(3000, 2850)] // > 2000 (*0.95)
    [InlineData(1500, 1500)] // < 2000 (без знижки)
    public void CalculateThresholdsAppliesCorrectDiscount(decimal tariff, decimal expected)
    {
        var inds = new List<Indicator> { new() { Volume = 1, Tariff = tariff } };
        var actual = BillCalculator.Calculate(inds, false, "", _monday, 0);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CalculateRegularClientAppliesDiscount()
    {
        var inds = new List<Indicator> { new() { Volume = 1, Tariff = 1500 } };
        var actual = BillCalculator.Calculate(inds, true, "", _monday, 0);
        Assert.Equal(1395m, actual); // 1500 * 0.93
    }

    [Theory]
    [InlineData("SALE10", 1500, 1350)]
    [InlineData("MINUS200", 1500, 1300)]
    [InlineData("MINUS200", 500, 500)] // Не діє < 1000
    public void CalculateCouponsAppliesCorrectly(string coupon, decimal tariff, decimal expected)
    {
        var inds = new List<Indicator> { new() { Volume = 1, Tariff = tariff } };
        var actual = BillCalculator.Calculate(inds, false, coupon, _monday, 0);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CalculateSundayAppliesWeekendDiscount()
    {
        var inds = new List<Indicator> { new() { Volume = 1, Tariff = 1500 } };
        var actual = BillCalculator.Calculate(inds, false, "", _sunday, 0);
        Assert.Equal(1470m, actual); // 1500 * 0.98
    }

    [Fact]
    public void CalculateSmallAmountAddsServiceFee()
    {
        var inds = new List<Indicator> { new() { Volume = 1, Tariff = 500 } };
        var actual = BillCalculator.Calculate(inds, false, "", _monday, 50m);
        Assert.Equal(550m, actual); // 500 + 50
    }
}
