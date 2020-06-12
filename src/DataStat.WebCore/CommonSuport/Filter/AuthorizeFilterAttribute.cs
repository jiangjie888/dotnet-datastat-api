using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using DataStat.WebCore.Common;
using DataStat.WebCore.CommonSuport.ExtException;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebSite.MVC.CommonSuport.ViewModel;

namespace DataStat.WebCore.CommonSuport.Authorization
{
    /// <summary>
    /// 权限过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {

        //private readonly string _syscode = System.Configuration.ConfigurationManager.AppSettings["_syscode"];
        //private readonly AppSettingsCfg _appsettings;
        //private readonly ISysPermissionService _permissionService;

        //public ILocalizationManager LocalizationManager { get; set; }




        //public MyAuthorizeFilterAttribute(ISysPermissionService permissionService,
        //                                  AppSettingsCfg appsettings)
        //{
        //    //LocalizationManager = NullLocalizationManager.Instance;
        //    _permissionService = permissionService;
        //    _appsettings = appsettings;
        //}

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            #region 优先排除不需要认证登录的属性
            MethodInfo methodinfo = (filterContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo;
            Type mtype = filterContext.ActionDescriptor.GetType();
            //匿名访问，直接返回
            if (ReflectionHelper.GetAttributesOfMemberAndType(methodinfo, mtype).OfType<AllowAnonymousAttribute>().Any()) return;
            if (ReflectionHelper.GetAttributesOfMemberAndType(methodinfo, mtype).OfType<IAllowAnonymous>().Any()) return;
            //if (ReflectionHelper.GetAttributesOfMemberAndType(methodinfo, mtype).OfType<DisableAuditing>().Any()) return;
            //var methodCustomAttributes = methodinfo.GetCustomAttributes(true).ToList(); //获得所有自定义的attributes标记
            #endregion

            var path = filterContext.HttpContext.Request.Path.ToString().ToLower();
            //var isViewPage = false;//当前Action请求是否为具体的功能页


            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //string token = filterContext.HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();

                //string cliamOrg = token.Split(".")[1];
                //string re = JsonWebToken.Decode(token, "", false);

                //if ((filterContext.HttpContext.User.Claims.Count() > 0))
                //{

                //}
                //filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                throw new AuthorizationException("认证失败：你的登录信息不存在或是过期,请重新登录");
                //var extresult = new ExtResult<string>
                //{
                //    Success = false,
                //    Error = "认证失败：你的登录信息不存在或是过期,请重新登录",
                //    Result = null,
                //    UnAuthorizedRequest = true
                //};
                //var excResult = new JsonResult(extresult);
                //filterContext.Result = excResult;
            }
            else
            {
                //根据验证判断进行处理
                //this.AuthorizeCore(filterContext, isViewPage);
                return;
            }

        }
    }
}
