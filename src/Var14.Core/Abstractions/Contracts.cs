using System.Collections.Generic;
using Var14.Core.Domain;

namespace Var14.Core.Abstractions;

// Інтерфейс сховища (бази даних)
public interface IUtilityBillRepository
{
    void Add(UtilityBill bill);
    UtilityBill? GetById(int id);
    IReadOnlyList<UtilityBill> GetAll();
}

// Інтерфейс політики розрахунку (тарифів)
public interface IPricingPolicy
{
    decimal Calculate(BillLine line);
}