using IntradayReportGenerator.Interfaces;
using IntradayReportGenerator.Services.Helper;
using IntradayReportGenerator.Services.Models;
using Services;

namespace IntradayReportGenerator.Services;

public class TradeAggregator(TimeProvider timeProvider) : ITradeAggregator
{
    public async Task<IEnumerable<PowerTradeAggregated>> AggregateTrades(IEnumerable<PowerTrade> trades)
    {
        var currentDateTime = timeProvider.GetUtcNow().DateTime;       

        var powerTradeAggregated = InitialiseCollectionWithTimePeriods();

        if (trades is null || !trades.Any())
            return [];

        foreach (var trade in trades)
        {
            foreach (var period in trade.Periods)
            {
                var localTime = period.Period.ToLocalTime();
                var volume = Math.Round(period.Volume,3);
                powerTradeAggregated.Find(p => p.LocalTime == localTime)?.Volume += volume;
            }
        }
        return powerTradeAggregated;
    }

    private static List<PowerTradeAggregated> InitialiseCollectionWithTimePeriods()
    {
        var powerTradeAggregated = new List<PowerTradeAggregated>();
        for (var i = 1; i <= 24; i++)
        {
            var volume = 0.0;
            powerTradeAggregated.Add(new PowerTradeAggregated
            {
                LocalTime = i.ToLocalTime(),
                Volume = volume
            });
        }
        return powerTradeAggregated;
    }
}