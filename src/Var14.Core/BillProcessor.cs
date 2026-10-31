using System;
using System.Collections.Generic;
using Var14.Core.Legacy; // Використовуємо сутність Indicator

namespace Var14.Core;

public static class BillProcessor
{
    public static decimal Process(
        int docId, string clientName, string clientEmail,
        bool isRegular, List<Indicator> indicators,
        DateOnly createdAt, string couponCode, string status,
        bool printToConsole, decimal serviceFee,
        out string newStatus)
    {
        newStatus = status;

        // 1. Валідація
        if (!ValidateInputs(docId, clientName, clientEmail, indicators, status))
        {
            return -1;
        }

        // 2. Розрахунок суми
        decimal total = CalculateTotal(indicators, isRegular, couponCode, createdAt, serviceFee);

        // 3. Зміна статусу
        total = UpdateStatus(status, total, out newStatus);

        // 4. Друк
        if (printToConsole)
        {
            PrintReceipt(docId, clientName, clientEmail, indicators, total, newStatus);
        }

        return total;
    }

    private static bool ValidateInputs(int docId, string clientName, string clientEmail, List<Indicator> indicators, string status)
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

    private static decimal CalculateTotal(List<Indicator> indicators, bool isRegular, string couponCode, DateOnly createdAt, decimal serviceFee)
    {
        decimal total = 0;
        foreach (var ind in indicators)
        {
            decimal sum = ind.Volume * ind.Tariff;
            if (ind.Volume >= 10) sum *= 0.95m;
            total += sum;
        }

        if (isRegular) total *= 0.93m;

        if (total > 5000) total *= 0.9m;
        else if (total > 2000) total *= 0.95m;

        if (couponCode == "SALE10") total -= total * 0.1m;
        else if (couponCode == "MINUS200" && total > 1000) total -= 200m;

        if (createdAt.DayOfWeek == DayOfWeek.Sunday) total *= 0.98m;

        if (total < 1000) total += serviceFee;
        if (total < 0) total = 0;

        return Math.Round(total, 2);
    }

    private static decimal UpdateStatus(string currentStatus, decimal currentTotal, out string newStatus)
    {
        newStatus = currentStatus;
        decimal total = currentTotal;

        if (currentStatus == "New" && total > 0)
            newStatus = "Paid";
        else if (currentStatus == "Paid")
            newStatus = "Shipped";
        else if (currentStatus == "Cancelled")
        {
            newStatus = "Cancelled";
            total = 0;
        }

        return total;
    }

    private static void PrintReceipt(int docId, string clientName, string clientEmail, List<Indicator> indicators, decimal total, string newStatus)
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