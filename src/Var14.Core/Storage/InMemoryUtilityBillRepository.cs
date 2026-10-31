using System;
using System.Collections.Generic;
using System.Linq;
using Var14.Core.Abstractions;
using Var14.Core.Domain;

namespace Var14.Core.Storage;

public sealed class InMemoryUtilityBillRepository : IUtilityBillRepository
{
    private readonly Dictionary<int, UtilityBill> _bills = new();

    public void Add(UtilityBill bill)
    {
        ArgumentNullException.ThrowIfNull(bill);
        if (!_bills.TryAdd(bill.Id, bill))
            throw new InvalidOperationException("Документ з таким Id вже існує");
    }

    public UtilityBill? GetById(int id) => _bills.GetValueOrDefault(id);

    public IReadOnlyList<UtilityBill> GetAll() => _bills.Values.ToList();
}