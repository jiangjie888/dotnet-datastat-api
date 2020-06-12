using DataStat.FrameWork.DomainModel.AuditLogs;
using DataStat.FrameWork.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace DataStat.WebCore.CommonSuport.AuditingLogs
{
    /// <summary>
    /// 日志中间件
    /// </summary>
    public class LogMiddleware
    {
        /// <summary>
        /// 管道代理对象
        /// </summary>
        private readonly RequestDelegate _next;

        private DateTime _startTime;
        
        private string url = "";
        private string controller = "";
        private string action = "";
        private string parameters = "";
        private string ipaddress = "";
        private string clientname = "";
        private string browserinfo = "";
        private string exception = "";
        private ClaimsPrincipal claimsPrincipal=null;
        private string userid = "";

        private readonly IRepository<AuditLog, long> _auditLogRepository;



        public LogMiddleware(RequestDelegate next, IRepository<AuditLog, long> auditLogRepository)
        {
            _next = next;
            _auditLogRepository = auditLogRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            _startTime = DateTime.Now;

            try
            {
                //请求Url
                url = context.Request.Host + context.Request.Path.Value + context.Request.QueryString;

                //string url = context.HttpContext.Request.Host + context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                //string method = context.Request.Method;
                controller = "";
                action = "";
                parameters = "";
                ipaddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                clientname = "";
                browserinfo = context.Request.Headers["User-Agent"];

                claimsPrincipal = context.User;
                var userid = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                //var requestMessage = context.Request.Form["RequestMessage"];
                // 后续添加了获取请求的请求体，如果在实际项目中不需要删除即可
                long contentLen = context.Request.ContentLength == null ? 0 : context.Request.ContentLength.Value;
                //if (contentLen > 0)
                //{
                //    // 读取请求体中所有内容
                //    System.IO.Stream stream = context.Request.Body;
                //    if (context.Request.Method == "POST")
                //    {
                //        stream.Position = 0;
                //    }
                //    byte[] buffer = new byte[contentLen];
                //    stream.Read(buffer, 0, buffer.Length);
                //    // 转化为字符串
                //    _requestBody = System.Text.Encoding.UTF8.GetString(buffer);
                //}
                parameters = Newtonsoft.Json.JsonConvert.SerializeObject(context.Request.HttpContext);
                await _next(context);

                //
            }
            catch (Exception e)
            {
                exception = e.Message;
            }
            finally
            {
                AuditLog entitylog = new AuditLog();
                entitylog.SystemCode = "Stat001";
                entitylog.TenantId = 1;
                entitylog.UserId = string.IsNullOrEmpty(userid)==false ? Convert.ToInt64(userid) : 0;
                entitylog.ServiceName = entitylog.ClipString(controller, AuditLog.MaxServiceNameLength);
                entitylog.MethodName = entitylog.ClipString(action, AuditLog.MaxMethodNameLength);
                entitylog.Parameters = entitylog.ClipString(parameters, AuditLog.MaxParametersLength);
                entitylog.ExecutionTime = System.DateTime.Now;
                entitylog.ExecutionDuration = Convert.ToInt32((DateTime.Now - _startTime).TotalMilliseconds);
                entitylog.ClientIpAddress = entitylog.ClipString(ipaddress, AuditLog.MaxClientIpAddressLength);
                entitylog.ClientName = entitylog.ClipString(clientname, AuditLog.MaxClientNameLength);
                entitylog.BrowserInfo = entitylog.ClipString(browserinfo, AuditLog.MaxBrowserInfoLength);
                entitylog.Exception = entitylog.ClipString(exception, AuditLog.MaxExceptionLength);
                entitylog.ImpersonatorUserId = null;
                entitylog.ImpersonatorTenantId = 1;
                entitylog.CustomData = entitylog.ClipString(url, AuditLog.MaxCustomDataLength);
                await _auditLogRepository.InsertAsync(entitylog);
            }
            await _next(context);

        }


    }
}