using DataStat.FrameWork.DomainModel.AuditLogs;
using DataStat.FrameWork.Repository;
using DataStat.WebCore.CommonSuport.ExtException;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using WebSite.MVC.CommonSuport.ViewModel;

namespace DataStat.WebCore.CommonSuport.Filter
{
    /// <summary>
    /// 拦截Action的异常，输出Json给EXT捕获(目前loaddata类操作在JS中暂时没有处理)  ExceptionFilter
    /// </summary>
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILoggerFactory _loggerFactory;

        //readonly IHostingEnvironment _env;

        public ExceptionFilter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            
            //_env = env;
        }

        public void OnException(ExceptionContext context)
        {

            var logger = _loggerFactory.CreateLogger(context.Exception.TargetSite.ReflectedType);
            logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);


            var controllerName = context.RouteData.Values["controller"].ToString();
            var actionName = context.RouteData.Values["action"].ToString();
            var flagexec = (context.Exception.GetType() == typeof(AuthorizationException)) ? true : false;

            var extresult = new ExtResult<string>
            {
                Success = false,
                Error = context.Exception.GetBaseException().Message,
                Result = null,
                UnAuthorizedRequest = flagexec
            };
            var excResult = new JsonResult(extresult);
            context.HttpContext.Response.StatusCode = (flagexec==true) ? (int)HttpStatusCode.Unauthorized : (int)HttpStatusCode.InternalServerError;
            context.Result = excResult;
            context.ExceptionHandled = true;

        }
    }
}
