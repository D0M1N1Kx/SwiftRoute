using System.ComponentModel;

namespace RouteXY.Api.Requests;

public class UpdateCourierLocationRequest
{
    [DefaultValue(47.4979)]
    public double Latitude { get; set; }

    [DefaultValue(19.0402)]
    public double Longitude { get; set; }

    [DefaultValue(47.5)]
    public float? SpeedKmh { get; set; }

    [DefaultValue(180.0)]
    public float? Heading { get; set; }

    public Guid? OrderId { get; set; }
}