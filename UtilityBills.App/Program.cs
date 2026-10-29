using System;
using System.Text;
using UtilityBills.Core.Domain;

Console.OutputEncoding = Encoding.UTF8;
var z = new UtilityBill("A-1001", "Іваненко");
z.AddLine("WATER", 3, 250m);
z.AddLine("ELEC", 12, 90m);
Console.WriteLine(z.CalculateTotal(false));
Console.WriteLine(z.CalculateTotal(true));
Console.WriteLine(z.IsValid());
Console.WriteLine(z.TryChangeStatus(1));
Console.Write(z.BuildReport());
