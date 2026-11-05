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
    private const decimal RegularClientDiscount = 0.93m;
    private const decimal WeekendDiscount = 0.98m;
    private const int MinVolumeForDiscount = 10;
    private const decimal VolumeDiscount = 0.95m;

    /// <summary>
    /// Округлює суму вгору до копійки (0.01 грн) відповідно до вимоги варіанта 14.
    /// </summary>
    private static decimal RoundUpToKopecks(decimal total) => Math.Ceiling(total * 100m) / 100m;

    public static decimal Calculate(List<Indicator> indicators, bool isRegular, string couponCode, DateOnly createdAt, decimal serviceFee)
    {
        ArgumentNullException.ThrowIfNull(indicators);

        decimal total = CalculateBaseSum(indicators);

        total = ApplyRegularClientDiscount(total, isRegular);

        total = ApplyVolumeDiscounts(total);

        total = ApplyCoupons(total, couponCode);

        total = ApplyWeekendDiscount(total, createdAt);

        total = ApplyServiceFee(total, serviceFee);

        return total < 0 ? 0 : RoundUpToKopecks(total);
    }

    private static decimal CalculateBaseSum(List<Indicator> indicators)
    {
        decimal sum = 0;
        foreach (var indicator in indicators)
        {
            decimal itemSum = indicator.Volume * indicator.Tariff;
            if (indicator.Volume >= MinVolumeForDiscount)
            {
                itemSum *= VolumeDiscount;
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
        return isRegular ? total * RegularClientDiscount : total;
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
        return isSunday ? total * WeekendDiscount : total;
    }

    private static decimal ApplyServiceFee(decimal total, decimal serviceFee)
    {
        return total < 1000m ? total + serviceFee : total;
    }
}
