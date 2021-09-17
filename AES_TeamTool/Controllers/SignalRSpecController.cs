using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using AES_TeamTool.SignalR;

namespace AES_TeamTool.Controllers
{
    [RoutePrefix("signalr-spec")]
    public class SignalRSpecController : ApiController
    {
        [HttpGet]
        [Route("test-push")]
        public string TestHubPush(string liveKey)
        {
            try
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<IndexHub>();
                context.Clients.Client(liveKey).liveMessage("you've recieved a test message!");
                return "success!";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

    }
}