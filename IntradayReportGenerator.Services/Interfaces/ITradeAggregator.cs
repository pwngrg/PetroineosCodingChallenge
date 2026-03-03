using IntradayReportGenerator.Services.Models;
using Services;

namespace IntradayReportGenerator.Interfaces;

public interface ITradeAggregator
{
    Task<IEnumerable<PowerTradeAggregated>> AggregateTrades(IEnumerable<PowerTrade> trades);
}