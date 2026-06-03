namespace RouteXY.Api.Entities;

public class CourierLocation
{
    public Guid Id { get; set; }
    public Guid CourierId { get; set; }
    public Guid? OrderId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public float? SpeedKmh { get; set; }
    public float? Heading { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public User Courier { get; set; } = null!;
    public Order? Order { get; set; }
}