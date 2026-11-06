using Var14.Core;
using Xunit;

namespace Var14.Tests;

public class ReportBuilderTests
{
    [Fact]
    public void Build_WithRows_ContainsUpperCaseTitleAndAllRows()
    {
        // Arrange
        var builder = new ReportBuilder { Title = "звіт" };
        var rows = new[] { "рядок 1", "рядок 2" };

        // Act
        var text = builder.Build(rows);

        // Assert
        Assert.StartsWith("ЗВІТ", text);
        Assert.Contains("рядок 1", text);
        Assert.Contains("рядок 2", text);
    }

    [Fact]
    public void Build_NullRows_Throws()
    {
        // Arrange
        var builder = new ReportBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.Build(null!));
    }
}
