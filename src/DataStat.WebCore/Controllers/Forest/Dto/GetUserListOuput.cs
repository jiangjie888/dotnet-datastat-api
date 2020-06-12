using DataStat.FrameWork.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStat.WebCore.Controllers.Forest.Dto
{
    public class GetUserListOuput: Entity<long>
    {
        public string UserAccount { set; get; }
        public string UserName { set; get; }
        public string UesrRelId { set; get; }

        public string Lon { set; get; }

        public string Lat { set; get; }
    }
}
