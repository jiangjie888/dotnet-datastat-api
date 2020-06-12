using System;
using System.Collections.Generic;
using System.Text;

namespace DataStat.WebCore.Controllers.Forest.Dto
{
    public class SynLocationDataInput
    {

        public string Type { set; get; }

        public long? UserId { set; get; }

        public string UserName { set; get; }

        public string Url { set; get; }

        public DateTime? BegDate { set; get; }

        public DateTime? EndDate { set; get; }

        
    }
}
