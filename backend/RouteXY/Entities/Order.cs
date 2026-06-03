using RouteXY.Api.Enums;

namespace RouteXY.Api.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public Guid DispatcherId { get; set; }
    public Guid? CourierId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? InventoryItemId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public double PickupLat { get; set; }
    public double PickupLng { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public double DeliveryLat { get; set; }
    public double DeliveryLng { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }

    public User Dispatcher { get; set; } = null!;
    public User? Courier { get; set; }
    public Warehouse? Warehouse { get; set; }
    public InventoryItem? InventoryItem { get; set; }
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = [];
}