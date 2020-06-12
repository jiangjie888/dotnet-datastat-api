using DataStat.FrameWork.DomainModel.AuditLogs;
using DataStat.FrameWork.Repository;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataStat.WebCore.CommonSuport.Filter
{

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class LogFilter : IAsyncActionFilter
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRepository<AuditLog, long> _auditLogRepository;

        /// <summary>
        /// 请求体中的所有值
        /// </summary>
        private string _requestBody { get; set; }

        private Stopwatch _stopwatch { get; set; }

        private string url { get; set; }
        private string controller { get; set; }
        private string action { get; set; }
        private string parameters { get; set; }
        private string ipaddress { get; set; }
        private string clientname { get; set; }
        private string browserinfo { get; set; }
        private string exception { get; set; }

        private ClaimsPrincipal claimsPrincipal { get; set; } = null;

        private string userid { get; set; }


        public LogFilter(ILoggerFactory loggerFactory, IRepository<AuditLog, long> auditLogRepository)
        {
            _loggerFactory = loggerFactory;
            _auditLogRepository = auditLogRepository;
            //_logFlag = logFlag;
        }

        /// <summary>
        /// 执行器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            #region
            //base.OnActionExecuting(context);

            // 后续添加了获取请求的请求体，如果在实际项目中不需要删除即可
            //long contentLen = context.HttpContext.Request.ContentLength == null ? 0 : context.HttpContext.Request.ContentLength.Value;
            //if (contentLen > 0)
            //{
            //    // 读取请求体中所有内容
            //    System.IO.Stream stream = context.HttpContext.Request.Body;
            //    if (context.HttpContext.Request.Method == "POST")
            //    {
            //        stream.Position = 0;
            //    }
            //    byte[] buffer = new byte[contentLen];
            //    stream.Read(buffer, 0, buffer.Length);
            //    // 转化为字符串
            //    _requestBody = System.Text.Encoding.UTF8.GetString(buffer);
            //}
            #endregion

            //请求Url
            url = context.HttpContext.Request.Host + context.HttpContext.Request.Path.Value + context.HttpContext.Request.QueryString;

            //string url = context.HttpContext.Request.Host + context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            //string method = context.Request.Method;
            controller = context.RouteData.Values["controller"].ToString();
            action = context.RouteData.Values["action"].ToString();
            parameters = Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments);
            ipaddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            clientname = "";
            browserinfo = context.HttpContext.Request.Headers["User-Agent"];

            claimsPrincipal = context.HttpContext.User;
            var userid = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            //_stopwatch = new Stopwatch();
            //_stopwatch.Start();
            // 开始性能计数
            var stopwatch = Stopwatch.StartNew();


            try
            {
                // 尝试调用接口方法
                var result = await next();

                // 产生异常之后，将其异常信息存放在审计信息之中
                if (result.Exception != null && !result.ExceptionHandled)
                {
                    exception = result.Exception.Message;
                }
            }
            catch (Exception e)
            {
                // 产生异常之后，将其异常信息存放在审计信息之中
                exception = e.Message;
                throw;
            }
            finally
            {
                // 停止计数，并且存储审计信息
                stopwatch.Stop();

                AuditLog entitylog = new AuditLog();
                entitylog.SystemCode = "Stat001";
                entitylog.TenantId = 1;
                entitylog.UserId = string.IsNullOrEmpty(userid) == false ? Convert.ToInt64(userid) : 0;
                entitylog.ServiceName = entitylog.ClipString(controller, AuditLog.MaxServiceNameLength);
                entitylog.MethodName = entitylog.ClipString(action, AuditLog.MaxMethodNameLength);
                entitylog.Parameters = entitylog.ClipString(parameters, AuditLog.MaxParametersLength);
                entitylog.ExecutionTime = System.DateTime.Now;
                entitylog.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                entitylog.ClientIpAddress = entitylog.ClipString(ipaddress, AuditLog.MaxClientIpAddressLength);
                entitylog.ClientName = entitylog.ClipString(clientname, AuditLog.MaxClientNameLength);
                entitylog.BrowserInfo = entitylog.ClipString(browserinfo, AuditLog.MaxBrowserInfoLength);
                entitylog.Exception = exception==null ? null : entitylog.ClipString(exception, AuditLog.MaxExceptionLength);
                entitylog.ImpersonatorUserId = null;
                entitylog.ImpersonatorTenantId = 1;
                entitylog.CustomData = entitylog.ClipString(url, AuditLog.MaxCustomDataLength);
                await _auditLogRepository.InsertAsync(entitylog);
            }
        }

        #region
        //public override void OnActionExecuted(ActionExecutedContext context)
        //{
        //    base.OnActionExecuted(context);

        //    _stopwatch.Stop();
        //    //dynamic result = context.Result.GetType().Name == "EmptyResult" ? new { Value = "无返回结果" } : context.Result as dynamic;
        //    try
        //    {
        //        AuditLog entitylog = new AuditLog();
        //        entitylog.SystemCode = "Stat001";
        //        entitylog.TenantId = 1;
        //        entitylog.UserId = string.IsNullOrEmpty(userid) == false ? Convert.ToInt64(userid) : 0;
        //        entitylog.ServiceName = entitylog.ClipString(controller, AuditLog.MaxServiceNameLength);
        //        entitylog.MethodName = entitylog.ClipString(action, AuditLog.MaxMethodNameLength);
        //        entitylog.Parameters = entitylog.ClipString(parameters, AuditLog.MaxParametersLength);
        //        entitylog.ExecutionTime = System.DateTime.Now;
        //        entitylog.ExecutionDuration = Convert.ToInt32(Stopwatch.Elapsed.TotalMilliseconds);
        //        entitylog.ClientIpAddress = entitylog.ClipString(ipaddress, AuditLog.MaxClientIpAddressLength);
        //        entitylog.ClientName = entitylog.ClipString(clientname, AuditLog.MaxClientNameLength);
        //        entitylog.BrowserInfo = entitylog.ClipString(browserinfo, AuditLog.MaxBrowserInfoLength);
        //        entitylog.Exception = entitylog.ClipString(exception, AuditLog.MaxExceptionLength);
        //        entitylog.ImpersonatorUserId = null;
        //        entitylog.ImpersonatorTenantId = 1;
        //        entitylog.CustomData = entitylog.ClipString(url, AuditLog.MaxCustomDataLength);
        //        _auditLogRepository.InsertAsync(entitylog);
        //    }
        //    catch (Exception e)
        //    {
        //        var logger = _loggerFactory.CreateLogger(context.Exception.TargetSite.ReflectedType);
        //        logger.LogError(e.Message);
        //    }
        //}

        //    Logger.Log.Info($"\n 方法：{LogFlag} \n " +
        //        $"地址：{url} \n " +
        //        $"方式：{method} \n " +
        //        $"请求体：{RequestBody} \n " +
        //        $"参数：{qs}\n " +
        //        $"结果：{res}\n " +
        //        $"耗时：{Stopwatch.Elapsed.TotalMilliseconds} 毫秒（指控制器内对应方法执行完毕的时间）");

        //}
        #endregion

    }
}
