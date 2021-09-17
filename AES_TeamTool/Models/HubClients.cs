using System.Collections.Generic;

namespace AES_TeamTool.Models
{
    public static class HubClients
    {
        static HubClients()
        {
            CommonUsers = new List<string>() { };
            AdminUsers = new List<string>() { };
        }

        public static List<string> CommonUsers;
        public static List<string> AdminUsers;

        public static void AddUserUniKey(UserGroup group, string uniKey)
        {
            switch (group)
            {
                case UserGroup.Common:
                    {
                        CommonUsers.Add(uniKey);
                        break;
                    }
                case UserGroup.Admin:
                    {
                        AdminUsers.Add(uniKey);
                        break;
                    }
                default: { break; }
            }
        }

        public static void RemoveUserUniKey(UserGroup group, string uniKey)
        {
            switch (group)
            {
                case UserGroup.Common:
                    {
                        CommonUsers.RemoveAll(element => element == uniKey);
                        break;
                    }
                case UserGroup.Admin:
                    {
                        AdminUsers.RemoveAll(element => element == uniKey);
                        break;
                    }
                default: { break; }
            }
        }
    }

    public enum UserGroup
    {
        Common = 1,
        Admin = 2,
    }
}