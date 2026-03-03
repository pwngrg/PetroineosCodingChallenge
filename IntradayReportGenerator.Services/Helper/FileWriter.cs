using IntradayReportGenerator.Services.Interfaces;

namespace IntradayReportGenerator.Services.Helper;
public class FileWriter : IFileWriter
{
    public async Task WriteAllTextAsync(string path, string contents)
    {
        await File.WriteAllTextAsync(path, contents);
    }
}
