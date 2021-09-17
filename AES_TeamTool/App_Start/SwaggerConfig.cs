//using System.Web.Http;
//using WebActivatorEx;
//using Swashbuckle.Application;
//using AES_TeamTool;

//[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

//namespace AES_TeamTool
//{
//    public class SwaggerConfig
//    {
//        public static void Register()
//        {
//            var thisAssembly = typeof(SwaggerConfig).Assembly;

//            GlobalConfiguration.Configuration
//                .EnableSwagger(c =>
//                    {
//                        c.SingleApiVersion("v1", "AES_TeamTool Project APIs");
//                        c.IncludeXmlComments(string.Format("{0}/bin/AES_TeamTool.xml", System.AppDomain.CurrentDomain.BaseDirectory));
//                    })
//                .EnableSwaggerUi(c => { });
//        }
//    }
//}
