using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AES_TeamTool.Utils
{
    public static class ValueQuery
    {
        public static string GetAppSetting(string xmlKey) => ConfigurationManager.AppSettings[xmlKey].ToString().Trim();

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

        public static V GetAttributeValue<T, V>(this Type type, Func<T, V> slect) where T : Attribute
        {
            T attr = type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;

            return attr != null ? slect(attr) : default(V);
        }
        public static string GetTableName(Type tabletype) => tabletype.GetAttributeValue((TableAttribute attr) => attr.Name);
        public static string GetTableName<C>() => typeof(C).GetAttributeValue((TableAttribute attr) => attr.Name);

        //public static string GetTableName<T>() where T : class
        //{
        //    string tableName = string.Empty;
        //    object[] attributes = typeof(T).GetCustomAttributes(false);
        //    foreach (var attribute in attributes)
        //    {
        //        if (attribute is TableAttribute)
        //        {
        //            tableName = (attribute as TableAttribute).Name;
        //        }
        //    }
        //    return tableName;
        //}
    }
}