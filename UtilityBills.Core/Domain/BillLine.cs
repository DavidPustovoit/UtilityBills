namespace UtilityBills.Core.Domain;

/// <summary>Позиція рахунку: послуга, обсяг споживання та тариф.</summary>
public sealed class BillLine
{
    public string ServiceCode { get; init; } = string.Empty;
    public decimal Volume { get; init; }
    public decimal Tariff { get; init; }
    public decimal Amount => Volume * Tariff;
}