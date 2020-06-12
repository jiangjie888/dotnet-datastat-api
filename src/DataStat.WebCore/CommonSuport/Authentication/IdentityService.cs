using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DataStat.WebCore.CommonSuport.Authentication
{

    /// <summary>
    /// 当前登录用户信息
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;
        public IdentityService(IHttpContextAccessor context)
        {
            _context = context;
        }

        //public string MyUserId => GetClaimValue(ClaimTypes.NameIdentifier);
        //public string MyUserAccout => GetClaimValue(ClaimTypes.Sid);

        //public string Name => GetClaimValue(ClaimTypes.Name);
        //public string Email => GetClaimValue(ClaimTypes.Email);

        


        public long GetUserId()
        {
            //var userid = _context.HttpContext.User.FindFirst("id");
            var userid = GetClaimValue(ClaimTypes.NameIdentifier);
            return userid != null ? Convert.ToInt64(userid) : 0;
        }

        public string GetUserAccount()
        {
            return GetClaimValue(ClaimTypes.Sid);
        }

        public string GetUserName()
        {
            return GetClaimValue(ClaimTypes.Name);
        }

        private string GetClaimValue(string claimType)
        {
            var claimsPrincipal = _context.HttpContext.User;
            var claim = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == claimType);
            if (string.IsNullOrEmpty(claim?.Value))
                return null;

            return claim.Value;
        }
    }
}
