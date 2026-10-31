using System;
using System.Collections.Generic;
using Var14.Core.Legacy;

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

        if (!BillValidator.Validate(docId, clientName, clientEmail, indicators, status))
        {
            return -1;
        }

        decimal total = BillCalculator.Calculate(indicators, isRegular, couponCode, createdAt, serviceFee);

        total = BillStatusManager.Update(status, total, out newStatus);

        if (printToConsole)
        {
            BillPrinter.Print(docId, clientName, clientEmail, indicators, total, newStatus);
        }

        return total;
    }
}