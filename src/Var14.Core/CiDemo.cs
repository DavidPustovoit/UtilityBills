namespace Var14.Core;

/// <summary>
/// Навмисний дефект для демонстрації зламаного конвеєра (Завдання 5).
/// </summary>
public static class CiDemo
{
    /// <summary>
    /// Повертає опис кількості.
    /// </summary>
    /// <param name="count">Кількість.</param>
    /// <returns>Опис.</returns>
    public static string Describe(int count)
    {
        string note;
        return "Позицій: " + count;
    }
}
