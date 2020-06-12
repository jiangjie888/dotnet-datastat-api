using Dapper;
using DataStat.DBUtil;
using DataStat.FrameWork.DomainModel;
using DataStat.FrameWork.DomainModel.AuditLogs;
using DataStat.FrameWork.Repository;
using DataStat.WebCore.Common;
using DataStat.WebCore.Common.Dto;
using DataStat.WebCore.CommonSuport.Authentication;
using DataStat.WebCore.CommonSuport.ExtException;
using DataStat.WebCore.CommonSuport.ViewModel;
using DataStat.WebCore.Configuration;
using DataStat.WebCore.Controllers.Forest.Dto;
using DataStat.WebCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataStat.WebCore.Controllers.Forest
{
    /// <summary>
    /// 森林数据同步控制器
    /// </summary>
    [Route("[controller]/[action]")]
    public class DataSynController : WebAppControllerBase
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IIdentityService _identity;
        private readonly IRepository<SysUser,long> _sysUserRepository;
        private readonly IRepository<SysUsertrack, long> _sysUserTrackRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HTTPClientHelper _httpClient;
        

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataSynController(IHostingEnvironment env, 
                                IIdentityService identity,
                                IHttpClientFactory httpClientFactory,
                                IRepository<SysUser, long> sysUserRepository,
                                IRepository<SysUsertrack, long> sysUserTrackRepository)
        {
            _appConfiguration = env.GetAppConfiguration();
            _identity = identity;
            _sysUserRepository = sysUserRepository;
            _sysUserTrackRepository = sysUserTrackRepository;
            _httpClientFactory = httpClientFactory;
            _httpClient = new HTTPClientHelper(_httpClientFactory);

        }

        /// <summary>
        /// 根据人,开始结束时间获取轨迹信息
        /// </summary>
        /// <param input=""></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<string> SynLocationData(SynLocationDataInput input)
        {
            //http://118.85.200.79/ids/flex/findLocationByMdn/18995896655/2020-06-01%2000:00/2020-06-04%2015:52?jsonp=jsonp_D87B57B230AE403CAEB03F9DA276A3FF
            //http://118.85.200.79/ids/flex/findLocationByMdn/18908604880/2020-06-01%2000:00/2020-06-04%2017:08?jsonp=jsonp_FA012AFB6B144FBA8323F6247F565100

            string jsonp = (input.Url.Split('?')[1]).Split('=')[1];
            _httpClient.Url = input.Url;
            //_httpClient.PostingData.Add("SerialnbrCode", code);
            string result = _httpClient.GetResponseBySimple("1");
            if(string.IsNullOrEmpty(result)==false)
            {
                if (result.IndexOf(jsonp) == 0)
                {
                    result = result.Replace(jsonp + "(", "").TrimEnd(')');
                }

            }
            result = result.Trim();
            var resultObj = (JArray)JsonConvert.DeserializeObject(result);
            //onp_D87B57B230AE403CAEB03F9DA276A3FF( [ { "
            //output = resultObj["result"].ToString();
            //ype": '基站定位', "time": '2020-06-01 08:56:00', "loc": '湖北省 宜昌市 五峰土家族自治县 曹家坪路 附近；渔洋关国土所往西北约305米', "r": 1, "lng": 111.070966, "lat": 30.166492 }

            //先删除，再新增
            DBConnection dbConnection = new DBConnection();
            //var conn = dbConnection.GetDbConnection();

            MySqlParameter[] parms = new MySqlParameter[1];
            parms[0] = new MySqlParameter("P_Id", MySqlDbType.Int64);
            parms[0].Value = input.UserId;
            var username = MySqlHelper.ExecuteScalar(dbConnection.DefaultConnectionStringSettings.ConnectionString, "select UserName from sys_user where Id=@P_Id", parms);
            input.UserName = username.ToString();

            #region 删除
            string sqldel = @"delete from sys_usertrack where UserId=@P_UserId and Time>=@P_BegDate and Time<=@P_EndDate";
            parms = new MySqlParameter[3];
            parms[0] = new MySqlParameter("P_UserId", MySqlDbType.Int64);
            parms[0].Value = input.UserId;

            parms[1] = new MySqlParameter("P_BegDate", MySqlDbType.DateTime);
            parms[1].Value = input.BegDate;

            parms[2] = new MySqlParameter("P_EndDate", MySqlDbType.DateTime);
            parms[2].Value = input.EndDate;

            MySqlHelper.ExecuteNonQuery(dbConnection.DefaultConnectionStringSettings.ConnectionString, sqldel, parms);
            #endregion

            foreach (var item in resultObj)
            {
                _sysUserTrackRepository.InsertAsync(new SysUsertrack()
                {
                    Type = item["type"].ToString(),
                    DeviceId = null,
                    Lat = item["lat"].ToString(),
                    Lon = item["lng"].ToString(),
                    Loc = item["loc"].ToString(),
                    Time = Convert.ToDateTime(item["time"]),
                    UserId = input.UserId.Value,
                    UserName = input.UserName
                });
            }
           

            return "Ok";


        }

        /// <summary>
        /// 获取当前所有人员
        /// </summary>
        /// <param input="v">参数1</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<List<GetUserListOuput>> GetUserList(RkeyInput input)
        {
            List<GetUserListOuput> output = new List<GetUserListOuput>();
            IEnumerable <SysUser> list = await _sysUserRepository.GetAllAsync();
            foreach (var item in list)
            {
                var location = await GetUserLastLocation(new RkeyInput() { Rkey = item.Id.ToString() });
                output.Add(new GetUserListOuput()
                {
                    Id = item.Id,
                    UserAccount = item.UserAccount,
                    UserName = item.UserName,
                    UesrRelId = item.UesrRelId,
                    CreationTime = item.CreationTime,
                    CreatorUserId = item.CreatorUserId,
                    Lon = location==null ? null:location.Lon,
                    Lat = location== null ? null : location.Lat
                });
            }
            return output;
        }


        /// <summary>
        /// 获取人员最后定位
        /// </summary>
        /// <param input="v">参数1</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<SysUsertrack> GetUserLastLocation(RkeyInput input)
        {
            SysUsertrack output = new SysUsertrack();
            try
            {
                var sql = string.Format(@"select * from {0} where UserId=@UserId order by Time desc limit 1;", "sys_usertrack");
                using (var conn = new DBConnection().GetDbConnection())
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    output = await conn.QueryFirstOrDefaultAsync<SysUsertrack>(sql, new { @UserId = input.Rkey }, commandType: CommandType.Text);
                }
                return output;
            }
            catch (Exception e) 
            {
                throw new Exception("操作失败：DB数据获取失败");
            }
        }

        /// <summary>
        /// 获取人员轨迹信息
        /// </summary>
        /// <param input="v">参数1</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<DataTable> FindLocationByMdn(SynLocationDataInput input)
        {
            DataTable output = new DataTable();

  
            DBConnection dbConnection = new DBConnection();
            //var conn = dbConnection.GetDbConnection();

            string sql = @"select * from sys_usertrack where UserId=@P_UserId and Time>=@P_BegDate and Time<=@P_EndDate order by Time";
            MySqlParameter[] parms = new MySqlParameter[3];
            parms[0] = new MySqlParameter("P_UserId", MySqlDbType.Int64);
            parms[0].Value = input.UserId;

            parms[1] = new MySqlParameter("P_BegDate", MySqlDbType.DateTime);
            parms[1].Value = input.BegDate;

            parms[2] = new MySqlParameter("P_EndDate", MySqlDbType.DateTime);
            parms[2].Value = input.EndDate;

            output = (await MySqlHelper.ExecuteDatasetAsync(dbConnection.DefaultConnectionStringSettings.ConnectionString, sql, parms)).Tables[0];
            return output;
        }


        /// <summary>
        /// 第一个测试Token认证
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> FirstTestAuth(string v)
        {
            return "the first test demo account=" + v + _identity.GetUserAccount();
        }

        #region add by jjie
        #endregion
    }
}
