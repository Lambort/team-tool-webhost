using System;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using AES_TeamTool.SignalR;
using AES_TeamTool.Models;
using AES_TeamTool.Utils;

namespace AES_TeamTool.Controllers
{
    [RoutePrefix("signalr-test")]
    public class UnitTestController : ApiController
    {
        [HttpGet]
        [Route("test-signalr-push")]
        public object TestHubPush(string liveKey)
        {
            try
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<IndexHub>();
                context.Clients.Client(liveKey).liveMessage("you've recieved a test message!");
                return "success";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        [HttpGet]
        [Route("test-http-context")]
        public object TesHttpContext()
        {
            try
            {
                string[] val = HttpContext.Current.Request.ServerVariables.AllKeys;
                List<Temp> list = new List<Temp>();
                foreach (string sub in val)
                {
                    list.Add(new Temp { key = sub, val = HttpContext.Current.Request.ServerVariables[sub] });
                }
                return list;
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        [HttpGet]
        [Route("test-get-attr")]
        public ResponseBody TestCustomAttributes()
        {
            try
            {
            string attrName = ValueQuery.GetTableName<UserInfo>();
                return new ResponseBody().SetResponse(200, new { Name = attrName });
            }
            catch(Exception err)
            {
                return new ResponseBody().SetResponse(409, new { Error= err}, err.Message);
            }
        }

    }

    public class Temp
    {
        public string key { get; set; }
        public string val { get; set; }
    }
}