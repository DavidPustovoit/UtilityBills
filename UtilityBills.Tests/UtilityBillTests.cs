using System;
using UtilityBills.Core.Domain;
using Xunit;

namespace UtilityBills.Tests;

public class UtilityBillTests
{
    [Fact]
    public void CalculateTotalNoSubsidiesReturnsCorrectAmountWithVat()
    {
        var bill = new UtilityBill("1", "Test");
        bill.AddLine("WATER", 10, 20m); // 200

        var total = bill.CalculateTotal(false);

        // 200 + 20% VAT = 240
        Assert.Equal(240m, total);
    }

    [Fact]
    public void CalculateTotalWithBaseSubsidyAppliesDiscount()
    {
        var bill = new UtilityBill("2", "Test");
        bill.AddLine("GAS", 100, 15m); // 1500 (більше 1000)

        var total = bill.CalculateTotal(true);

        // 1500 - 10% = 1350. + 20% VAT = 1620
        Assert.Equal(1620m, total);
    }

    [Fact]
    public void CalculateTotalLargeConsumptionAppliesLargeDiscount()
    {
        var bill = new UtilityBill("3", "Test");
        bill.AddLine("HEAT", 100, 60m); // 6000 (більше 5000)

        var total = bill.CalculateTotal(false); // Субсидія false, але знижка за обсяг працює

        // 6000 - 15% = 5100. + 20% VAT = 6120
        Assert.Equal(6120m, total);
    }

    [Fact]
    public void CalculateTotalMoreThan10LinesAppliesBulkDiscount()
    {
        var bill = new UtilityBill("4", "Test");
        for (int i = 0; i < 11; i++)
        {
            bill.AddLine("SRV", 1, 10m);
        }
        // Підсума = 110. Знижка = -100. Всього = 10. + 20% VAT = 12
        var total = bill.CalculateTotal(false);

        Assert.Equal(12m, total);
    }

    [Fact]
    public void IsValidValidBillReturnsTrue()
    {
        var bill = new UtilityBill("5", "ValidUser");
        bill.AddLine("WATER", 1, 1m);
        Assert.True(bill.IsValid());
    }

    [Fact]
    public void IsValidEmptyNameReturnsFalse()
    {
        var bill = new UtilityBill("6", "A"); // Довжина < 2
        bill.AddLine("WATER", 1, 1m);
        Assert.False(bill.IsValid());
    }

    [Fact]
    public void BuildReportGoldenMasterReturnsExactFormat()
    {
        var bill = new UtilityBill("7", "Ivan");
        bill.AddLine("WATER", 3, 250m);
        bill.AddLine("ELEC", 12, 90m);

        var actual = bill.BuildReport();

        var expected = "Послуга: WATER; обсяг: 3; тариф: 250; сума: 750\n" +
                       "Послуга: ELEC; обсяг: 12; тариф: 90; сума: 1080\n" +
                       "Разом: 2196.0\n";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryChangeStatusValidTransitionChangesStatus()
    {
        var bill = new UtilityBill("8", "Ivan");
        Assert.True(bill.TryChangeStatus(1)); // 1 - Paid

        // Порівнюємо статус із конкретним значенням перелічення BillStatus.Paid
        Assert.Equal(BillStatus.Paid, bill.Status);
    }
}
