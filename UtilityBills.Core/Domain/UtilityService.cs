namespace UtilityBills.Core.Domain;

/// <summary>Комунальна послуга (вода, світло тощо).</summary>
public sealed class UtilityService
{
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public decimal Tariff { get; init; }
}
