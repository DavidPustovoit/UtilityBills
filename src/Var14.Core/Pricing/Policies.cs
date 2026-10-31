using System;
using Var14.Core.Abstractions;
using Var14.Core.Domain;

namespace Var14.Core.Pricing;

// Стандартний тариф (без знижок)
public sealed class StandardPricingPolicy : IPricingPolicy
{
    public decimal Calculate(BillLine line) => line.Amount;
}

// Пільговий тариф (зі знижкою)
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _discountRate;

    public DiscountPricingPolicy(decimal discountRate)
    {
        if (discountRate is < 0m or > 0.5m)
            throw new ArgumentOutOfRangeException(nameof(discountRate), "Знижка має бути від 0 до 50%");
        _discountRate = discountRate;
    }

    public decimal Calculate(BillLine line) => Math.Round(line.Amount * (1m - _discountRate), 2);
}