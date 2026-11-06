using System.Text;

namespace Var14.Core;

/// <summary>
/// Формує текст простого звіту з готових рядків.
/// </summary>
public sealed class ReportBuilder
{
    /// <summary>
    /// Заголовок звіту.
    /// </summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>
    /// Формує текст звіту з готових рядків.
    /// </summary>
    /// <param name="rows">Рядки для включення.</param>
    /// <returns>Текст звіту.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var text = new StringBuilder();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (var row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }
}
