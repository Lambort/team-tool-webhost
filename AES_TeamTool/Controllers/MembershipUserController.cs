using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Web.Http;
using AES_TeamTool.Models;
using AES_TeamTool.Utils;

namespace AES_TeamTool.Controllers
{
    [RoutePrefix("user-manage")]
    public class MembershipUserController : ApiController
    {
        [HttpPost]
        [Route("login")]
        public ResponseBody UserLogin([FromBody] LoginInfo loginInfo)
        {
            try
            {
                string LDAPPath = $@"LDAP://{loginInfo.Domain}.ats.net";
                DirectoryEntry entryInfo = new DirectoryEntry(LDAPPath, loginInfo.UserName, loginInfo.Password);
                object nativeObject = entryInfo.NativeObject;
                DirectorySearcher searcher = new DirectorySearcher(entryInfo)
                {
                    Filter = $"(samaccountname={loginInfo.UserName})"
                };
                SearchResult searchOut = searcher.FindOne();
                if (searchOut == null)
                {
                    throw new Exception("cannot find the user");
                }

                LoginBackInfo backInfo = new LoginBackInfo()
                {
                    IsMatch = true,
                    LoginError = "none",
                    UserName = !searchOut.Properties.Contains("name") ? null : searchOut.Properties["name"][0].ToString(),
                    DisplayName = !searchOut.Properties.Contains("displayname") ? null : searchOut.Properties["displayname"][0].ToString(),
                    EmployeeID = !searchOut.Properties.Contains("company") ? null : searchOut.Properties["company"][0].ToString(),
                    MailGroup = !searchOut.Properties.Contains("msexchdelegatelistbl") ? null : searchOut.Properties["msexchdelegatelistbl"],
                    MemberGroup = !searchOut.Properties.Contains("memberof") ? null : searchOut.Properties["memberof"],
                    CostCenter = !searchOut.Properties.Contains("department") ? null : searchOut.Properties["department"][0].ToString(),
                };

                string connectStr = ValueQuery.GetAppSetting("devSqlConnDC");
                SqlHelper sqlHelper = new SqlHelper(connectStr, true);
                DataTable judeUserTable = sqlHelper.CommonQuery(ValueQuery.GetTableName<UserInfo>(), "*", $"[UserName] = '{backInfo.UserName}'");
                if (judeUserTable.Rows.Count < 1)
                {
                    DataTable lastUidTable = sqlHelper.CommonQuery(ValueQuery.GetTableName<UserInfo>(), "top(1) [UserId]", null, "[CreatedOn] desc");
                    int foundLast;
                    int lastUid = lastUidTable.Rows.Count > 0 && int.TryParse(lastUidTable.Rows[0].Field<string>("UserId"), out foundLast) ? foundLast : 0;
                    bool registerResult = sqlHelper.InsertDataLine(ValueQuery.GetTableName<UserInfo>()
                        , new List<string>() { "Domain", "UserName", "UserId", "EmployeeID", "DisplayName", "CostCenter" }
                        , new List<string>() {
                            loginInfo.Domain.ToString(), backInfo.UserName, (lastUid + 1).ToString().PadLeft(6, '0'),
                            backInfo.EmployeeID ?? "public", backInfo.DisplayName, backInfo.CostCenter ?? "public"
                        }
                    );
                    if (!registerResult)
                    {
                        return new ResponseBody().SetResponse(409, new { Error = "Register mistake" }, "Register mistake");
                    }
                }
                DataTable updatedUserTable = sqlHelper.CommonQuery(ValueQuery.GetTableName<UserInfo>(), "top(1) *", $"[UserName] = '{backInfo.UserName}'");
                sqlHelper.DisposeConnection();

                return new ResponseBody().SetResponse(200, ValueQuery.GetDescription(ResultCode.SUCCESS), backInfo, null);
            }
            catch (Exception error)
            {
                LoginBackInfo findResult = new LoginBackInfo()
                {
                    IsMatch = false,
                    LoginError = error.Message,
                };
                return new ResponseBody()
                    .SetResponse(409, ValueQuery.GetDescription(ResultCode.OTHER_CUSTOM_CODE), findResult, error.Message);
            }
        }
    }
}