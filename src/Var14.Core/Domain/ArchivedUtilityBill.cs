using System;
using System.Collections.Generic;

namespace Var14.Core.Domain;

// Архівний рахунок не є звичайним рахунком, тому ми не робимо : UtilityBill. 
// Він лише інкапсулює його в собі, щоб показати дані.
public sealed class ArchivedUtilityBill
{
    private readonly UtilityBill _bill;

    public ArchivedUtilityBill(UtilityBill bill, DateOnly archivedAt)
    {
        _bill = bill ?? throw new ArgumentNullException(nameof(bill));
        ArchivedAt = archivedAt;
    }

    public DateOnly ArchivedAt { get; }
    
    // Показуємо тільки те, що безпечно для читання
    public int Id => _bill.Id;
    public UtilityBillStatus Status => _bill.Status;
    public IReadOnlyList<BillLine> Lines => _bill.Lines;
}