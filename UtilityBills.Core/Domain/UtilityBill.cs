using System;
using System.Collections.Generic;

namespace UtilityBills.Core.Domain;

/// <summary>Рахунок за комунальні послуги абонента.</summary>
public sealed class UtilityBill
{
    private readonly List<BillLine> _lines = new();

    public int Id { get; init; }

    public int CustomerId { get; init; }

    public BillStatus Status { get; set; } = BillStatus.Draft;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<BillLine> Lines => _lines;

    /// <summary>Додає рядок до рахунку.</summary>
    public void AddLine(BillLine line) => _lines.Add(line);

    /// <summary>Обчислює загальну суму рахунку.</summary>
    /// <returns></returns>
    public decimal Total()
    {
        decimal sum = 0m;
        foreach (BillLine line in _lines)
        {
            sum += line.Amount;
        }

        return sum;
    }
}
