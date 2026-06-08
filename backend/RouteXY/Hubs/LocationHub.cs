using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RouteXY.Api.Hubs;

[Authorize]
public class LocationHub : Hub
{
    public async Task JoinAsDispatcher()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "dispatchers");
    }

    public async Task JoinAsCourier(string courierId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"courier_{courierId}");
    }
}