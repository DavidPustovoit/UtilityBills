using System;
using System.Collections.Generic;
using Var14.Core.Legacy;
using Xunit;
using Var14.Core;

namespace Var14.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<Indicator> indicators = new()
        {
            new Indicator { TariffCode = "A1", Volume = 12, Tariff = 100m },
            new Indicator { TariffCode = "B2", Volume = 1, Tariff = 250m }
        };

        decimal total = BillProcessor.Process(
            1, "Іваненко", "i@ex.com", true, indicators,
            new DateOnly(2026, 3, 10), "", "New", false,
            60m, out string next);

        Assert.Equal(1292.70m, total);
        Assert.Equal("Paid", next);
    }

    [Fact]
    public void Process_SmallOrder_AddsDelivery()
    {
        List<Indicator> indicators = new()
        {
            new Indicator { TariffCode = "A1", Volume = 2, Tariff = 200m }
        };

        decimal total = BillProcessor.Process(
            2, "Петренко", "p@ex.com", false, indicators,
            new DateOnly(2026, 3, 10), "SALE10", "Paid",
            false, 60m, out string next);

        Assert.Equal(420m, total);
        Assert.Equal("Shipped", next);
    }

    [Fact]
    public void Process_NoLines_ReturnsMinusOne()
    {
        decimal total = BillProcessor.Process(
            3, "Коваль", "k@ex.com", false,
            new List<Indicator>(),
            new DateOnly(2026, 3, 10), "", "New", false,
            60m, out string next);

        Assert.Equal(-1m, total);
    }
}