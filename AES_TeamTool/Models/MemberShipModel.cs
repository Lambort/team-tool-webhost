using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AES_TeamTool.Models
{
    public class LoginInfo
    {
        public Domain Domain { get; set; }
        public string UserName { get; set; }
        public string EmployeeID { get; set; }
        public string Password { get; set; }
    }
    public class LoginBackInfo : UserInfo
    {
        public bool IsMatch { get; set; }
        public string LoginError { get; set; }
    }



    [Table("Webapp_Membership_User")]
    public class UserInfo
    {
        public Domain Domain { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeID { get; set; }
        public string CostCenter { get; set; }
        public object MailGroup { get; set; }
        public object MemberGroup { get; set; }
        public bool IsBlocked { get; set; }
    }

    [Table("Webapp_Membership_UserHistory")]
    public class UserHistory
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public UserCheck CheckType { get; set; }
        public string Modified { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Comment { get; set; }
    }




    [Table("Webapp_Membership_Role")]
    public class RoleInfo
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public bool IsBlocked { get; set; }
    }

    [Table("Webapp_Membership_RoleHistory")]
    public class RoleHistory
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public Operation OperateType { get; set; }
        public string Modified { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Comment { get; set; }
    }




    [Table("Webapp_Membership_FunctionGroup")]
    public class FunctionGroupInfo
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupIcon { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public string IsBlocked { get; set; }
    }

    [Table("Webapp_Membership_FunctionGroupHistory")]
    public class FunctionGroupHistory
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public Operation OperateType { get; set; }
        public string Modified { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Comment { get; set; }
    }




    [Table("Webapp_Membership_Function")]
    public class FunctionInfo
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionRoute { get; set; }
        public string FunctionIcon { get; set; }
        public string Description { get; set; }
        public int GroupId { get; set; }
        public string Creator { get; set; }
        public string IsBlocked { get; set; }
    }

    [Table("Webapp_Membership_FunctionHistory")]
    public class FunctionHistory
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public Operation OperateType { get; set; }
        public string Modified { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Comment { get; set; }
    }




    [Table("Webapp_Membership_MapUserRole")]
    public class MapUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Creator { get; set; }
        public bool IsBlocked { get; set; }
    }

    [Table("Webapp_Membership_MapUserRoleHistory")]
    public class MapUserRoleHistory
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public Operation OperateType { get; set; }
        public string Modified { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Comment { get; set; }
    }




    [Table("Webapp_Membership_MapRoleFunction")]
    public class MapRoleFunction
    {

        public int RoleId { get; set; }
        public int FunctionId { get; set; }
        public string Creator { get; set; }
        public bool IsBlocked { get; set; }

    }

    [Table("Webapp_Membership_MapRoleFunctionHistory")]
    public class MapRoleFunctionHistory
    {
        public int RoleId { get; set; }
        public int FunctionId { get; set; }
        public Operation OperateType { get; set; }
        public string Modified { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public string Comment { get; set; }
    }


    public enum Domain
    {
        Main = 1,
        Prod = 2
    }

    public enum Operation
    {
        Create = 1,
        Update = 2,
        Block = 3,
        Recover = 4,
    }

    public enum UserCheck
    {
        Create = 1,
        Login = 2,
        Logout = 3,
        Block = 4,
    }
}