using System;
using System.Collections.Generic;
using Var14.Core.Legacy;

namespace Var14.Core;

public static class BillValidator
{
    public static bool Validate(int docId, string clientName, string clientEmail, List<Indicator> indicators, string status)
    {
        if (docId <= 0) { Console.WriteLine("Помилка: номер документа"); return false; }
        if (string.IsNullOrEmpty(clientName)) { Console.WriteLine("Помилка: немає клієнта"); return false; }
        if (string.IsNullOrEmpty(clientEmail) || !clientEmail.Contains('@')) { Console.WriteLine("Помилка: пошта клієнта"); return false; }
        if (indicators == null || indicators.Count == 0) { Console.WriteLine("Помилка: немає позицій"); return false; }
        if (status != "New" && status != "Paid" && status != "Shipped" && status != "Cancelled") { Console.WriteLine("Помилка: невідомий стан"); return false; }

        foreach (var ind in indicators)
        {
            if (ind.Volume <= 0) { Console.WriteLine("Помилка: кількість"); return false; }
            if (ind.Tariff < 0) { Console.WriteLine("Помилка: ціна"); return false; }
        }
        return true;
    }
}
