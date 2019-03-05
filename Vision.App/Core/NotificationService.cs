using BlazorSignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using Vision.Shared;

namespace Vision.App
{
    public class NotificationService
    {
        private HubConnection connection;
        public Action<AlertDto> OnMessage { get; set; }

        public async Task ConnectAsync()
        {
            connection = new HubConnectionBuilder().WithUrlBlazor("/notificationhub").Build();
            connection.On<AlertDto>("AlertsChannel", (alert) => OnMessage?.Invoke(alert));

            if (connection.State != HubConnectionState.Connected)
            {
                await connection.StartAsync();
                Console.WriteLine("Connected");
            }
        }
    }
}
