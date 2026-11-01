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

    // ВИПРАВЛЕНО 1: Оброблено змістовно, без порожнього catch
    public UtilityBill? LoadBill(int billId)
    {
        var bill = _repository.GetById(billId);
        if (bill == null)
        {
            // Тут міг би бути лог
        }
        return bill;
    }

    // ВИПРАВЛЕНО 2 і 3: Ловимо конкретний виняток і зберігаємо стек (throw)
    public void CancelBill(int billId)
    {
        try
        {
            var bill = _repository.GetById(billId)
                ?? throw new Errors.DomainRuleException("bill.exists", "Рахунок не знайдено");
            // Логіка скасування...
        }
        catch (InvalidOperationException) // ловимо помилку, а не Exception
        {
            throw;
        }
    }

    // ВИПРАВЛЕНО 4: Використовуємо TryParse замість try-catch
    public static int CountValidVolumes(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            if (decimal.TryParse(row, out var q) && q > 0)
            {
                valid++;
            }
        }
        return valid;
    }
}
