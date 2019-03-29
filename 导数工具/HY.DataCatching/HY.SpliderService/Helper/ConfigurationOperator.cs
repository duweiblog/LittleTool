using System;
using System.Configuration;
using System.Web;
using System.Net.Configuration;

namespace HY.SpliderService
{
    public class ConfigurationOperator
    {
        /// <summary>  
        /// 获取配置值  
        /// </summary>  
        /// <param name="key">配置标识</param>  
        /// <returns></returns>  
        public static string GetValue(string key)
        {
            string result = string.Empty;
            if (ConfigurationManager.AppSettings[key] != null)
                result = ConfigurationManager.AppSettings.Get(key);
            return result;
        }
    }
}
