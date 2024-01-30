using Microsoft.AspNetCore.SignalR;

namespace SignalRServices.Hubs
{
    public class MatchMakingHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task MatchFound(string message)
        {
            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
