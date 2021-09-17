using System.Web.Http;

namespace AES_TeamTool.Controllers
{
    [RoutePrefix("router-spec")]
    public class RouterSpecController : ApiController
    {

        [HttpGet]
        [Route("test-get")]
        public string TestGet(int uid)
        {
            return $"get {uid}";
        }

        [HttpPost]
        [Route("test-post")]
        public object TestPost([FromBody] RequsetBody req)
        {
            return new { Data = "data", Message = "message" };
        }

    }

    public class RequsetBody
    {
        public object Data { get; set; }
    }
}