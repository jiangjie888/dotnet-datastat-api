using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSite.MVC.CommonSuport.ViewModel
{
    /// <summary>
    /// EXT提交后台后的返回类型
    /// </summary>
    public class ExtResult<T>
    {
        /// <summary>
        /// 标注成功或失败
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// 标注是否认证
        /// </summary>
        public bool UnAuthorizedRequest { get; set; }

        /// <summary>
        /// 标识是否ABP
        /// </summary>
        public bool __abp { get; set; } = false;
}
}
