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
	
	// АНТИПАТЕРН 1: порожній catch, дефект зникає (проковтнутий виняток)
    public UtilityBill? LoadBill(int billId)
    {
        try
        {
            return _repository.GetById(billId);
        }
        catch (Exception)
        {
        }
        return null;
    }

    // АНТИПАТЕРНИ 2 і 3: широке перехоплення (catch Exception) і втрата стека (throw ex)
    public void CancelBill(int billId)
    {
        try
        {
            var bill = _repository.GetById(billId);
            // Наприклад тут якась логіка скасування, яка працює з БД
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
            throw ex;
        }
    }

    // АНТИПАТЕРН 4: try/catch замість звичайної умови (винятки як керування потоком)
    public int CountValidVolumes(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            try
            {
                var q = decimal.Parse(row);
                if (q > 0) valid++;
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }
        return valid;
    }
}
