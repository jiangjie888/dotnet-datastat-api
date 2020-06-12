using DataStat.FrameWork.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataStat.WebCore.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Table("sys_user")]
    public class SysUser : Entity<long>
    {
        public string UserAccount { set; get; }
        public string UserName { set; get; }
        public string UesrRelId { set; get; }
    }
}
