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
    public class TestController : WebAppControllerBase
    {
        private readonly IConfigurationRoot _appConfiguration;
        //protected readonly ClaimsPrincipal _principal;
        private readonly IIdentityService _identity;

        //protected ICompositeViewEngine viewEngine;
        //private readonly IRazorViewEngine _viewEngine;
        //private readonly IConfigurationRoot _appConfiguration;
        //IApplicationLifetime _applicationLifetime;

        private readonly IRepository<AuditLog,long> _auditLogRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TestController(IHostingEnvironment env, IIdentityService identity, IRepository<AuditLog, long> auditLogRepository)
        {
            _appConfiguration = env.GetAppConfiguration();
            _identity = identity;
            _auditLogRepository = auditLogRepository;
        }

        /// <summary>
        /// 测试异常
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<string> TestException(string v)
        {
           throw new AuthorizationException("test auth exception");
        }

        /// <summary>
        /// 第一个测试样例
        /// </summary>
        /// <param name="v">参数1</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<string> FirstTest(string v)
        {
            //throw new AuthorizationException("test auth exception");
            return "the first test demo "+ v;
        }

        /// <summary>
        /// 分页返回
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ExtGrid<TestData> FirstTest1()
        {
            ExtGrid<TestData> output = new ExtGrid<TestData>();
            List<TestData> list = new List<TestData>();
            output.TotalCount = 10;
            for (int i = 1; i <= 10; i++)
            {
                var item = new TestData() { kid = i, Name = "jiangjie-" + i, CreatedTime = System.DateTime.Now };
                list.Add(item);
            }
            output.Items = list;
            return output;
        }

        /// <summary>
        /// 直接返回DataTable
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<DataTable> FirstTest2(string v)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn() { ColumnName = "kid", DataType = typeof(System.Int32) });
            dt.Columns.Add(new DataColumn() { ColumnName = "Name", DataType = typeof(System.String) });
            dt.Columns.Add(new DataColumn() { ColumnName = "CreatedTime", DataType = typeof(System.DateTime) });

            DataRow dr;
            for (int i = 1; i <= 10; i++)
            {
                dr = dt.NewRow();
                dr["kid"] = i;
                dr["Name"] = "jiangjie-" + i;
                dr["CreatedTime"] = System.DateTime.Now;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 测试无返回值
        /// </summary>
        /// <param name="v"></param>
        [HttpPost]
        [AllowAnonymous]
        public void FirstTest3(string v)
        {
            string str = "123";
        }

        /// <summary>
        /// 第一个测试连接数据库 
        /// </summary> DBNull.Value;
        /// <returns></returns>
        [HttpPost]
        public ActionResult<DataTable> DbConnectionFirstTest(int id,string flowcode)
        {

            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings();
            connectionStringSettings.ConnectionString = _appConfiguration["db:mysql:workflow"];
            connectionStringSettings.ProviderName = _appConfiguration["db:mysql:providerName"];
            DBConnection dbConnection = new DBConnection();
            dbConnection.DefaultConnectionStringSettings = connectionStringSettings;
            var conn = dbConnection.GetDbConnection();

            MySqlParameter[] parms = new MySqlParameter[1];
            parms[0] = new MySqlParameter("P_Id", MySqlDbType.Int64);
            parms[0].Value = id;

            //parms[1] = new MySqlParameter("P_FlowCode", MySqlDbType.VarChar);
            //parms[1].Value = flowcode;

            DataTable dt = MySqlHelper.ExecuteDataset(dbConnection.DefaultConnectionStringSettings.ConnectionString, "select * from wf_flowinfo where Id=@P_Id",parms).Tables[0];
            return dt;
        }


        /// <summary>
        /// 测试数据库仓储GetAsync
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<AuditLog>> TestRepositoryGetAsync()
        {
            //ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings(_appConfiguration["db:mysql:providerName"],_appConfiguration["db:mysql:workflow"]);
            //_auditLogRepository.DefaultConnectionStringSettings = connectionStringSettings;
            return await _auditLogRepository.GetAsync(1);
        }

        /// <summary>
        /// 测试数据库仓储GetAllAsync
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<AuditLog>> TestRepositoryGetAllAsync()
        {
            return await _auditLogRepository.GetAllAsync();
        }

        /// <summary>
        /// 测试数据库仓储QueryAllbyPage
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ExtGrid<AuditLog> TestRepositoryQueryAllbyPage(int start, int limit, string sort, string dir, List<DataFilter> filters)
        {
            ExtGrid<AuditLog> result = new ExtGrid<AuditLog>();
            long total = 0;
            result.Items = _auditLogRepository.QueryAllbyPage(start, limit, sort, dir, null, out total).ToList();
            result.TotalCount = total;
            return result;
        }

        /// <summary>
        /// 测试数据库仓储InsertAsync
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task TestRepositoryInsertAsync()
        {
            AuditLog entity = new AuditLog()
            {
                SystemCode = "Stat001",
                TenantId = 1,
                UserId = 1,
                ServiceName = "Test",
                MethodName = "TestRepositoryInsertAsync",
                Parameters = "{fdafdafd}",
                ExecutionTime = System.DateTime.Now,
                ExecutionDuration = 22,
                ClientIpAddress = "172.16.1.3",
                ClientName = "ClientName",
                BrowserInfo = "BrowserInfo",
                Exception = "Exception",
                ImpersonatorUserId = null,
                ImpersonatorTenantId = 1,
                CustomData = ""
            };
            await _auditLogRepository.InsertAsync(entity);
        }

        /// <summary>
        /// 第一个测试Token认证
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<string> FirstTestAuth(string v)
        {
            return "the first test demo account=" + v + _identity.GetUserAccount();
        }

        #region add by jjie
        #endregion
    }
}
