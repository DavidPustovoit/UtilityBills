using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityBills.Core.Domain;

/// <summary>
/// Рахунок за комунальні послуги абонента.
/// </summary>
public class UtilityBill
{
    private const decimal VatRate = 0.2m;
    private const decimal SubsidyDiscountThreshold = 1000m;
    private const decimal SubsidyDiscountRate = 0.10m;
    private const decimal LargeConsumptionThreshold = 5000m;
    private const decimal LargeConsumptionDiscountRate = 0.15m;
    private const int BulkLineCount = 10;
    private const decimal BulkDiscountAmount = 100m;
    private const int MaxLineCount = 100;
    private const int MinCustomerNameLength = 2;

    private readonly List<LineItem> _lines = new();

    /// <summary>
    /// Ініціалізує новий екземпляр рахунку.
    /// </summary>
    /// <param name="id">Ідентифікатор рахунку.</param>
    /// <param name="customerName">Ім'я абонента.</param>
    public UtilityBill(string id, string customerName)
    {
        Id = id;
        CustomerName = customerName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Отримує ідентифікатор рахунку.</summary>
    public string Id { get; }

    /// <summary>Отримує ім'я абонента.</summary>
    public string CustomerName { get; }

    /// <summary>Отримує поточний стан рахунку.</summary>
    public BillStatus Status { get; private set; } = BillStatus.New;

    /// <summary>Отримує дату створення рахунку.</summary>
    public DateTime CreatedAt { get; }

    /// <summary>Отримує список позицій у рахунку.</summary>
    public IReadOnlyList<LineItem> Lines => _lines.AsReadOnly();

    /// <summary>
    /// Шукає рахунок за ідентифікатором у списку.
    /// </summary>
    /// <param name="bills">Список рахунків.</param>
    /// <param name="id">Ідентифікатор для пошуку.</param>
    /// <returns>Знайдений рахунок або null, якщо не знайдено.</returns>
    public static UtilityBill? FindById(IEnumerable<UtilityBill> bills, string id)
    {
        return bills.FirstOrDefault(b => b.Id == id);
    }

    /// <summary>
    /// Додає до рахунку нову позицію (послугу).
    /// </summary>
    /// <param name="serviceCode">Код послуги.</param>
    /// <param name="volume">Обсяг споживання.</param>
    /// <param name="tariff">Тариф за одиницю.</param>
    public void AddLine(string serviceCode, int volume, decimal tariff)
    {
        _lines.Add(new LineItem(serviceCode, volume, tariff));
    }

    /// <summary>
    /// Обчислює загальну суму рахунку з урахуванням субсидій та податків.
    /// </summary>
    /// <param name="hasSubsidy">Ознака наявності субсидії.</param>
    /// <returns>Загальна сума до сплати.</returns>
    public decimal CalculateTotal(bool hasSubsidy)
    {
        var total = CalculateSubtotal();
        total = ApplySubsidies(total, hasSubsidy);
        total = ApplyBulkDiscount(total);

        if (total < 0)
        {
            total = 0;
        }

        return Math.Round(total + (total * VatRate), 2);
    }

    /// <summary>
    /// Змінює стан рахунку, якщо перехід дозволений.
    /// </summary>
    /// <param name="newStatusInt">Цільовий стан рахунку.</param>
    /// <returns>true, якщо стан змінено; false, якщо перехід заборонений.</returns>
    public bool TryChangeStatus(int newStatusInt)
    {
        if (!Enum.IsDefined(typeof(BillStatus), newStatusInt))
        {
            return false;
        }

        var newStatus = (BillStatus)newStatusInt;

        bool isValidTransition = (Status == BillStatus.New && (newStatus == BillStatus.Paid || newStatus == BillStatus.Cancelled)) ||
                                 (Status == BillStatus.Paid && newStatus == BillStatus.Sent);

        if (isValidTransition)
        {
            Status = newStatus;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє рахунок на валідність.
    /// </summary>
    /// <returns>true, якщо рахунок дійсний; інакше false.</returns>
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return false;
        }

        if (string.IsNullOrEmpty(CustomerName) || CustomerName.Length <= MinCustomerNameLength)
        {
            return false;
        }

        if (_lines.Count == 0 || _lines.Count >= MaxLineCount)
        {
            return false;
        }

        if (!Enum.IsDefined(typeof(BillStatus), Status))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Формує текстовий звіт за рахунком.
    /// </summary>
    /// <returns>Рядок зі звітом.</returns>
    public string BuildReport() => BillReportBuilder.Build(this);

    private static decimal ApplySubsidies(decimal currentTotal, bool hasSubsidy)
    {
        if (hasSubsidy && currentTotal > SubsidyDiscountThreshold)
        {
            return currentTotal * (1m - SubsidyDiscountRate);
        }

        if (currentTotal > LargeConsumptionThreshold)
        {
            return currentTotal * (1m - LargeConsumptionDiscountRate);
        }

        return currentTotal;
    }

    private decimal CalculateSubtotal() => _lines.Sum(l => l.TotalAmount);

    private decimal ApplyBulkDiscount(decimal currentTotal)
    {
        if (_lines.Count > BulkLineCount)
        {
            return currentTotal - BulkDiscountAmount;
        }

        return currentTotal;
    }
}
