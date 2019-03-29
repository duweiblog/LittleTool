using HY.SpiderService.Module;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.SpiderService.Spider
{
    public abstract class BaseSpider
    {
        public string url;

        public HttpResult HttpResult;

        public static HttpHelper http = new HttpHelper();

        public static DbHelperSQL FITTHelper = new DbHelperSQL("FITT");


        public static DbHelperSQL MtimeHelper = new DbHelperSQL("Mtime");

        public string SESSION = ConfigurationOperator.GetValue("SESSION");
        //开始
        public abstract void Begin();
        public abstract string DownloadPage(string url);
        // public abstract object AnalysisHtml(string html, string ItemID, string href);
        //public abstract void InsertDB(IBaseModule modules);
        public abstract DataTable GetAsyncTable();

        public long ConvertJSTime(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (dt.Ticks - startTime.Ticks) / 10000;//除10000调整为13位
        }

        public DateTime ConvertDataTime(string time)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(time + "0000");  //说明下，时间格式为13位后面补加4个"0"，如果时间格式为10位则后面补加7个"0",至于为什么我也不太清楚，也是仿照人家写的代码转换的
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow); //得到转换后的时间
            return dtResult;
        }
    }

    public static class StrExtions
    {
        public static string Transferred(this string str)
        {
            return string.IsNullOrEmpty(str) ? "" : str.Replace("&#183;", "·");
        }

        public static string TrimStr(this string str)
        {
            return string.IsNullOrEmpty(str) ? "无" : str.Trim();
        }
    }
}
