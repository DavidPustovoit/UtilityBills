using Var14.Core.Domain;
using Var14.Core.Errors;

namespace Var14.App;

public static class BillLineParser
{
    public static Result<BillLine> Parse(string serviceName, string volumeStr, string priceStr)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return Result<BillLine>.Fail("Назва послуги не може бути порожньою");
        }

        if (!decimal.TryParse(volumeStr, out var volume) || volume <= 0)
        {
            return Result<BillLine>.Fail("Об'єм — це число більше 0");
        }

        if (!decimal.TryParse(priceStr, out var price) || price < 0)
        {
            return Result<BillLine>.Fail("Тариф не може бути від'ємним");
        }

        return Result<BillLine>.Ok(new BillLine(serviceName, volume, price));
    }
}
