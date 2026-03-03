using IntradayReportGenerator.Services;
using IntradayReportGenerator.Services.Models;

namespace IntradayReportGenerator.UnitTests.Services
{
    public class CsvGeneratorTests
    {
        private readonly CSVGenerator _csvGenerator;

        public CsvGeneratorTests()
        {
            _csvGenerator = new CSVGenerator();
        }

        [Fact]
        public async Task ExportData_WithValidData_CreatesFileWithCorrectContent()
        {
            var filePath = GetTestFilePath("valid_data.csv");
            var data = new List<PowerTradeAggregated>
            {
            new() { LocalTime = new TimeOnly(23, 0), Volume = 100.554 },
            new() { LocalTime = new TimeOnly(0, 0), Volume = 200.753 },
            new() { LocalTime = new TimeOnly(1, 0), Volume = 150.253}
            };

            await _csvGenerator.ExportData(filePath, data);

            Assert.True(File.Exists(filePath));
            var content = await File.ReadAllTextAsync(filePath);
            var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            Assert.Equal(4, lines.Length);
            Assert.Equal("Local Time,Volume", lines[0]);
            Assert.Equal("23:00,100.554", lines[1]);
            Assert.Equal("00:00,200.753", lines[2]);
            Assert.Equal("01:00,150.253", lines[3]);
        }          

        private string GetTestFilePath(string fileName)
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{fileName}");
            return filePath;
        }
    }
}
