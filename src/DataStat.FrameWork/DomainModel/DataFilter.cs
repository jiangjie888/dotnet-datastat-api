using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStat.FrameWork.DomainModel
{
    public class DataFilter
    {
        public string type { get; set; }
        public string value { get; set; }
        public string field { get; set; }
        public string comparison { get; set; }
    }
}
