using IntradayReportGenerator.Services.Helper;

namespace IntradayReportGenerator.UnitTests.Services.Helper;

public class DateTimeHelperTests
{
    [Theory]
    [InlineData(1, 23, 0)]
    [InlineData(2, 0, 0)]
    [InlineData(3, 1, 0)]
    [InlineData(4, 2, 0)]
    [InlineData(5, 3, 0)]
    [InlineData(6, 4, 0)]
    [InlineData(7, 5, 0)]
    [InlineData(8, 6, 0)]
    [InlineData(9, 7, 0)]
    [InlineData(10, 8, 0)]
    [InlineData(11, 9, 0)]
    [InlineData(12, 10, 0)]
    [InlineData(13, 11, 0)]
    [InlineData(14, 12, 0)]
    [InlineData(15, 13, 0)]
    [InlineData(16, 14, 0)]
    [InlineData(17, 15, 0)]
    [InlineData(18, 16, 0)]
    [InlineData(19, 17, 0)]
    [InlineData(20, 18, 0)]
    [InlineData(21, 19, 0)]
    [InlineData(22, 20, 0)]
    [InlineData(23, 21, 0)]
    [InlineData(24, 22, 0)]
    public void ToLocalTime_WithValidPeriod_ReturnsCorrectTimeOnly(int period, int expectedHour, int expectedMinute)
    {
        var result = period.ToLocalTime();

        Assert.Equal(expectedHour, result.Hour);
        Assert.Equal(expectedMinute, result.Minute);
    }
}
