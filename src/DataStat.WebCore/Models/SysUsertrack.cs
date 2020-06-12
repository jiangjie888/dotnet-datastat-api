using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataStat.WebCore.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Table("sys_usertrack")]
    public class SysUsertrack
    {
        public long Id { set; get; }

        public string Type { set; get; }
        public string DeviceId { set; get; }
        public string Lat { set; get; }
        public string Lon { set; get; }
        public string Loc { set; get; }
        public DateTime Time { set; get; }
        public long UserId { set; get; }
        public string UserName { set; get; }

    }
}
