namespace RouteXY.Api.Responses;

public class CourierLocationResponse
{
    public Guid CourierId { get; set; }
    public string CourierName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public float? SpeedKmh { get; set; }
    public float? Heading { get; set; }
    public DateTime RecordedAt { get; set; }
}