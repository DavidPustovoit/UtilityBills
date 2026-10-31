using System;
using Var14.Core.Abstractions;
using Var14.Core.Domain;
using Var14.Core.Pricing;
using Var14.Core.Services;
using Var14.Core.Storage;

// Викликаємо 'new' для створення логіки
IUtilityBillRepository repository = new InMemoryUtilityBillRepository();
IPricingPolicy pricing = new DiscountPricingPolicy(0.10m); // Даємо 10% знижки

var service = new UtilityBillService(repository, pricing);

// Створюємо рахунок і додаємо позиції
var bill = new UtilityBill(1, "SUB-777", DateOnly.FromDateTime(DateTime.Today));
bill.AddLine(new BillLine("Електроенергія", 100, 4.32m)); // 100 кВт по 4.32 грн (432 грн)

service.RegisterBill(bill);

// Має вивести суму зі знижкою 10% (432 - 43.2 = 388.80)
Console.WriteLine($"Сума до сплати: {service.CalculateTotal(bill):0.00} грн");