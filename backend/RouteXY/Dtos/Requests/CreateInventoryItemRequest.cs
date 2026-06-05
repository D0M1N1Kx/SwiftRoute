using System.ComponentModel;

namespace RouteXY.Api.Requests;

public class CreateInventoryItemRequest
{
    public Guid WarehouseId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [DefaultValue(5)]
    public int Quantity { get; set; }

    [DefaultValue("db")]
    public string? Unit { get; set; }

    public string? Category { get; set; }
}