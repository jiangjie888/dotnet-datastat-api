using DataStat.DBUtil;
using DataStat.WebCore.Common.Dto;
using DataStat.WebCore.CommonSuport.Authentication;
using DataStat.WebCore.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace DataStat.WebCore.Controllers.BigData
{
    /// <summary>
    /// 大数据平台
    /// </summary>
    [Route("[controller]/[action]")]
    public class BigDataController : WebAppControllerBase
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IIdentityService _identity;
        //private readonly IRepository<AuditLog, long> _auditLogRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BigDataController(IHostingEnvironment env, IIdentityService identity)
        {
            _appConfiguration = env.GetAppConfiguration();
            _identity = identity;
        }

        #region 获取区划
        /// <summary>
        /// 根据区划Id获取下一级区划数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<DataTable> GetNextArea([FromBody] RkeyInput input)
        {
            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings();
            connectionStringSettings.ConnectionString = _appConfiguration["db:mysql:infra"];
            connectionStringSettings.ProviderName = _appConfiguration["db:mysql:providerName"];
            DBConnection dbConnection = new DBConnection();
            dbConnection.DefaultConnectionStringSettings = connectionStringSettings;


            MySqlParameter[] parms = new MySqlParameter[1];
            parms[0] = new MySqlParameter("P_AreaId", MySqlDbType.VarChar);
            parms[0].Value = input.Rkey;

            string sql = @"select a.Id,a.AreaCode,a.AreaName,a.AreaLevel,a.AreaType,a.PId from sys_area a where PId =@P_AreaId and a.IsDeleted = 0";
            DataTable dt = MySqlHelper.ExecuteDataset(dbConnection.DefaultConnectionStringSettings.ConnectionString, sql, parms).Tables[0];

            return dt;
        }

        
        /// <summary>
        /// 根据当前用户获取下一级区划数据
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<DataTable> GetAreaByUser()
        {
            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings();
            connectionStringSettings.ConnectionString = _appConfiguration["db:mysql:infra"];
            connectionStringSettings.ProviderName = _appConfiguration["db:mysql:providerName"];
            DBConnection dbConnection = new DBConnection();
            dbConnection.DefaultConnectionStringSettings = connectionStringSettings;


            MySqlParameter[] parms = new MySqlParameter[1];
            parms[0] = new MySqlParameter("P_UserId", MySqlDbType.Int64);
            parms[0].Value = _identity.GetUserId();

            var entity = MySqlHelper.ExecuteDataRow(dbConnection.DefaultConnectionStringSettings.ConnectionString, "select * from sys_areabindpeople where userid=@P_UserId and IsDeleted = 0", parms);

            string sql = @"select a.Id,a.AreaCode,a.AreaName,a.AreaLevel,a.AreaType,a.PId from sys_area a where PId = '"+ entity["AreaId"]+ "' and a.IsDeleted = 0";
            DataTable dt = MySqlHelper.ExecuteDataset(dbConnection.DefaultConnectionStringSettings.ConnectionString, sql).Tables[0];
            
            return dt;
        }
        #endregion

        /// <summary>
        /// 获取人口统计数据
        /// </summary>
        /// <param name="areacode"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<DataTable> GetPopulationStat(string areacode)
        {
            DataTable output = new DataTable();
            DBConnection dbConnection = new DBConnection();
            //MySqlParameter[] parms = new MySqlParameter[1];
            //parms[0] = new MySqlParameter("P_AreaCode", MySqlDbType.VarChar);
            //parms[0].Value = areacode;
            string sql = @"select * from t_basedataqurery where GridId like '%"+ areacode + "%'";
            DataTable data = MySqlHelper.ExecuteDataset(dbConnection.DefaultConnectionStringSettings.ConnectionString, sql).Tables[0];

            output.Columns.Add(new DataColumn() { ColumnName = "Type", DataType = typeof(System.String) });
            output.Columns.Add(new DataColumn() { ColumnName = "Code", DataType = typeof(System.String) });
            output.Columns.Add(new DataColumn() { ColumnName = "Name", DataType = typeof(System.String) });
            output.Columns.Add(new DataColumn() { ColumnName = "Flag", DataType = typeof(System.String) });
            output.Columns.Add(new DataColumn() { ColumnName = "Total", DataType = typeof(System.Int32) });
            output.AcceptChanges();

            #region 初始化返回数据表
            DataTable dt = MySqlHelper.ExecuteDataset(dbConnection.DefaultConnectionStringSettings.ConnectionString, "select * from p_extflag").Tables[0];
            DataRow dr;
            foreach (DataRow item in dt.Rows)
            {
                dr = output.NewRow();
                dr["Type"] = "ext";
                dr["Code"] = item["ExtCode"].ToString();
                dr["Name"] = item["ExtDesc"].ToString();
                dr["Flag"] = item["ExtFlag"].ToString();
                dr["Total"] = data.Select("ExtFlag like '%"+ item["ExtCode"] + "%'").Length;
                output.Rows.Add(dr);
            }
            dr = output.NewRow();
            dr["Type"] = "sex";
            dr["Code"] = "100001";
            dr["Name"] = "男性人口";
            dr["Flag"] = "男";
            dr["Total"] = data.Select("Sex = '100001'").Length;
            output.Rows.Add(dr);

            dr = output.NewRow();
            dr["Type"] = "sex";
            dr["Code"] = "100002";
            dr["Name"] = "女性人口";
            dr["Flag"] = "女";
            dr["Total"] = data.Select("Sex = '100002'").Length;
            output.Rows.Add(dr);

            dr = output.NewRow();
            dr["Type"] = "poputype";
            dr["Code"] = "111001";
            dr["Name"] = "常住人口";
            dr["Flag"] = "常住";
            dr["Total"] = data.Select("PopulationType = '111001'").Length;
            output.Rows.Add(dr);

            dr = output.NewRow();
            dr["Type"] = "poputype";
            dr["Code"] = "111002";
            dr["Name"] = "流动人口";
            dr["Flag"] = "流动";
            dr["Total"] = data.Select("PopulationType = '111002'").Length;
            output.Rows.Add(dr);
            #endregion
            data = null;
            dt = null;
            return output;
        }

    }
}
