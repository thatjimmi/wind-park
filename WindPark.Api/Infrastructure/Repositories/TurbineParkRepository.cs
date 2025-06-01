using WindPark.Domain;

namespace WindPark.Infrastructure.Repositories;

public interface ITurbineParkRepository
{
    Task<TurbinePark> GetTurbineParkState();
    Task UpdateStateAsync(TurbinePark park, string operation, string? details = null);
}

public class InMemoryTurbineParkRepository : ITurbineParkRepository
{
    private readonly object _lock = new();
    private readonly ILogger<InMemoryTurbineParkRepository> _logger;
    
    private TurbinePark _current;

    public InMemoryTurbineParkRepository(ILogger<InMemoryTurbineParkRepository> logger)
    {
        _logger = logger;
        
        _current = new TurbinePark(
            ProductionTarget: 10,
            MarketPrice: 6m,
            Turbines:
            [
                new Turbine('A', 2, 15m),
                new Turbine('B', 2, 5m),
                new Turbine('C', 6, 5m),
                new Turbine('D', 6, 5m),
                new Turbine('E', 5, 3m)
            ]);
    }

    public Task<TurbinePark> GetTurbineParkState()
    {
        lock (_lock)
        {
            return Task.FromResult(_current);
        }
    }

    public Task UpdateStateAsync(TurbinePark park, string operation, string? details = null)
    {
        lock (_lock)
        {
            _current = park;
        }
        
        _logger.LogInformation("Saving park state - Operation: {Operation}, Details: {Details}", 
            operation, details);

        return Task.CompletedTask;
    }
}