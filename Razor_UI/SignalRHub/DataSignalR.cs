using Microsoft.AspNetCore.SignalR;

namespace Razor_UI.SignalRHub
{
    public class DataSignalR : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}