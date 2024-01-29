using Microsoft.AspNetCore.SignalR;

namespace GalaxyPvP.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task ServerInitiatedMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveServerMessage", message);
        }
    }
}
