using System;
using Var14.Core.Abstractions;
using Var14.Core.Domain;

namespace Var14.Core.Services;

public sealed class UtilityBillService
{
    private readonly IUtilityBillRepository _repository;
    private readonly IPricingPolicy _pricingPolicy;

    public UtilityBillService(IUtilityBillRepository repository, IPricingPolicy pricingPolicy)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricingPolicy = pricingPolicy ?? throw new ArgumentNullException(nameof(pricingPolicy));
    }

    public void RegisterBill(UtilityBill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        _repository.Add(bill);
    }

    public decimal CalculateTotal(UtilityBill bill)
    {
        decimal total = 0m;
        foreach (var line in bill.Lines)
        {
            total += _pricingPolicy.Calculate(line);
        }
        return total;
    }
}
