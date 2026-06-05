using System.ComponentModel;

namespace RouteXY.Api.Requests;

public class CreateOrderRequest
{
    public string RecipientName { get; set; } = string.Empty;

    [DefaultValue("+36201234567")]
    public string RecipientPhone { get; set; } = string.Empty;

    [DefaultValue("Budapest, Kossuth ter 1.")]
    public string PickupAddress { get; set; } = string.Empty;

    [DefaultValue(47.5068)]
    public double PickupLat { get; set; }

    [DefaultValue(19.0457)]
    public double PickupLng { get; set; }

    [DefaultValue("Budapest, Andrassy ut 60.")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [DefaultValue(47.5068)]
    public double DeliveryLat { get; set; }

    [DefaultValue(19.0457)]
    public double DeliveryLng { get; set; }

    public Guid? WarehouseId { get; set; }
    public Guid? InventoryItemId { get; set; }

    [DefaultValue("Fragile package, handle with care")]
    public string? Notes { get; set; }
}