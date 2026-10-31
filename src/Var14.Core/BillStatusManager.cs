namespace Var14.Core;

public static class BillStatusManager
{
    public static decimal Update(string currentStatus, decimal currentTotal, out string newStatus)
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
}