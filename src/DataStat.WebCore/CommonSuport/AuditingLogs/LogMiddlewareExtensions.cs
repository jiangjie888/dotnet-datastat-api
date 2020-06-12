using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataStat.WebCore.CommonSuport.AuditingLogs
{
    /// <summary>
    /// 这是扩展中间件
    /// </summary>
    public static class LogMiddlewareExtensions
    {
        /// <summary>
        /// 调用日志中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLogLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogMiddleware>();
        }
    }
}
