using System;

namespace Var14.Core.Domain;

public sealed record BillLine
{
    public BillLine(string serviceName, decimal volume, decimal tariffRate)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Назва послуги не може бути порожньою");
        }

        if (volume <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(volume), "Об'єм має бути > 0");
        }

        if (tariffRate < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tariffRate), "Тариф не може бути < 0");
        }

        ServiceName = serviceName;
        Volume = volume;
        TariffRate = tariffRate;
    }

    public string ServiceName { get; }
    public decimal Volume { get; }
    public decimal TariffRate { get; }

    // Обчислюване значення, а не поле
    public decimal Amount => Volume * TariffRate;
}
