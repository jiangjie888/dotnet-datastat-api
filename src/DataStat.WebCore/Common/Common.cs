using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Security.Cryptography;

namespace DataStat.WebCore.Common
{
    public static class Common
    {

        /// <summary>
        /// 生成充值流水号格式：8位日期加5位顺序号，如2016080100001
        /// </summary>
        //public static string GetSerialNumber(string url, string code)
        //{
        //    string output = "";
        //    HTTPClientHelper httpRequest = new HTTPClientHelper();
        //    httpRequest.Url = url;
        //    httpRequest.PostingData.Add("SerialnbrCode", code);
        //    string result = httpRequest.GetResponseBySimple();
        //    var resultObj = (JObject)JsonConvert.DeserializeObject(result);

        //    output = resultObj["result"].ToString();
        //    return output;
        //}


        //public static JObject GetSysSeting(string url, string code)
        //{
        //    //string output = "";
        //    HTTPClientHelper httpRequest = new HTTPClientHelper();
        //    //RkeyInput input = new RkeyInput() { Rkey = code };
        //    httpRequest.Url = url;
        //    httpRequest.PostingData.Add("Code", code);
        //    string result = httpRequest.GetResponseBySimple();
        //    var resultObj = (JObject)JsonConvert.DeserializeObject(result);
        //    return resultObj;
        //    //output = resultObj;
        //    //return output;
        //}

        /// <summary>
        /// 获取字典值
        /// </summary>
        //public static string GetDictName(string url, string pcode,string dictVal)
        //{
        //    string output = "";
        //    HTTPClientHelper httpRequest = new HTTPClientHelper();
        //    httpRequest.Url = url;
        //    httpRequest.PostingData.Add("code", pcode);
        //    httpRequest.PostingData.Add("dictVal", dictVal);
        //    string result = httpRequest.GetResponseBySimple();
        //    var resultObj = (JObject)JsonConvert.DeserializeObject(result);

        //    output = resultObj["result"].ToString();
        //    return output;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="field"></param>
        ///// <returns></returns>
        //public static object GetPropertyValue(object info, string field)
        //{
        //    if (info == null) return null;

        //    Type t = info.GetType();

        //    IEnumerable<System.Reflection.PropertyInfo> property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;

        //    var re = property.First();
        //    return property.First().GetValue(info, null);

        //}


        /// <summary>
        /// 获取私有成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">实例对像</param>
        /// <param name="propertyname">成员名</param>
        /// <returns></returns>
        public static T GetPrivateProperty<T>(object instance, string propertyname)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            PropertyInfo field = type.GetProperty(propertyname, flag);
            return (T)field.GetValue(instance, null);
        }

        /// <summary>
        /// 获取所有成员
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">实例对像</param>
        /// <param name="propertyname">成员名</param>
        /// <returns></returns>
        public static T GetProperty<T>(object instance, string propertyname)
        {
            Type type = instance.GetType();
            T outdata = default(T);
            foreach (PropertyInfo pi in type.GetProperties())
            {
                if (pi.Name == propertyname)
                {
                    outdata = (T)pi.GetValue(instance, null);
                    break;
                }
            }
            return outdata;
        }

        /// <summary>
        /// 设置成员值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyname"></param>
        /// <param name="propertyvalue"></param>
        /// <returns></returns>
        public static object SetProperty(object instance, string propertyname, string propertyvalue)
        {
            Type type = instance.GetType();
            foreach (PropertyInfo pi in type.GetProperties())
            {
                if (pi.Name == propertyname)
                {
                    pi.SetValue(instance, propertyvalue);
                    break;
                }
            }
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static void GetPropertys()
        {
            var types = Assembly.Load("WebApp.Core").GetTypes();
            foreach (var type in types)
            {
                var pros = type.GetProperties();
            }
        }

        #region
        /// <summary>
        /// 加密 create by jjie
        /// </summary>
        /// <param name="strOr"></param>
        /// <returns></returns>
        public static string MD5String(string codeName, string strOr)
        {
            byte[] tempData = System.Text.Encoding.GetEncoding(codeName).GetBytes(strOr);
            return BitConverter.ToString((new MD5CryptoServiceProvider()).ComputeHash(tempData)).Replace("-", "");
        }
        #endregion

        #region
        /// <summary>
        /// 加密 create by jjie
        /// </summary>
        /// <param name="strOr"></param>
        /// <returns></returns>
        public static string DecodeBase64(string codeName, string strOr)
        {
            string result = "";
            byte[] bytes = Convert.FromBase64String(strOr);
            result = System.Text.Encoding.GetEncoding(codeName).GetString(bytes);
            return result;
        }
        #endregion
    }
}
