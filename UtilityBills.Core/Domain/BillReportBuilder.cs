using System.Text;

namespace UtilityBills.Core.Domain;

/// <summary>
/// Будівельник звітів для рахунку.
/// </summary>
public static class BillReportBuilder
{
    /// <summary>
    /// Формує текстовий звіт за рахунком.
    /// </summary>
    /// <param name="bill">Рахунок, для якого будується звіт.</param>
    /// <returns>Текстовий звіт.</returns>
    public static string Build(UtilityBill bill)
    {
        var sb = new StringBuilder();
        foreach (var line in bill.Lines)
        {
            sb.Append($"Послуга: {line.ServiceCode}; обсяг: {line.Volume}; тариф: {line.Tariff}; сума: {line.TotalAmount}\n");
        }

        sb.Append($"Разом: {bill.CalculateTotal(false)}\n");

        return sb.ToString();
    }
}
