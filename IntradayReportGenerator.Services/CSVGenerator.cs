using IntradayReportGenerator.Services.Interfaces;
using IntradayReportGenerator.Services.Models;
using System.Text;

namespace IntradayReportGenerator.Services;

public class CSVGenerator(IFileWriter fileWriter) : IExtractGenerator
{
    public async Task ExportData(string filePath, IEnumerable<PowerTradeAggregated> powerTradeAggregatedList)
    {
        var builder= new StringBuilder();

        builder.AppendLine("Local Time,Volume");          
        foreach (var powerTradeAggregated in powerTradeAggregatedList)
        {
            builder.AppendLine($"{powerTradeAggregated.LocalTime},{powerTradeAggregated.Volume}");
        }
        await fileWriter.WriteAllTextAsync(filePath, builder.ToString());
    }
}
