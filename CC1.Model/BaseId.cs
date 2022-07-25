using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace CC1.Model
{
    /// <summary>
    /// 基础模型类
    /// </summary>
    public class BaseId
    {
        /// <summary>
        /// 主键ID，自动增长
        /// </summary>
        [SugarColumn(IsIdentity  =true,IsPrimaryKey =true)]
        public int Id { get; set; }
    }
}
