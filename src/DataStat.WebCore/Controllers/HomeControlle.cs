using DataStat.DBUtil;
using DataStat.FrameWork.DomainModel;
using DataStat.FrameWork.DomainModel.AuditLogs;
using DataStat.FrameWork.Repository;
using DataStat.WebCore.CommonSuport.Authentication;
using DataStat.WebCore.CommonSuport.ExtException;
using DataStat.WebCore.CommonSuport.ViewModel;
using DataStat.WebCore.Configuration;
using DataStat.WebCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataStat.WebCore.Controllers
{
    /// <summary>
    /// 测试控制器
    /// </summary>
    [Route("[controller]/[action]")]
    public class HomeControlle : WebAppControllerBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public HomeControlle()
        {

        }

        /// <summary>
        /// 系统首页
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View("/index.html");
        }
    }
}
