using System;
using System.Collections.Generic;
using System.Text;

namespace DataStat.WebCore.CommonSuport.Authentication
{
    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    public interface IIdentityService
    {
        long GetUserId();

        string GetUserAccount();

        string GetUserName();
    }
}
