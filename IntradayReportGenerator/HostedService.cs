using IntradayReportGenerator.Interfaces;
using IntradayReportGenerator.Services.Interfaces;
using Polly;
using Polly.Retry;
using Services;

namespace IntradayReportGenerator;

public class HostedService(
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration,
    TimeProvider timeProvider,
    ILogger<HostedService> logger) : BackgroundService
{
    private readonly ResiliencePipeline _retryPipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(2), // Initial delay before the first retry
            UseJitter = true,
            BackoffType = DelayBackoffType.Exponential,
            ShouldHandle = new PredicateBuilder().Handle<PowerServiceException>(),
            OnRetry = args =>
            {
                //Log the retry attempt
                logger.LogWarning(
                "Retry attempt {AttemptNumber} for PowerService.GetTradesAsync after {Delay}ms. Reason: {Exception}", args.AttemptNumber + 1, args.RetryDelay.TotalMilliseconds, args.Outcome.Exception?.Message);
                return ValueTask.CompletedTask;
            }
        }).Build();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //use two mins as default if not set in configuration
        var delay = configuration.GetValue<int>("RunIntervalInMinutes", 2);

        //use C:\\Temp as default if not set in configuration
        var extractLocation = configuration.GetValue<string>("ExtractLocation", "C:\\Temp");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //use scoped services to ensure that we get fresh instances of the services for each execution cycle, and to properly manage their lifetimes.
                using var scope = serviceScopeFactory.CreateScope();
                var tradeAggregator = scope.ServiceProvider.GetRequiredService<ITradeAggregator>();
                var extractGenerator = scope.ServiceProvider.GetRequiredService<IExtractGenerator>();

                var powerService = scope.ServiceProvider.GetRequiredService<IPowerService>();

                var currentDateTime = timeProvider.GetUtcNow().DateTime;

                var fileName = $"{currentDateTime:yyyyMMdd_HHmm}.csv";

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("{currentDateTime}: Background Worker running", currentDateTime);
                }

                logger.LogInformation("{currentDateTime}: Fetching trade detail from Power Service", currentDateTime);
                //apply retry logic since we have seen the call to the dll failing occassionally.
                var trades = await _retryPipeline.ExecuteAsync(async cancellationToken =>
                {
                    return await powerService.GetTradesAsync(currentDateTime);
                }, stoppingToken);

                logger.LogInformation("{currentDateTime}: Aggregating trades and generating report...", currentDateTime);
                var tradesAggregated = await tradeAggregator.AggregateTrades(trades);

                logger.LogInformation("{currentDateTime}: Writing report to file...", currentDateTime);
                await extractGenerator.ExportData($"{extractLocation}{fileName}", tradesAggregated);

                logger.LogInformation("{currentDateTime}: Report generation completed. Waiting for the next cycle...", currentDateTime);
            }

            //this exception had to be caught as a result of initial manual testing.
            catch (PowerServiceException psex)
            {
                logger.LogError(psex, "External Assembly Error. An error occurred while getting trades from power service. Will try again in the next cycle.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred during the execution of the background service. Will try again in the next cycle.");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromMinutes(delay), stoppingToken);
            }
        }
    }
}
