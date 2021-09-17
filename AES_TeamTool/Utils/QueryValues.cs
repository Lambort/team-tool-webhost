using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;

namespace AES_TeamTool.Utils
{
    public static class DynamicValueQuery
    {
        public static string GetAppSetting(string appSettingKey) => ConfigurationManager.AppSettings[appSettingKey].ToString();

        public static string GetDescription(Enum enumObject)
        {
            Type type = enumObject.GetType();
            MemberInfo[] memberInfos = type.GetMember(enumObject.ToString());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                DescriptionAttribute[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    return attrs[0].Description;
                }
            }
            return enumObject.ToString();
        }
    }
}