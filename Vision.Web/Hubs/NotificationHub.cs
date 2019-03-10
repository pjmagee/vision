namespace Vision.Web.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            this.logger = logger;
            this.logger.LogInformation("NotificationHub Created");
        }

        public override async Task OnConnectedAsync()
        {
            logger.LogInformation("New Connection from Client");
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            logger.LogInformation(exception, "Disconnected");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
