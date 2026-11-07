# UtilityBills

![CI](https://github.com/DavidPustovoit/UtilityBills/actions/workflows/ci.yml/badge.svg)

Навчальний проєкт з дисципліни «Конструювання програмного забезпечення».

Варіант № 14: нарахування комунальних платежів.

## Склад рішення
- UtilityBills.Core — логіка предметної області.
- UtilityBills.App — консольний застосунок введення і виведення.

## Вимоги
.NET SDK 8.0 або новіший, Git.

## Складання та запуск
dotnet build

dotnet run --project UtilityBills.App

## Стандарт кодування
Правила описано у файлі CODING_STANDARD.md і закріплено в .editorconfig.

Перевірка: dotnet format --verify-no-changes


## Безперервна інтеграція

Конвеєр `.github/workflows/ci.yml` запускається на кожен push у `main` і `feature/**` та на кожен pull request у `main`. Кроки йдуть від найдешевшого до найдорожчого: помилка форматування зупиняє конвеєр раніше, ніж почнеться компіляція.

| Крок | Команда | Критерій якості |
|------|---------|-----------------|
| Формат | `dotnet format --verify-no-changes` | жодних змін |
| Складання | `dotnet build -c Release` | 0 попереджень і помилок |
| Тести | `dotnet test -c Release` | 100 % зелених |
| Покриття | `/p:Threshold=60` | не менше 60 % рядків |

Перевірити локально перед push: `dotnet format --verify-no-changes && dotnet build -c Release && dotnet test -c Release`.

## Автор
Пустовойт Давид Олександрович, група 472