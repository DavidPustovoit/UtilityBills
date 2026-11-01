using System;
using System.Collections.Generic;
using Var14.Core.Errors;

namespace Var14.Core.Domain;

public sealed class UtilityBill
{
    // Прихована колекція
    private readonly List<BillLine> _lines = new();

    public UtilityBill(int id, string subscriberId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        if (string.IsNullOrWhiteSpace(subscriberId))
        {
            throw new ArgumentException("Id клієнта порожнє");
        }

        Id = id;
        SubscriberId = subscriberId;
        CreatedAt = createdAt;
        Status = UtilityBillStatus.Draft;
    }

    public int Id { get; }
    public string SubscriberId { get; }
    public DateOnly CreatedAt { get; }

    // Ніхто зовні не змінить статус безпосередньо
    public UtilityBillStatus Status { get; private set; }

    // Клієнт бачить список, але не може викликати на ньому .Add()
    public IReadOnlyList<BillLine> Lines => _lines;

    public void AddLine(BillLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (Status != UtilityBillStatus.Draft)
        {
            throw new InvalidOperationException("Позиції додають лише до чернетки (Draft)");
        }

        _lines.Add(line);
    }

    public void Issue()
    {
        if (Status != UtilityBillStatus.Draft)
        {
            throw new DomainRuleException("bill.issue", "Тільки чернетку можна випустити");
        }

        if (_lines.Count == 0)
        {
            throw new DomainRuleException("bill.empty", "Не можна випустити порожній рахунок");
        }

        Status = UtilityBillStatus.Issued;
    }

    public void Pay()
    {
        if (Status != UtilityBillStatus.Issued)
        {
            throw new DomainRuleException("bill.pay", $"Стан {Status} не дозволяє оплату");
        }

        Status = UtilityBillStatus.Paid;
    }
}
