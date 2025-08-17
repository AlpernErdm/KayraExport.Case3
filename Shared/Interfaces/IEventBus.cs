namespace Shared.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : class;
    
    Task PublishAsync(IEnumerable<object> events);
}
