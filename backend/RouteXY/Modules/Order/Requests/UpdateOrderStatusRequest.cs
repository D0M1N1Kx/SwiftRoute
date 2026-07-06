using System.ComponentModel;
using RouteXY.Api.Enums;

namespace RouteXY.Api.Requests;

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }

    [DefaultValue("Package picked up from warehouse")]
    public string? Note { get; set; }
}