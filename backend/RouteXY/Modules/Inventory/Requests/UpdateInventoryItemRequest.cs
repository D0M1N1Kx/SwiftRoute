namespace RouteXY.Api.Requests;

public class UpdateInventoryItemRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public string? Unit { get; set; }
    public string? Category { get; set; }
}