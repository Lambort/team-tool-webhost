using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using AES_TeamTool.Models;
using AES_TeamTool.Utils;

namespace AES_TeamTool.Controllers
{
    [RoutePrefix("tv-dashboard")]
    public class TVDashBoardController : ApiController
    {
        [HttpPost]
        [Route("query")]
        public BaseResponseBody QueryTimeTravel([FromBody] ReqBody req)
        {
            try
            {
                string mornTime = $"{DateTime.Now.ToShortDateString()} 07:00:00.000";
                string eveTime = $"{DateTime.Now.ToShortDateString()} 19:00:00.000";
                string nextMornTime = $"{DateTime.Now.AddDays(1).ToShortDateString()} 07:00:00.000";

                string startTime = req.shift == SHIFT.DayShift ? mornTime : eveTime;
                string endTime = req.shift == SHIFT.DayShift ? eveTime : nextMornTime;

                //string excuQueryStr = $"exec ProductionCustomDB.[Dashboard].[SP_TV_EFEMLocalRemote] '{startTime}','{endTime}','{req.equipment}'";

                string excuQueryStr = "exec ProductionCustomDB.[Dashboard].[SP_TV_EFEMLocalRemote] '2021-08-30 07:00:00.000','2021-08-30 19:00:00.000','CCY'";

                string customDBStr = DynamicValueQuery.GetAppSetting("customDb");

                DataTable equipmentData = new BaseSQLHandler(customDBStr).CommonQuery(excuQueryStr);

                var formatData = equipmentData.AsEnumerable()
                    .Select(row => new
                    {
                        Equipment = row.Field<string>("EquipmentName"),
                        Mode = row.Field<string>("Mode"),
                        Span = Math.Round(new TimeSpan(row.Field<DateTime>("CombinTime_End").Ticks - row.Field<DateTime>("CombinTime_Start").Ticks).TotalMinutes, 3)
                    })
                    .ToList();

                return new BaseResponseBody().SetResponse(
                    (int)ResultCode.SUCCESS,
                    DynamicValueQuery.GetDescription(ResultCode.SUCCESS),
                    formatData,
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
    }

    public enum SHIFT
    {
        DayShift = 1,
        NightShift = 2
    }

    public class ReqBody
    {
        public string equipment { get; set; }
        public SHIFT shift { get; set; }
    }

}