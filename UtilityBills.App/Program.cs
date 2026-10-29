using System;
using System.Text;
using UtilityBills.Core.Domain;

Console.OutputEncoding = Encoding.UTF8;
var z = new bill("A-1001", "Іваненко");
z.Add("WATER", 3, 250m);
z.Add("ELEC", 12, 90m);
Console.WriteLine(z.ProcessData(false));
Console.WriteLine(z.ProcessData(true));
Console.WriteLine(z.Ck());
Console.WriteLine(z.Chg(1));
Console.Write(z.Rep());