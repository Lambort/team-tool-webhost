using System;
using System.ComponentModel;
using AES_TeamTool.Utils;

namespace AES_TeamTool.Models
{
    public class ResponseBody
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object Error { get; set; }

        public ResponseBody SetResponse(int code, string message, object data, object error)
        {
            Code = code;
            Message = message;
            Data = data;
            Error = error;
            return this;
        }

        public ResponseBody SetResponse(int code, object data, object error = null)
        {
            Code = code;
            Message = ValueQuery.GetDescription((ResultCode)Enum.Parse(typeof(ResultCode), code.ToString()));
            Data = data;
            Error = error;
            return this;
        }
    }

    public enum ResultCode
    {
        [Description("request data success")]
        SUCCESS = 200,

        [Description("request failed")]
        FAIL = 400,

        [Description("request error, method refuse")]
        UNAUTHORIZED = 403,

        [Description("request failed, cannot find data")]
        NOT_FOUND = 404,

        [Description("server internal error")]
        INTERNAL_SERVER_ERROR = 500,

        [Description("custom type error")]
        OTHER_CUSTOM_CODE = 409
    }
}