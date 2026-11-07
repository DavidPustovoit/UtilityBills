using System.Globalization;
using Var14.Core.Domain;
using Var14.Core.Errors;

namespace Var14.Core;

/// <summary>
/// Межа між «брудними» даними з консолі та доменними сутностями.
/// </summary>
public static class BillLineParser
{
    /// <summary>
    /// Максимальна довжина назви послуги.
    /// </summary>
    public const int MaxServiceNameLength = 100;

    /// <summary>
    /// Максимально припустимий обсяг споживання.
    /// </summary>
    public const decimal MaxVolume = 1_000_000m;

    /// <summary>
    /// Максимально припустимий тариф за одиницю.
    /// </summary>
    public const decimal MaxTariff = 1_000_000m;

    /// <summary>
    /// Розбирає рядкові значення з консолі в сутність <see cref="BillLine"/>.
    /// </summary>
    /// <param name="serviceName">Назва послуги.</param>
    /// <param name="volumeStr">Обсяг у вигляді рядка.</param>
    /// <param name="priceStr">Тариф у вигляді рядка.</param>
    /// <returns>Успішний результат із сутністю або повідомлення про помилку.</returns>
    public static Result<BillLine> Parse(string serviceName, string volumeStr, string priceStr)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return Result<BillLine>.Fail("Назва послуги не може бути порожньою");
        }

        var trimmedName = serviceName.Trim();
        if (trimmedName.Length > MaxServiceNameLength)
        {
            return Result<BillLine>.Fail($"Назва послуги не може перевищувати {MaxServiceNameLength} символів");
        }

        var culture = CultureInfo.CurrentCulture;
        var styles = NumberStyles.Number;

        if (!decimal.TryParse(volumeStr, styles, culture, out var volume) || volume <= 0)
        {
            return Result<BillLine>.Fail("Об'єм — це число більше 0");
        }

        if (volume > MaxVolume)
        {
            return Result<BillLine>.Fail($"Об'єм не може перевищувати {MaxVolume}");
        }

        if (!decimal.TryParse(priceStr, styles, culture, out var price) || price < 0)
        {
            return Result<BillLine>.Fail("Тариф не може бути від'ємним");
        }

        if (price > MaxTariff)
        {
            return Result<BillLine>.Fail($"Тариф не може перевищувати {MaxTariff}");
        }

        return Result<BillLine>.Ok(new BillLine(trimmedName, volume, price));
    }
}
