using System.ComponentModel;

namespace RouteXY.Api.Requests;

public class CreateWarehouseRequest
{
    public string Name { get; set; } = string.Empty;

    [DefaultValue("Vaci ut 23.")]
    public string? Address { get; set; }

    [DefaultValue("Budapest")]
    public string? City { get; set; }

    [DefaultValue(47.4979)]
    public double? Latitude { get; set; }

    [DefaultValue(19.0402)]
    public double? Longitude { get; set; }
}