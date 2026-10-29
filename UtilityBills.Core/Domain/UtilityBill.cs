using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilityBills.Core.Domain;

public class UtilityBill
{
    private readonly List<string[]> _lines = new List<string[]>();

    public UtilityBill(string id, string customerName)
    {
        Id = id; // ставимо id
        CustomerName = customerName; // ставимо ab
        CreatedAt = DateTime.Now; // ставимо дату
    }

    public string Id { get; private set; }

    public string CustomerName { get; private set; }

    public int Status { get; private set; } = 0; // 0-новий,1-оплач,2-відпр,3-скасов

    public DateTime CreatedAt { get; private set; }
    // public string prim; // примітка, поки не треба

    public static UtilityBill FindById(List<UtilityBill> bills, string id)
    {
        for (int i = 0; i < bills.Count; i++)
        {
            if (bills[i].Id == id)
            {
                return bills[i];
            }
        }

        return null;
    }

    public void AddLine(string serviceCode, int volume, decimal tariff)
    {
        string[] line = new string[3];
        line[0] = serviceCode;
        line[1] = volume.ToString();
        line[2] = tariff.ToString();
        _lines.Add(line); // додаємо t у ln
    }

    public decimal CalculateTotal(bool hasSubsidy)
    {
        decimal total = 0;
        int lineCount = 0;

        for (int i = 0; i < _lines.Count; i++)
        {
            int volume = int.Parse(_lines[i][1]);
            decimal tariff = decimal.Parse(_lines[i][2]);
            total = total + (volume * tariff); // додаємо до суми
            lineCount = lineCount + 1; // збільшуємо kolvo на одиницю
        }

        // if (total > 500) { total = total - 50; } // стара знижка
        if (hasSubsidy && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = total + (total * 0.2m);

        return Math.Round(total, 2); // повертаємо sum1
    }

    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false; // не можна
    }

    public bool IsValid()
    {
        if (Id != null && Id != string.Empty
            && CustomerName != null && CustomerName.Length > 2
            && _lines.Count > 0 && _lines.Count < 100
            && Status >= 0 && Status <= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string BuildReport()
    {
        string report = string.Empty;

        for (int i = 0; i < _lines.Count; i++)
        {
            report = report + "Послуга: " + _lines[i][0] +
                     "; обсяг: " + _lines[i][1] +
                     "; тариф: " + _lines[i][2] +
                     "; сума: " + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";

        return report; // повертаємо s
    }
}
