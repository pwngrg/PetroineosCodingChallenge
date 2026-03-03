using IntradayReportGenerator;
using IntradayReportGenerator.Interfaces;
using IntradayReportGenerator.Services;
using IntradayReportGenerator.Services.Interfaces;
using Services;

// We want to catch any unhandled exception in the app domain to log it for diagnostic purposes.
AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
{
    var logger = Host.CreateDefaultBuilder()
                .Build()
                .Services.GetRequiredService<ILogger<Program>>();

    logger.LogCritical("An unhandled exception occurred. Application is terminating. Exception object: {ExceptionObject}", eventArgs.ExceptionObject);
};

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<ITradeAggregator, TradeAggregator>();
builder.Services.AddScoped<IExtractGenerator, CSVGenerator>();
builder.Services.AddScoped<IPowerService, PowerService>();
builder.Services.AddSingleton(TimeProvider.System);


//We do not want Background service terminating in case of any unhandled exception.
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "IntradayReportGenerator";
});

builder.Services.AddHostedService<HostedService>();

var host = builder.Build();

try
{
    host.Run();
}
catch (Exception ex)
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "An error occurred while running the service. Application is terminating.");
    throw;
}