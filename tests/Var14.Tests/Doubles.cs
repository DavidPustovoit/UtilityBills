using System;
using System.Collections.Generic;
using System.Linq;
using Var14.Core.Abstractions;
using Var14.Core.Domain;

namespace Var14.Tests;

/// <summary>
/// Фейк сховища: робоча реалізація в пам'яті + лічильник викликів Add.
/// </summary>
internal sealed class FakeUtilityBillRepository : IUtilityBillRepository
{
    private readonly Dictionary<int, UtilityBill> _bills = new();

    public int AddCalls { get; private set; }

    public void Add(UtilityBill bill)
    {
        AddCalls++;
        if (!_bills.TryAdd(bill.Id, bill))
        {
            throw new InvalidOperationException("Документ з таким Id вже існує");
        }
    }

    public UtilityBill? GetById(int id) => _bills.GetValueOrDefault(id);

    public IReadOnlyList<UtilityBill> GetAll() => _bills.Values.ToList();
}

/// <summary>
/// Стаб політики ціноутворення: завжди повертає наперед задану ціну.
/// </summary>
internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price) => _price = price;

    public decimal Calculate(BillLine line) => _price;
}
