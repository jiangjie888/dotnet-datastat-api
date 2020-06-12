using System;
using System.Collections.Generic;
using System.Text;

namespace DataStat.WebCore.Common.Dto
{
    public class RkeyInput
    {
        /// <summary>
        /// 主表行主键
        /// </summary>
        public string Rkey { get; set; }

        /// <summary>
        /// 子表行主键
        /// </summary>
        public string RkeyDet { get; set; }

        /// <summary>
        /// 网格Id
        /// </summary>
        public string GridId { get; set; }

    }
}
