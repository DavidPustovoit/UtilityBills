namespace UtilityBills.Core.Domain;

/// <summary>
/// Рядок рахунку (послуга).
/// </summary>
/// <param name="ServiceCode">Код послуги.</param>
/// <param name="Volume">Обсяг споживання.</param>
/// <param name="Tariff">Тариф за одиницю.</param>
public record LineItem(string ServiceCode, int Volume, decimal Tariff)
{
    /// <summary>
    /// Отримує загальну суму за послугу.
    /// </summary>
    public decimal TotalAmount => Volume * Tariff;
}
