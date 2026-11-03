# План рефакторингу

| ID | Рефакторинг | В | Р | Т, хв | Індекс | Черга |
|---|---|---|---|---|---|---|
| D-09 | R18 Introduce Guard (перевірка `null`) | 4 | 1 | 5 | 4.0 | 1 |
| D-01 | R04 Replace Magic Number with Constant (сходинки) | 5 | 1 | 15 | 5.0 | 2 |
| D-07 | R04 Replace Magic Number with Constant (купони) | 5 | 1 | 15 | 5.0 | 3 |
| D-12 | R02 Rename (`ind` -> `indicator`) | 3 | 1 | 5 | 3.0 | 4 |
| D-10 | R15 Replace Temp with Query (відмова від мутації `total`) | 4 | 3 | 20 | 1.3 | 5 |
| D-02 | R05 Decompose Conditional (сходинки) | 5 | 3 | 25 | 1.6 | 6 |
| D-08 | R03 Extract Variable (для умов купона) | 3 | 2 | 10 | 1.5 | 7 |
| D-03 | R01 Extract Method (ApplyCoupons) | 5 | 3 | 20 | 1.6 | 8 |
| D-03 | R01 Extract Method (ApplyWeekendDiscount) | 5 | 2 | 15 | 2.5 | 9 |
| D-03 | R01 Extract Method (ApplyServiceFee) | 5 | 2 | 15 | 2.5 | 10 |
| D-06 | R10 Move Method (обрахунок бази ближче до `Indicator`) | 4 | 3 | 20 | 1.3 | 11 |
| D-04 | R11 Introduce Parameter Object (BillContext - опціонально) | 2 | 4 | 20 | 0.5 | 12 |

**Правила безпеки:**
1. Один рефакторинг - один коміт.
2. Після кожного збереження - `dotnet test`. Якщо тести падають більше ніж 5 хвилин - `git restore .`.
3. Спочатку виносяться константи, потім розбиваються методи (нижчий ризик готує базу для вищого).