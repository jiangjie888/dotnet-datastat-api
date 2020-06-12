

using DataStat.WebCore.CommonSuport.Authorization;
using DataStat.WebCore.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Claims;

namespace DataStat.WebCore.Controllers
{
    /// <summary>
    /// »ù¿ØÖÆÆ÷
    /// </summary>
    //[AuthorizeFilter]
    public abstract class WebAppControllerBase : Controller
    {



        //protected void CheckErrors(IdentityResult identityResult)
        //{
        //    identityResult.CheckErrors(LocalizationManager);
        //}.

        //public string MyUserId => GetClaimValue(ClaimTypes.NameIdentifier);
        //public string MyUserAccout => GetClaimValue(ClaimTypes.Sid);

        //public string Name => GetClaimValue(ClaimTypes.Name);
        //public string Email => GetClaimValue(ClaimTypes.Email);

        //private string GetClaimValue(string claimType)
        //{
        //    var claimsPrincipal = _principal;

        //    var claim = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == claimType);
        //    if (string.IsNullOrEmpty(claim?.Value))
        //        return null;

        //    return claim.Value;
        //}
    }
}
