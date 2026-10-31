using System;
using System.Collections.Generic;
using Var14.Core.Legacy;

namespace Var14.Core;

public static class BillPrinter
{
    public static void Print(int docId, string clientName, string clientEmail, List<Indicator> indicators, decimal total, string newStatus)
    {
        Console.WriteLine($"Документ № {docId}");
        Console.WriteLine($"Клієнт: {clientName}");
        Console.WriteLine($"Пошта: {clientEmail}");
        foreach (var ind in indicators)
        {
            Console.WriteLine($"{ind.TariffCode} x {ind.Volume}");
        }
        Console.WriteLine($"Разом: {total}");
        Console.WriteLine($"Стан: {newStatus}");
    }
}