using System;
using Microsoft.Extensions.Logging;
using Var14.App;
using Var14.Core.Abstractions;
using Var14.Core.Domain;
using Var14.Core.Pricing;
using Var14.Core.Services;
using Var14.Core.Storage;

using var factory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});

var logger = factory.CreateLogger<UtilityBillService>();

// Викликаємо 'new' для створення логіки
IUtilityBillRepository repository = new InMemoryUtilityBillRepository();
IPricingPolicy pricing = new DiscountPricingPolicy(0.10m); // Даємо 10% знижки

var service = new UtilityBillService(repository, pricing, logger);

using var auditLog = new FileAuditLog("audit.log");

// Створюємо рахунок і додаємо позиції
var bill = new UtilityBill(1, "SUB-777", DateOnly.FromDateTime(DateTime.Today));

Console.WriteLine("Введіть дані послуги:");
while (true)
{
    Console.Write("Назва послуги: ");
    string? name = Console.ReadLine();

    Console.Write("Об'єм (наприклад, 10,5): ");
    string? volume = Console.ReadLine();

    Console.Write("Тариф: ");
    string? price = Console.ReadLine();

    var parsed = BillLineParser.Parse(name ?? "", volume ?? "", price ?? "");

    if (!parsed.IsSuccess)
    {
        Console.WriteLine($"\nПОМИЛКА ВВОДУ: {parsed.Error}. Спробуйте ще раз.\n");
        continue;
    }

    bill.AddLine(parsed.Value!);

    break;
}

service.RegisterBill(bill);
Console.WriteLine($"\nСума до сплати: {service.CalculateTotal(bill):0.00} грн");

auditLog.Write("Рахунок успішно розраховано", bill.Id);
