using IntradayReportGenerator.Services;
using IntradayReportGenerator.Services.Helper;
using Microsoft.Extensions.Time.Testing;
using Services;

namespace IntradayReportGenerator.UnitTests.Services;

public class TradeAggregatorTests
{
    private readonly FakeTimeProvider _fakeTimeProvider;
    private readonly TradeAggregator _tradeAggregator;

    public TradeAggregatorTests()
    {
        _fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 3, 3, 10, 0, 0, TimeSpan.Zero));
        _tradeAggregator = new TradeAggregator(_fakeTimeProvider);
    }


    [Fact]
    public async Task AggregateTrades_WithNullTrades_ReturnsEmptyCollection()
    {
        var result = await _tradeAggregator.AggregateTrades(trades: null);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AggregateTrades_WithEmptyTrades_ReturnsEmptyCollection()
    {
        var trades = Enumerable.Empty<PowerTrade>();
        var result = await _tradeAggregator.AggregateTrades(trades);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AggregateTrades_WithSingleTrade_ReturnsCorrectlyAggregatedVolumes()
    {
        // Arrange
        var periods = new[]
        {
           new PowerPeriod{ Period = 1, Volume = 10.5 },
           new PowerPeriod{ Period = 2, Volume = 20.0 },
           new PowerPeriod{ Period = 3, Volume = 15.5 }
        };

        var trade = PowerTrade.Create(new DateTime(2026, 3, 3), periods.Length);

        for (int i = 0; i < periods.Length; i++)
        {
            trade.Periods[i] = periods[i];
        }

        var trades = new[] { trade };

        // Act
        var result = (await _tradeAggregator.AggregateTrades(trades)).ToList();
        
        // Assert
        Assert.Equal(24, result.Count);
        Assert.Equal(10.5, result.First(x => x.LocalTime == 1.ToLocalTime()).Volume);
        Assert.Equal(20.0, result.First(x => x.LocalTime == 2.ToLocalTime()).Volume);
        Assert.Equal(15.5, result.First(x => x.LocalTime == 3.ToLocalTime()).Volume);
    }

    [Fact]
    public async Task AggregateTrades_WithMultipleTrades_AggregatesVolumesCorrectly()
    {
        var firstPowerPeriods = new[]
        {
            new PowerPeriod{ Period = 1, Volume = 10.0 },
            new PowerPeriod{ Period = 2, Volume = 20.0 },
            new PowerPeriod{ Period = 3,    Volume = 15 }
        };

        var firstTrades = PowerTrade.Create(new DateTime(2026, 3, 3), firstPowerPeriods.Length);

        for (int i = 0; i < firstPowerPeriods.Length; i++)
        {
            firstTrades.Periods[i] = firstPowerPeriods[i];
        }

        var secondPowerPeriods = new[]
        {
            new PowerPeriod{ Period = 1, Volume =  5.0 },
            new PowerPeriod{ Period = 2, Volume = 10.0 },
            new PowerPeriod{ Period = 3, Volume = -5.0 }
        };

        var secondTrades = PowerTrade.Create(new DateTime(2026, 3, 3), secondPowerPeriods.Length);

        for (int i = 0; i < secondPowerPeriods.Length; i++)
        {
            secondTrades.Periods[i] = secondPowerPeriods[i];
        }

        var trades = new[] { firstTrades, secondTrades };
                
        var result = (await _tradeAggregator.AggregateTrades(trades)).ToList();

        Assert.Equal(24, result.Count);
        Assert.Equal(15.0, result.First(x => x.LocalTime == 1.ToLocalTime()).Volume); 
        Assert.Equal(30.0, result.First(x => x.LocalTime == 2.ToLocalTime()).Volume); 
        Assert.Equal(10.0, result.First(x => x.LocalTime == 3.ToLocalTime()).Volume);
    }
}
