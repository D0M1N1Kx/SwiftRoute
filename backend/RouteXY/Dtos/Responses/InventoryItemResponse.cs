namespace RouteXY.Api.Responses;

public class InventoryItemResponse
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public string? Category { get; set; }
    public DateTime UpdatedAt { get; set; }
}