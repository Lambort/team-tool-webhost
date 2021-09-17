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
    public class ChangeManagementController : ApiController
    {
        [HttpPost]
        [Route("record")]
        public BaseResponseBody RecordChangeItem([FromBody] ChangeManagement reqbody)
        {
            try
            {
#if DEBUG
                string recordDbStr = DynamicValueQuery.GetAppSetting("devSqlConnDC");
#endif
#if (!DEBUG)
                string recordDbStr = DynamicValueQuery.GetAppSetting("prodSqlConnDC");
#endif
                string recordTableName = DynamicValueQuery.GetAppSetting("changeManageTableName");

                List<string> colsName = new List<string>();

                PropertyInfo[] props = typeof(ChangeManagement).GetProperties();

                foreach (PropertyInfo prop in props) { colsName.Add(prop.Name); }

                List<string> colsValue = new List<string>() {
                    reqbody.ReleaseDate, reqbody.Type.ToString(), reqbody.ItemName,reqbody.Profiles, reqbody.Description,
                    reqbody.SpecAttached, reqbody.InvolvedModule, reqbody.Plant.ToString() ,reqbody.Requester, reqbody.ImpactedTool,
                    reqbody.Team.ToString(), reqbody.Developer,reqbody.Comment,  reqbody.Mark, reqbody.StableVersion,reqbody.ConfirmStatus
                };

                bool isRecordDone = new BaseSQLHandler(recordDbStr).InsertDataLine(recordTableName, colsName, colsValue);

                bool isSendMail = new BaseMailHandler().SyncSendMail(
                        new List<string>() { },
                        new List<string>() { $"{reqbody.Developer.Replace(' ', '.')}@cn.ats.net" },
                        MailBuilder(colsName, colsValue)
                    );

                return new BaseResponseBody().SetResponse(
                    (int)ResultCode.SUCCESS,
                    DynamicValueQuery.GetDescription(ResultCode.SUCCESS),
                    "success",
                    null
                );
            }
            catch (Exception err)
            {
                return new BaseResponseBody().SetResponse(
                    (int)ResultCode.OTHER_CUSTOM_CODE,
                    DynamicValueQuery.GetDescription(ResultCode.OTHER_CUSTOM_CODE),
                    null,
                    err.Message
                );
            }
        }

        [HttpPost]
        [Route("query")]
        public BaseResponseBody QueryChangeItem([FromBody] ChangeManagement reqbody)
        {
            try
            {
#if DEBUG
                string recordDbStr = DynamicValueQuery.GetAppSetting("devSqlConnDC");
#endif
#if (!DEBUG)
                string recordDbStr = DynamicValueQuery.GetAppSetting("prodSqlConnDC");
#endif
                string recordTableName = DynamicValueQuery.GetAppSetting("changeManageTableName");

                string builtCondition = string.Empty;
                builtCondition += reqbody.Type.ToString() == null || reqbody.Type.ToString() == string.Empty ? null : $"Type = '{reqbody.Type.ToString()}'";
                builtCondition += reqbody.ReleaseDate == null || reqbody.ReleaseDate == string.Empty ? null : $" and ReleaseDate = '{reqbody.ReleaseDate}'";
                builtCondition += reqbody.ItemName == null || reqbody.ItemName == string.Empty ? null : $" and ItemName = '{reqbody.ItemName}'";
                builtCondition += reqbody.InvolvedModule == null || reqbody.InvolvedModule == string.Empty ? null : $" and InvolvedModule = '{reqbody.InvolvedModule}'";
                builtCondition += reqbody.Plant.ToString() == null || reqbody.Plant.ToString() == string.Empty ? null : $" and Plant = '{reqbody.Plant.ToString()}'";
                builtCondition += reqbody.Requester == null || reqbody.Requester == string.Empty ? null : $" and Point = '{reqbody.Requester}'";
                builtCondition += reqbody.Team.ToString() == null || reqbody.Team.ToString() == string.Empty ? null : $" and Team = '{reqbody.Team.ToString()}'";
                builtCondition += reqbody.Developer == null || reqbody.Developer == string.Empty ? null : $" and Developer = '{reqbody.Developer}'";
                builtCondition += reqbody.ConfirmStatus == null || reqbody.ConfirmStatus == string.Empty ? null : $" and ConfirmStatus = '{reqbody.ConfirmStatus}'";

                DataTable queriedData = new BaseSQLHandler(recordDbStr).CommonQuery(recordTableName, "*", builtCondition);

                return new BaseResponseBody().SetResponse(
                    (int)ResultCode.SUCCESS,
                    DynamicValueQuery.GetDescription(ResultCode.SUCCESS),
                    queriedData,
                    null
                );

            }
            catch (Exception err)
            {
                return new BaseResponseBody().SetResponse(
                    (int)ResultCode.OTHER_CUSTOM_CODE,
                    DynamicValueQuery.GetDescription(ResultCode.OTHER_CUSTOM_CODE),
                    null,
                    err.Message
                );
            }
        }

        [NonAction]
        private string MailBuilder(List<string> keys, List<string> vals, MailDirection direction = MailDirection.Horizontal)
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