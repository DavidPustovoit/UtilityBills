using Var14.Core;
using Xunit;

namespace Var14.Tests;

public class VersionInfoTests
{
    [Fact]
    public void Current_ReturnsVersionFromBuildProps()
    {
        // Arrange & Act
        var actual = VersionInfo.Current();

        // Assert
        Assert.Equal("0.8.0", actual);
    }
}
