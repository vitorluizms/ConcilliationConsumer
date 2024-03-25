namespace ConcilliationConsumer.Models;
public class BaseEntity
{
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}