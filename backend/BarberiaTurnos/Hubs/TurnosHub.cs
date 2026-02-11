using Microsoft.AspNetCore.SignalR;

namespace BarberiaTurnos.Hubs;

public class TurnosHub : Hub
{
    public async Task UpdateQueue()
    {
        await Clients.All.SendAsync("QueueUpdated");
    }
}
