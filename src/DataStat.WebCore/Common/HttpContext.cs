﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataStat.WebCore.Common
{
    /// <summary>
    /// 获取HttpContext
    /// </summary>
    public static class HttpContext
    {
        public static IServiceProvider ServiceProvider;

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor));
                Microsoft.AspNetCore.Http.HttpContext context = ((Microsoft.AspNetCore.Http.HttpContextAccessor)factory).HttpContext;
                return context;
            }
        }

    }
}
