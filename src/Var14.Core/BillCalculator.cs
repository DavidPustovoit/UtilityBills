using System;
using System.Collections.Generic;
using Var14.Core.Legacy;

namespace Var14.Core;

public static class BillCalculator
{
    public static decimal Calculate(List<Indicator> indicators, bool isRegular, string couponCode, DateOnly createdAt, decimal serviceFee)
    {
        decimal total = 0;
        foreach (var ind in indicators)
        {
            decimal sum = ind.Volume * ind.Tariff;
            if (ind.Volume >= 10) sum *= 0.95m;
            total += sum;
        }

        if (isRegular) total *= 0.93m;

        if (total > 5000) total *= 0.9m;
        else if (total > 2000) total *= 0.95m;

        if (couponCode == "SALE10") total -= total * 0.1m;
        else if (couponCode == "MINUS200" && total > 1000) total -= 200m;

        if (createdAt.DayOfWeek == DayOfWeek.Sunday) total *= 0.98m;

        if (total < 1000) total += serviceFee;
        if (total < 0) total = 0;

        return Math.Round(total, 2);
    }
}