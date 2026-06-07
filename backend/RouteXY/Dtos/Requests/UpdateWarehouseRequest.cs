namespace RouteXY.Api.Requests;

public class UpdateWarehouseRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool? IsActive { get; set; }
}