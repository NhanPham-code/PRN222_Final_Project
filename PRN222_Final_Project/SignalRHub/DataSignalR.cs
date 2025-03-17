using Microsoft.AspNetCore.SignalR;

namespace PRN222_Final_Project.SignalRHub
{
    public class DataSignalR : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
