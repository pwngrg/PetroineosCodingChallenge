namespace IntradayReportGenerator.Services.Helper;
public static class DateTimeHelper
{
    public static TimeOnly ToLocalTime(this int period)
    {
        return period switch
        {
            1 => new TimeOnly(23, 00),
            2 => new TimeOnly(00, 00),
            3 => new TimeOnly(01, 00),
            4 => new TimeOnly(02, 00),
            5 => new TimeOnly(03, 00),
            6 => new TimeOnly(04, 00),
            7 => new TimeOnly(05, 00),
            8 => new TimeOnly(06, 00),
            9 => new TimeOnly(07, 00),
            10 => new TimeOnly(08, 00),
            11 => new TimeOnly(09, 00),
            12 => new TimeOnly(10, 00),
            13 => new TimeOnly(11, 00),
            14 => new TimeOnly(12, 00),
            15 => new TimeOnly(13, 00),
            16 => new TimeOnly(14, 00),
            17 => new TimeOnly(15, 00),
            18 => new TimeOnly(16, 00),
            19 => new TimeOnly(17, 00),
            20 => new TimeOnly(18, 00),
            21 => new TimeOnly(19, 00),
            22 => new TimeOnly(20, 00),
            23 => new TimeOnly(21, 00),
            24 => new TimeOnly(22, 00),
            _ => throw new ArgumentOutOfRangeException(nameof(period), "Period must be between 1 and 24."),
        };
    }
}
