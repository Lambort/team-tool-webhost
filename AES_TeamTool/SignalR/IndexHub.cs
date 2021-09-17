using AES_TeamTool.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;

namespace AES_TeamTool.SignalR
{
    [HubName("indexHub")]
    public class IndexHub : Hub
    {
        [HubMethodName("broadcastMessage")]
        public static void BroadcastMessage(object messageBody)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<IndexHub>();
            context.Clients.All.liveMessage(messageBody);
        }

        [HubMethodName("groupMessage")]
        public static void BroadcastMessage(List<string> clientList, object messageBody)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<IndexHub>();
            context.Clients.Clients(clientList).liveMessage(messageBody);
        }
    }
}