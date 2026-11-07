using Var14.Core;
using Xunit;

namespace Var14.Tests;

public class BillLineParserTests
{
    [Theory]
    [InlineData("", "10", "5")]
    [InlineData("   ", "10", "5")]
    [InlineData("WATER", "", "5")]
    [InlineData("WATER", "abc", "5")]
    [InlineData("WATER", "0", "5")]
    [InlineData("WATER", "-1", "5")]
    [InlineData("WATER", "1000001", "5")]
    [InlineData("WATER", "10", "")]
    [InlineData("WATER", "10", "abc")]
    [InlineData("WATER", "10", "-1")]
    [InlineData("WATER", "10", "1000001")]
    public void Parse_InvalidInput_ReturnsFailure(string name, string volume, string price)
    {
        var result = BillLineParser.Parse(name, volume, price);

        Assert.False(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
    }

    [Fact]
    public void Parse_ServiceNameTooLong_ReturnsFailure()
    {
        var tooLong = new string('A', BillLineParser.MaxServiceNameLength + 1);

        var result = BillLineParser.Parse(tooLong, "10", "5");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Parse_ValidInput_ReturnsTrimmedLine()
    {
        var result = BillLineParser.Parse(" WATER ", "10", "5");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("WATER", result.Value!.ServiceName);
        Assert.Equal(10m, result.Value.Volume);
        Assert.Equal(5m, result.Value.TariffRate);
    }
}
