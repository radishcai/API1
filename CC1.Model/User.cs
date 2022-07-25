using SqlSugar;
using System;

namespace CC1.Model
{
    public class User:BaseId
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [SugarColumn(ColumnDataType ="nvarchar(30)")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>

        [SugarColumn(ColumnDataType = "nvarchar(100)")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>

        [SugarColumn(ColumnDataType = "nvarchar(100)")]
        public string UserPwd { get; set; }

        /// <summary>
        /// 手机
        /// </summary>

        [SugarColumn(ColumnDataType = "nvarchar(50)")]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>

        [SugarColumn(ColumnDataType = "nvarchar(100)")]
        public string Email { get; set; }

        /// <summary>
        /// 性别
        /// </summary>

        [SugarColumn(ColumnDataType = "nvarchar(2)")]
        public string Sex { get; set; }

        /// <summary>
        /// 是否禁用 1是 0否
        /// </summary>
        public int IsValid { get; set; }

        /// <summary>
        /// 是否管理员 1是 0否
        /// </summary>
        public int IsAdmin { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginNum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Create { get; set; }
    }
}
