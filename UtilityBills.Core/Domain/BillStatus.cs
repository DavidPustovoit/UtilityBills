namespace UtilityBills.Core.Domain;

/// <summary>
/// Статуси рахунку.
/// </summary>
public enum BillStatus
{
    /// <summary>Новий рахунок.</summary>
    New = 0,

    /// <summary>Оплачений рахунок.</summary>
    Paid = 1,

    /// <summary>Відправлений рахунок.</summary>
    Sent = 2,

    /// <summary>Скасований рахунок.</summary>
    Cancelled = 3,
}
