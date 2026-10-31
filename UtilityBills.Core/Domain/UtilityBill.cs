using System;
using System.Collections.Generic;

namespace UtilityBills.Core.Domain;

/// <summary>
/// Рахунок за комунальні послуги абонента.
/// </summary>
public class UtilityBill
{
    /// <summary>Ставка податку на додану вартість.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Поріг суми для базової субсидії.</summary>
    private const decimal SubsidyDiscountThreshold = 1000m;

    /// <summary>Частка знижки за базовою субсидією.</summary>
    private const decimal SubsidyDiscountRate = 0.10m;

    /// <summary>Поріг суми для підвищеної субсидії.</summary>
    private const decimal LargeConsumptionThreshold = 5000m;

    /// <summary>Частка знижки за підвищеною субсидією.</summary>
    private const decimal LargeConsumptionDiscountRate = 0.15m;

    /// <summary>Кількість послуг, з якої діє знижка за комплексність.</summary>
    private const int BulkLineCount = 10;

    /// <summary>Сума знижки за комплексність, грн.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Максимальна кількість позицій у рахунку.</summary>
    private const int MaxLineCount = 100;

    /// <summary>Мінімальна довжина імені абонента.</summary>
    private const int MinCustomerNameLength = 2;

    /// <summary>Стан: новий.</summary>
    private const int StatusNew = 0;

    /// <summary>Стан: оплачений.</summary>
    private const int StatusPaid = 1;

    /// <summary>Стан: відправлений.</summary>
    private const int StatusSent = 2;

    /// <summary>Стан: скасований.</summary>
    private const int StatusCancelled = 3;

    private readonly List<string[]> _lines = new List<string[]>();

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
    public string Id { get; private set; }

    /// <summary>Отримує ім'я абонента.</summary>
    public string CustomerName { get; private set; }

    /// <summary>Отримує поточний стан рахунку.</summary>
    public int Status { get; private set; } = StatusNew;

    /// <summary>Отримує дату створення рахунку.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає рахунок за ідентифікатором у списку.
    /// </summary>
    /// <param name="bills">Список рахунків.</param>
    /// <param name="id">Ідентифікатор для пошуку.</param>
    /// <returns>Знайдений рахунок або null, якщо не знайдено.</returns>
    public static UtilityBill? FindById(List<UtilityBill> bills, string id)
    {
        for (int i = 0; i < bills.Count; i++)
        {
            if (bills[i].Id == id)
            {
                return bills[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до рахунку нову позицію (послугу).
    /// </summary>
    /// <param name="serviceCode">Код послуги.</param>
    /// <param name="volume">Обсяг споживання.</param>
    /// <param name="tariff">Тариф за одиницю.</param>
    public void AddLine(string serviceCode, int volume, decimal tariff)
    {
        string[] line = new string[3];
        line[0] = serviceCode;
        line[1] = volume.ToString();
        line[2] = tariff.ToString();
        _lines.Add(line);
    }

    /// <summary>
    /// Обчислює загальну суму рахунку з урахуванням субсидій та податків.
    /// </summary>
    /// <param name="hasSubsidy">Ознака наявності субсидії.</param>
    /// <returns>Загальна сума до сплати.</returns>
    public decimal CalculateTotal(bool hasSubsidy)
    {
        decimal total = 0;
        int lineCount = 0;

        for (int i = 0; i < _lines.Count; i++)
        {
            int volume = int.Parse(_lines[i][1]);
            decimal tariff = decimal.Parse(_lines[i][2]);
            total = total + (volume * tariff);
            lineCount = lineCount + 1;
        }

        if (hasSubsidy && total > SubsidyDiscountThreshold)
        {
            total = total * (1m - SubsidyDiscountRate);
        }
        else if (total > LargeConsumptionThreshold)
        {
            total = total * (1m - LargeConsumptionDiscountRate);
        }

        if (lineCount > BulkLineCount)
        {
            total = total - BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок
        total = total + (total * VatRate);

        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан рахунку, якщо перехід дозволений.
    /// </summary>
    /// <param name="newStatus">Цільовий стан рахунку.</param>
    /// <returns>true, якщо стан змінено; false, якщо перехід заборонений.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusPaid)
        {
            Status = StatusPaid;
            return true;
        }

        if (Status == StatusPaid && newStatus == StatusSent)
        {
            Status = StatusSent;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
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
        if (Id != null && Id != string.Empty
            && CustomerName != null && CustomerName.Length > MinCustomerNameLength
            && _lines.Count > 0 && _lines.Count < MaxLineCount
            && Status >= StatusNew && Status <= StatusCancelled)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Формує текстовий звіт за рахунком.
    /// </summary>
    /// <returns>Рядок зі звітом.</returns>
    public string BuildReport()
    {
        string report = string.Empty;

        for (int i = 0; i < _lines.Count; i++)
        {
            report = report + "Послуга: " + _lines[i][0] +
                     "; обсяг: " + _lines[i][1] +
                     "; тариф: " + _lines[i][2] +
                     "; сума: " + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";

        return report;
    }
}
