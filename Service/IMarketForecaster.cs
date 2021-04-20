using DependencyInjection.Models;

namespace DependencyInjection.Service
{
    public interface IMarketForecaster
    {
        MarketResult GetMarketPrediction();
    }
}