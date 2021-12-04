using System;
using System.Data;
using System.Web.Http;
using System.Collections.Generic;
using System.Reflection;
using AES_TeamTool.Models;
using AES_TeamTool.Utils;

namespace AES_TeamTool.Controllers
{
    [RoutePrefix("change-manage")]
    public class ChangeManageController : ApiController
    {
        [HttpPost]
        [Route("record")]
        public ResponseBody RecordChangeItem([FromBody] ChangeManagement reqbody)
        {
            try
            {
#if DEBUG
                string recordDbStr = ValueQuery.GetAppSetting("devSqlConnDC");
#endif
#if (!DEBUG)
                string recordDbStr = DynamicValueQuery.GetAppSetting("prodSqlConnDC");
#endif
                string recordTableName = ValueQuery.GetAppSetting("changeManageTableName");

                List<string> colsName = new List<string>();

                PropertyInfo[] props = typeof(ChangeManagement).GetProperties();

                foreach (PropertyInfo prop in props) { colsName.Add(prop.Name); }

                List<string> colsValue = new List<string>() {
                    reqbody.ReleaseDate, reqbody.Type.ToString(), reqbody.ItemName,reqbody.Profiles, reqbody.Description,
                    reqbody.SpecAttached, reqbody.InvolvedModule, reqbody.Plant.ToString() ,reqbody.Requester, reqbody.ImpactedTool,
                    reqbody.Team.ToString(), reqbody.Developer,reqbody.Comment,  reqbody.Mark, reqbody.StableVersion,reqbody.ConfirmStatus
                };

                bool isRecordDone = new SqlHelper(recordDbStr).InsertDataLine(recordTableName, colsName, colsValue);

                bool isSendMail = new MailHelper().SyncSendMail(
                        new List<string>() { },
                        new List<string>() { $"{reqbody.Developer.Replace(' ', '.')}@cn.ats.net" },
                        MailBuilder(colsName, colsValue)
                    );

                return new ResponseBody().SetResponse(
                    (int)ResultCode.SUCCESS,
                    ValueQuery.GetDescription(ResultCode.SUCCESS),
                    "success",
                    null
                );
            }
            catch (Exception err)
            {
                return new ResponseBody().SetResponse(
                    (int)ResultCode.OTHER_CUSTOM_CODE,
                    ValueQuery.GetDescription(ResultCode.OTHER_CUSTOM_CODE),
                    null,
                    err.Message
                );
            }
        }

        [HttpPost]
        [Route("query")]
        public ResponseBody QueryChangeItem([FromBody] ChangeManagement reqbody)
        {
            try
            {
#if DEBUG
                string recordDbStr = ValueQuery.GetAppSetting("devSqlConnDC");
#endif
#if (!DEBUG)
                  string recordDbStr = DynamicValueQuery.GetAppSetting("prodSqlConnDC");
#endif
                string recordTableName = ValueQuery.GetAppSetting("changeManageTableName");

                string builtCondition = string.Empty;

                builtCondition += string.IsNullOrWhiteSpace(reqbody.Type.ToString()) ? null : $"Type = '{reqbody.Type}'";
                builtCondition += string.IsNullOrWhiteSpace(reqbody.ReleaseDate) ? null : $" and ReleaseDate = '{reqbody.ReleaseDate}'";
                builtCondition += string.IsNullOrWhiteSpace(reqbody.ItemName) ? null : $" and ItemName = '{reqbody.ItemName}'";
                builtCondition += string.IsNullOrWhiteSpace(reqbody.InvolvedModule) ? null : $" and InvolvedModule = '{reqbody.InvolvedModule}'";
                builtCondition += string.IsNullOrWhiteSpace(reqbody.Plant.ToString()) ? null : $" and Plant = '{reqbody.Plant}'";
                builtCondition += string.IsNullOrWhiteSpace(reqbody.Requester) ? null : $" and Point = '{reqbody.Requester}'";
                builtCondition += string.IsNullOrWhiteSpace(reqbody.Team.ToString()) ? null : $" and Team = '{reqbody.Team}'";
                builtCondition += string.IsNullOrWhiteSpace(reqbody.Developer) ? null : $" and Developer = '{reqbody.Developer}'";
                builtCondition += string.IsNullOrWhiteSpace(reqbody.ConfirmStatus) ? null : $" and ConfirmStatus = '{reqbody.ConfirmStatus}'";

                DataTable queriedData = new SqlHelper(recordDbStr).CommonQuery(recordTableName, "*", builtCondition);

                return new ResponseBody().SetResponse(
                    (int)ResultCode.SUCCESS,
                    ValueQuery.GetDescription(ResultCode.SUCCESS),
                    queriedData,
                    null
                );

            }
            catch (Exception err)
            {
                return new ResponseBody().SetResponse(
                    (int)ResultCode.OTHER_CUSTOM_CODE,
                    ValueQuery.GetDescription(ResultCode.OTHER_CUSTOM_CODE),
                    null,
                    err.Message
                );
            }
        }

        [NonAction]
        private string MailBuilder(List<string> keys, List<string> vals)
        {
            string trows = string.Empty;

            if (keys.Count != 0 && keys.Count != vals.Count) { return null; }

            for (int i = 0; i < keys.Count; i++)
            {
                trows += $@"<tr> <td><strong>{keys[i]}: </strong></td> <td>{vals[i]}</td> </tr>";
            }

            string tbody = $"<table>{trows}</table>";

            return tbody;
        }

        private enum MailDirection
        {
            Horizontal = 1,

            Vertical = 2,
        }
    }
}