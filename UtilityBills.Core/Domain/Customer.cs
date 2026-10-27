namespace UtilityBills.Core.Domain;

/// <summary>Абонент комунальних послуг.</summary>
public sealed class Customer
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool HasSubsidy { get; init; }
}