using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DataStat.WebSite
{
    public class HiddenApiFilter : IDocumentFilter
    {
        /// <summary>  
        /// 重写Apply方法，移除隐藏接口的生成  
        /// </summary>  
        /// <param name="swaggerDoc">swagger文档文件</param>  
        /// <param name="schemaRegistry"></param>  
        /// <param name="apiExplorer">api接口集合</param>  
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (ApiDescription apiDescription in context.ApiDescriptionsGroups.Items.SelectMany(e => e.Items))
            {
                var attributes = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)apiDescription.ActionDescriptor).MethodInfo.CustomAttributes;
                if (attributes.Count(t => t.AttributeType == typeof(HiddenApiAttribute)) == 0) continue;

                var key = "/" + apiDescription.RelativePath.TrimEnd('/');
                if (!key.Contains("/test/") && swaggerDoc.Paths.ContainsKey(key))
                    swaggerDoc.Paths.Remove(key);
            }
        }
    }

    public class CustomDocumentFiliter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            SetContorllerDescription(swaggerDoc.Extensions);
        }

        private void SetContorllerDescription(Dictionary<string, object> extensionsDict)
        {
            string _xmlPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\swagger.xml");
            ConcurrentDictionary<string, string> _controllerDescDict = new ConcurrentDictionary<string, string>();

            if (File.Exists(_xmlPath))
            {
                XmlDocument _xmlDoc = new XmlDocument();
                _xmlDoc.Load(_xmlPath);

                string _type = string.Empty, _path = string.Empty, _controllerName = string.Empty;
                XmlNode _summaryNode = null;

                foreach (XmlNode _node in _xmlDoc.SelectNodes("//member"))
                {
                    _type = _node.Attributes["name"].Value;

                    if (_type.StartsWith("T:") && !_type.Contains("T:HKERP.HKERPAppServiceBase") && !_type.Contains("T:HKERP.Net.MimeTypes.MimeTypeNames"))
                    {
                        _summaryNode = _node.SelectSingleNode("summary");
                        string[] _names = _type.Split('.');
                        string _key = _names[_names.Length - 1];
                        if (_key == "SessionAppService")
                            _key = "Session";
                        if (_key.IndexOf("AppService", _key.Length - "AppService".Length, StringComparison.Ordinal) > -1)
                        {
                            _key = _key.Substring(0, _key.Length - "AppService".Length);
                        }

                        if (_summaryNode != null && !string.IsNullOrEmpty(_summaryNode.InnerText) && !_controllerDescDict.ContainsKey(_key))
                        {
                            _controllerDescDict.TryAdd(_key, _summaryNode.InnerText.Trim());
                        }
                    }
                }
                _controllerDescDict.TryAdd("Account", "系统默认登录注销");

                extensionsDict.TryAdd("ControllerDescription", _controllerDescDict);
            }
        }
    }
}
