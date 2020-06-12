﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataStat.FrameWork.DomainModel.AuditLogs
{
    [Table("Sys_AuditLogs")]
    public class AuditLog
    {
        /// <summary>
        /// Maximum length of <see cref="ServiceName"/> property.
        /// </summary>
        public static int MaxServiceNameLength = 256;

        /// <summary>
        /// Maximum length of <see cref="MethodName"/> property.
        /// </summary>
        public static int MaxMethodNameLength = 256;

        /// <summary>
        /// Maximum length of <see cref="Parameters"/> property.
        /// </summary>
        public static int MaxParametersLength = 1024;

        /// <summary>
        /// Maximum length of <see cref="ClientIpAddress"/> property.
        /// </summary>
        public static int MaxClientIpAddressLength = 64;

        /// <summary>
        /// Maximum length of <see cref="ClientName"/> property.
        /// </summary>
        public static int MaxClientNameLength = 128;

        /// <summary>
        /// Maximum length of <see cref="BrowserInfo"/> property.
        /// </summary>
        public static int MaxBrowserInfoLength = 512;

        /// <summary>
        /// Maximum length of <see cref="Exception"/> property.
        /// </summary>
        public static int MaxExceptionLength = 2000;

        /// <summary>
        /// Maximum length of <see cref="CustomData"/> property.
        /// </summary>
        public static int MaxCustomDataLength = 2000;


        public virtual long Id { get; set; }

        public virtual string SystemCode { get; set; }

        /// <summary>
        /// TenantId.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// UserId.
        /// </summary>
        public virtual long? UserId { get; set; }

        /// <summary>
        /// Service (class/interface) name.
        /// </summary>
        public virtual string ServiceName { get; set; }

        /// <summary>
        /// Executed method name.
        /// </summary>
        public virtual string MethodName { get; set; }

        /// <summary>
        /// Calling parameters.
        /// </summary>
        public virtual string Parameters { get; set; }

        /// <summary>
        /// Start time of the method execution.
        /// </summary>
        public virtual DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Total duration of the method call as milliseconds.
        /// </summary>
        public virtual int ExecutionDuration { get; set; }

        /// <summary>
        /// IP address of the client.
        /// </summary>
        public virtual string ClientIpAddress { get; set; }

        /// <summary>
        /// Name (generally computer name) of the client.
        /// </summary>
        public virtual string ClientName { get; set; }

        /// <summary>
        /// Browser information if this method is called in a web request.
        /// </summary>
        public virtual string BrowserInfo { get; set; }

        /// <summary>
        /// Exception object, if an exception occured during execution of the method.
        /// </summary>
        public virtual string Exception { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.ImpersonatorUserId"/>.
        /// </summary>
        public virtual long? ImpersonatorUserId { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.ImpersonatorTenantId"/>.
        /// </summary>
        public virtual int? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.CustomData"/>.
        /// </summary>
        public virtual string CustomData { get; set; }

        /// <summary>
        /// Creates a new CreateFromAuditInfo from given <see cref="auditInfo"/>.
        /// </summary>
        /// <param name="auditInfo">Source <see cref="AuditInfo"/> object</param>
        /// <returns>The <see cref="AuditLog"/> object that is created using <see cref="auditInfo"/></returns>
        //public static AuditLog CreateFromAuditInfo(AuditInfo auditInfo)
        //{
        //    var exceptionMessage = auditInfo.Exception != null ? auditInfo.Exception.ToString() : null;
        //    //var _appsettings = IocManager.Instance.Resolve<AppSettingsCfg>(); 
        //    return new AuditLog
        //    {
        //        SystemCode = "Stat001",
        //        TenantId = auditInfo.TenantId,
        //        UserId = auditInfo.UserId,
        //        ServiceName = auditInfo.ServiceName.TruncateWithPostfix(MaxServiceNameLength),
        //        MethodName = auditInfo.MethodName.TruncateWithPostfix(MaxMethodNameLength),
        //        Parameters = auditInfo.Parameters.TruncateWithPostfix(MaxParametersLength),
        //        ExecutionTime = auditInfo.ExecutionTime,
        //        ExecutionDuration = auditInfo.ExecutionDuration,
        //        ClientIpAddress = auditInfo.ClientIpAddress.TruncateWithPostfix(MaxClientIpAddressLength),
        //        ClientName = auditInfo.ClientName.TruncateWithPostfix(MaxClientNameLength),
        //        BrowserInfo = auditInfo.BrowserInfo.TruncateWithPostfix(MaxBrowserInfoLength),
        //        Exception = exceptionMessage.TruncateWithPostfix(MaxExceptionLength),
        //        ImpersonatorUserId = auditInfo.ImpersonatorUserId,
        //        ImpersonatorTenantId = auditInfo.ImpersonatorTenantId,
        //        CustomData = auditInfo.CustomData.TruncateWithPostfix(MaxCustomDataLength)
        //    };
        //}

        /// <summary>
        /// 截取指定长度字符串
        /// </summary>
        /// <param name="inputString">要处理的字符串</param>
        /// <param name="len">指定长度</param>
        /// <returns>返回处理后的字符串</returns>
        public string ClipString(string inputString, int len)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return "";
            bool isShowFix = false;
            if (len % 2 == 1)
            {
                isShowFix = true;
                len--;
            }
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }

            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (isShowFix && mybyte.Length > len)
                tempString += "…";
            return tempString;
        }

        public override string ToString()
        {
            return string.Format(
                "AUDIT LOG: {0}.{1} is executed by user {2} in {3} ms from {4} IP address.",
                ServiceName, MethodName, UserId, ExecutionDuration, ClientIpAddress
                );
        }
    }
}
