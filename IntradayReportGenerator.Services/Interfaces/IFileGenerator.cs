using IntradayReportGenerator.Services.Models;

namespace IntradayReportGenerator.Services.Interfaces;

public interface IExtractGenerator
{
    Task ExportData(string storePath, IEnumerable<PowerTradeAggregated> content);
}
