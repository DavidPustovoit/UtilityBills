using System;
using System.Collections.Generic;
using Var14.Core.Legacy;

namespace Var14.Core;

public static class BillCalculator
{
    private const decimal PremiumThreshold = 5000m;
    private const decimal PremiumDiscount = 0.9m;
    private const decimal StandardThreshold = 2000m;
    private const decimal StandardDiscount = 0.95m;

    public static decimal Calculate(List<Indicator> indicators, bool isRegular, string couponCode, DateOnly createdAt, decimal serviceFee)
    {
        ArgumentNullException.ThrowIfNull(indicators);

        decimal total = CalculateBaseSum(indicators);

        total = ApplyRegularClientDiscount(total, isRegular);

        total = ApplyVolumeDiscounts(total);

        total = ApplyCoupons(total, couponCode);

        total = ApplyWeekendDiscount(total, createdAt);

        if (total < 1000)
        {
            total += serviceFee;
        }

        if (total < 0)
        {
            total = 0;
        }

        return Math.Round(total, 2);
    }

    private static decimal CalculateBaseSum(List<Indicator> indicators)
    {
        decimal sum = 0;
        foreach (var indicator in indicators)
        {
            decimal itemSum = indicator.Volume * indicator.Tariff;
            if (indicator.Volume >= 10)
            {
                itemSum *= 0.95m;
            }

            sum += itemSum;
        }
        return sum;
    }

    private static decimal ApplyVolumeDiscounts(decimal currentTotal)
    {
        if (currentTotal > PremiumThreshold)
        {
            return currentTotal * PremiumDiscount;
        }
        if (currentTotal > StandardThreshold)
        {
            return currentTotal * StandardDiscount;
        }
        return currentTotal;
    }

    private static decimal ApplyRegularClientDiscount(decimal total, bool isRegular)
    {
        return isRegular ? total * 0.93m : total;
    }

    private static decimal ApplyCoupons(decimal total, string couponCode)
    {
        if (couponCode == "SALE10")
        {
            return total - (total * 0.1m);
        }

        bool isMinus200Eligible = couponCode == "MINUS200" && total > 1000m;
        if (isMinus200Eligible)
        {
            return total - 200m;
        }

        return total;
    }

    private static decimal ApplyWeekendDiscount(decimal total, DateOnly createdAt)
    {
        bool isSunday = createdAt.DayOfWeek == DayOfWeek.Sunday;
        return isSunday ? total * 0.98m : total;
    }
}
