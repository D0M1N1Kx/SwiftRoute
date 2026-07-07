using RouteXY.Api.Enums;

namespace RouteXY.Api.Modules.Order;

public class OrderHistoryResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ChangedById { get; set; }
    public required string ChangedByName { get; set; }
    public OrderStatus OldStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public string? Note { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}