using RouteXY.Api.Enums;

namespace RouteXY.Api.Responses;

public class OrderResponse
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public double PickupLat { get; set; }
    public double PickupLng { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public double DeliveryLat { get; set; }
    public double DeliveryLng { get; set; }
    public OrderStatus Status { get; set; }
    public string? Notes { get; set; }
    public string DispatcherName { get; set; } = string.Empty;
    public string? CourierName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}