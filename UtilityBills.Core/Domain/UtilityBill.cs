using System;
using System.Collections.Generic;

namespace UtilityBills.Core.Domain;

/// <summary>
/// Рахунок за комунальні послуги абонента.
/// </summary>
public class UtilityBill
{
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
    public int Status { get; private set; } = 0;

    /// <summary>Отримує дату створення рахунку.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає рахунок за ідентифікатором у списку.
    /// </summary>
    /// <param name="bills">Список рахунків.</param>
    /// <param name="id">Ідентифікатор для пошуку.</param>
    /// <returns>Знайдений рахунок або null, якщо не знайдено.</returns>
    public static UtilityBill FindById(List<UtilityBill> bills, string id)
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

        if (hasSubsidy && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок
        total = total + (total * 0.2m);

        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан рахунку, якщо перехід дозволений.
    /// </summary>
    /// <param name="newStatus">Цільовий стан рахунку.</param>
    /// <returns>true, якщо стан змінено; false, якщо перехід заборонений.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
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
            && CustomerName != null && CustomerName.Length > 2
            && _lines.Count > 0 && _lines.Count < 100
            && Status >= 0 && Status <= 3)
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