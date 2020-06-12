using System;
using System.Collections.Generic;
using System.Text;

namespace DataStat.WebCore.CommonSuport.ViewModel
{
    /// <summary>
    /// 表格返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtGrid<T>
    {
        public long TotalCount { set; get; }

        public List<T> Items { set; get; }
    }
}
