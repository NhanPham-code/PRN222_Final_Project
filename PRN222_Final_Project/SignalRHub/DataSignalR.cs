using Microsoft.AspNetCore.SignalR;

namespace PRN222_Final_Project.SignalRHub
{
    public class DataSignalR : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendOrderNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", message);
        }

        public async Task ReloadCart()
        {
            await Clients.All.SendAsync("ReloadCart");
        }
    }
}
