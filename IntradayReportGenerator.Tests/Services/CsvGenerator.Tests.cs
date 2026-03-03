using IntradayReportGenerator.Services;
using IntradayReportGenerator.Services.Interfaces;
using IntradayReportGenerator.Services.Models;
using Moq;

namespace IntradayReportGenerator.UnitTests.Services;

public class CsvGeneratorTests
{
    private readonly CSVGenerator _csvGenerator;
    private readonly Mock<IFileWriter> _fileWriter;

    public CsvGeneratorTests()
    {
        _fileWriter = new Mock<IFileWriter>();
        _csvGenerator = new CSVGenerator(_fileWriter.Object);
    }

    [Fact]
    public async Task ExportData_WithValidData_CreatesFileWithCorrectContent()
    {
        var filePath = "C:\\Temp\test.csv";
        var data = new List<PowerTradeAggregated>
        {
        new() { LocalTime = new TimeOnly(23, 0), Volume = 100.554 },
        new() { LocalTime = new TimeOnly(0, 0), Volume = 200.753 },
        new() { LocalTime = new TimeOnly(1, 0), Volume = 150.253}
        };

        await _csvGenerator.ExportData(filePath, data);

        _fileWriter.Verify(x => x.WriteAllTextAsync(
            filePath, It.Is<string>(s => 
                            s.Contains("Local Time,Volume") && 
                            s.Contains("23:00,100.554") && 
                            s.Contains("00:00,200.753") &&                                 
                            s.Contains("01:00,150.253"))));
    }          
}
