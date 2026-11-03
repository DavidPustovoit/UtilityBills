# Реєстр дефектів проєктування коду

| ID | Місце (файл:рядки) | Дефект | Серйозність | Пріоритет |
|---|---|---|---|---|
| D-01 | `BillCalculator.cs:25-33` | Сходинки тарифу магічними числами (Профільний) | 5 | 20 |
| D-02 | `BillCalculator.cs:25-33` | Довгий ланцюг if-else за сходинками (Профільний) | 4 | 16 |
| D-03 | `BillCalculator.cs:9-50` | Довгий метод (45 логічних рядків) | 5 | 15 |
| D-04 | `BillCalculator.cs:9` | Довгий список параметрів (5 аргументів) | 3 | 9 |
| D-05 | `BillCalculator.cs:14-18` | Дублювання (Volume >= 10) з `LegacyProcessor` | 4 | 8 |
| D-06 | `BillCalculator.cs:12` | Заздрість до чужих даних (доступ до полів `Indicator`) | 3 | 9 |
| D-07 | `BillCalculator.cs:35-42` | Магічні числа (SALE10, MINUS200, 1000, 200, 0.1m) | 4 | 12 |
| D-08 | `BillCalculator.cs:39` | Складний вираз (`couponCode == "MINUS200" && total > 1000`) | 2 | 4 |
| D-09 | `BillCalculator.cs:9` | Відсутня передумова (Guard Clause для `indicators == null`) | 4 | 12 |
| D-10 | `BillCalculator.cs:22` | Множинне присвоєння (мутація змінної `total` 7 разів) | 3 | 9 |
| D-11 | `BillValidator.cs:9-13` | Змішування логіки та виводу (`Console.WriteLine`) | 4 | 12 |
| D-12 | `BillCalculator.cs:12` | Погане іменування (`ind` замість `indicator`) | 1 | 3 |