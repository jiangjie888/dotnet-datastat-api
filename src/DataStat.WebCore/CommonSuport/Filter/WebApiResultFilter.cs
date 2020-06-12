using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebSite.MVC.CommonSuport.ViewModel;

public class WebApiResultFilter : ResultFilterAttribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        //根据实际需求进行具体实现
        var extresult = new ExtResult<object>();

        if (context.Result is ObjectResult)
        {
            var objectResult = context.Result as ObjectResult;
            
            if (objectResult.Value == null)
            {
                extresult.Success = true;
                extresult.Error = null;
                extresult.Result = null;
                extresult.UnAuthorizedRequest = true;
                //context.Result = new ObjectResult(new { code = 404, sub_msg = "未找到资源", msg = "" });
            }
            else
            {
                extresult.Success = true;
                extresult.Error = null;
                extresult.Result = objectResult.Value;
                extresult.UnAuthorizedRequest = true;
                //context.Result = new ObjectResult(new { code = 200, msg = "", result = objectResult.Value });
            }
        }
        else if (context.Result is EmptyResult)
        {
            extresult.Success = true;
            extresult.Error = null;
            extresult.Result = null;
            extresult.UnAuthorizedRequest = true;
        }
        else if (context.Result is ContentResult)
        {
            extresult.Success = true;
            extresult.Error = null;
            extresult.Result = (context.Result as ContentResult).Content;
            extresult.UnAuthorizedRequest = true;
        }
        else if (context.Result is StatusCodeResult)
        {
            extresult.Success = true;
            extresult.Error = null;
            extresult.Result = (context.Result as StatusCodeResult).StatusCode;
            extresult.UnAuthorizedRequest = true;
        }
        var excResult = new JsonResult(extresult);
        context.Result = excResult;
    }
}