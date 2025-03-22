using Microsoft.AspNetCore.SignalR;

namespace Razor_UI.SignalRHub
{
    public class DataSignalR : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task UpdatePaymentStatus(int orderId, string status)
        {
            await Clients.All.SendAsync("ReceivePaymentUpdate", orderId, status);
        }
    }
}