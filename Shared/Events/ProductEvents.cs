namespace Shared.Events;

public abstract class BaseEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    
    public string EventType => GetType().Name;
    
    public string? CorrelationId { get; set; }
}

public class ProductCreatedEvent : BaseEvent
{
    public Guid ProductId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    
    public string? Category { get; set; }
    
    public decimal Price { get; set; }
    
    public Guid CreatedByUserId { get; set; }
}

public class ProductUpdatedEvent : BaseEvent
{
    public Guid ProductId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    
    public string? Category { get; set; }
    
    public decimal Price { get; set; }
    
    public Guid UpdatedByUserId { get; set; }
    
    public List<string> UpdatedFields { get; set; } = new();
}

public class ProductDeletedEvent : BaseEvent
{
    public Guid ProductId { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    
    public Guid DeletedByUserId { get; set; }
}
