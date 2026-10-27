using System;
using System.Globalization;
using UtilityBills.Core.Domain;

UtilityBill bill = new()
{
    Id = 1,
    CustomerId = 14,
    CreatedAt = DateTimeOffset.Now,
};

bill.AddLine(new BillLine
{
    ServiceCode = "WATER",
    Volume = 10.5m,
    Tariff = 35.20m,
});
bill.AddLine(new BillLine
{
    ServiceCode = "ELEC",
    Volume = 150m,
    Tariff = 2.64m,
});

string total = bill.Total().ToString("F2", CultureInfo.InvariantCulture);

Console.WriteLine($"Рахунок #{bill.Id}");
Console.WriteLine($"Стан: {bill.Status}");
Console.WriteLine($"Позицій: {bill.Lines.Count}");
Console.WriteLine($"Сума: {total}");
