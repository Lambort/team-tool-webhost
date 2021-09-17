using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.AspNet.SignalR;
using System.Web.Http;
using System.Net.Http.Formatting;
using Swashbuckle.Application;
using Newtonsoft.Json.Serialization;

[assembly: OwinStartup(typeof(AES_TeamTool.App_Start.WebConfig))]

namespace AES_TeamTool.App_Start
{
    public class WebConfig
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration appConfig = new HttpConfiguration();

            appConfig.MapHttpAttributeRoutes();

            appConfig.Routes.MapHttpRoute(
                name: "IndexRouter",
                routeTemplate: "aes/{controller}/{action}",
                defaults: new { }
            );

            appConfig
                .EnableSwagger(c => c.SingleApiVersion("v1", "AES Team Tool APIs"))
                .EnableSwaggerUi();

            appConfig.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            appConfig.Formatters.JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };

            appConfig.Formatters.Add(new JsonMediaTypeFormatter());

            appBuilder.UseCors(CorsOptions.AllowAll);

            appBuilder.MapSignalR("/signalr", new HubConfiguration());

            appBuilder.UseWebApi(appConfig);
        }
    }
}