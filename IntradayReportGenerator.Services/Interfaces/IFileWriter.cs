namespace IntradayReportGenerator.Services.Interfaces;

public interface IFileWriter
{
    Task WriteAllTextAsync(string path, string contents);
}
