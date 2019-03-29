//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HY.SpiderService.Spider
//{
//    public class PiaoFangTingSpider
//    {
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using HY.SpiderService.Spider;
using HY.SpiderService.Module;

namespace HY.SpiderService.Spider
{
    public class PiaoFangTingSpider : BaseSpider
    {
        private HttpHelper httpHelper = new HttpHelper();
        private string SESSION = ConfigurationOperator.GetValue("SESSION");
        private int pageSize = int.Parse(ConfigurationOperator.GetValue("pageSize"));
        private int syear = int.Parse(ConfigurationOperator.GetValue("syear"));
        private string filmName = System.Web.HttpUtility.UrlEncode(ConfigurationOperator.GetValue("filmName"));
        private string tableName;

        public PiaoFangTingSpider()
        {
            tableName = "影片票房分厅分场";
        }

        public override string DownloadPage(string url)
        { 
            System.Threading.Thread.Sleep(1000);
            var postData = "page=1&size=50&s_fromDay=1544284800000&s_toDay=1544457599999&s_isExport=false&sort=totalBoxOffice,desc";
            var cookie = "SESSION=" + SESSION + ";username=zysj-yw;remember=false;rememberIsCerLogin=false;name=%E4%B8%AD%E5%BD%B1%E6%95%B0%E5%AD%97%E4%B8%9A%E5%8A%A1;userType=NationalManagementUser;isShowTopMenu=true;subSystem=srfs;";
            HttpItem item = new HttpItem()
            {
                URL = url,//URL
                Encoding = Encoding.UTF8,//编码格式
                Method = "post",//URL
                Cookie = cookie,//字符串Cookie 
                UserAgent = " Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko",//用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "application/json, text/plain, */*",//    可选项有默认值 
                ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值
                //Postdata = postData
            };
            HttpResult = httpHelper.GetHtml(item);
            return HttpResult.Html;
        }
        public override void Begin()
        {
            var sdate = SDate();
            var edate = DateTime.Parse(syear + "-12" + "-31");
            //按天
            for (var i = sdate; i <= edate; i = i.AddDays(1))
            {
                var jssDate = ConvertJSTime(i);
                var jseDate = ConvertJSTime(i.AddDays(1).AddSeconds(-1));
                //var url = "https://gjdyzjb.cn/srfs/w/cinemaChainBoxOfficeInquiry/s?page=" + 0 + "&size=" + pageSize + "&s_fromDay=" + jssDate + "&s_toDay=" + jseDate + "&s_everyDay=true&s_sort=cinema_code&sort=cinema_code,asc";
                var url = "https://gjdyzjb.cn/srfs/w/statisticsForCinemaFilmScenes/s?page=" + 0 + "&size=" + pageSize + "&s_fromDay=" + jssDate + "&s_toDay=" + jseDate + "&s_filmName=" + filmName + "&s_isExport=false&sort=totalBoxOffice,desc";
                var html = DownloadPage(url);
                var JsonObjectTing = JsonConvert.DeserializeObject<JsonObjectTing>(html);



                //每天有多少页
                for (int j = SPage(i); j < JsonObjectTing.pageable.totalPages; j++)
                {
                    Console.WriteLine(i.ToString("yyyy-MM-dd") + "第{" + (j) + "}页");
                    //url = "https://gjdyzjb.cn/srfs/w/cinemaChainBoxOfficeInquiry/s?page=" + j + "&size=" + pageSize + "&s_fromDay=" + jssDate + "&s_toDay=" + jseDate + "&s_everyDay=true&s_sort=cinema_code&sort=cinema_code,asc";
                    url = "https://gjdyzjb.cn/srfs/w/statisticsForCinemaFilmScenes/s?page=" + j + "&size=" + pageSize + "&s_fromDay=" + jssDate + "&s_toDay=" + jseDate + "&s_filmName=" + filmName + "&s_isExport=false&sort=totalBoxOffice,desc";
                    html = DownloadPage(url);
                    JsonObjectTing = JsonConvert.DeserializeObject<JsonObjectTing>(html);
                    InsertZhuanzi(JsonObjectTing.data, j);
                    if (JsonObjectTing.pageable.last)
                        break;
                }
            }
            Console.WriteLine("完成！输入任意键结束！");
        }

        public void InsertZhuanzi(List<PiaoFangTing> items, int page)
        {
            var dt = FITTHelper.Query("select * from " + tableName + " where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            foreach (var item in items)
            {
                var date = ConvertDataTime(item.businessDate);
                DataRow dr = dt.NewRow();
                dr["省份"] = item.provinceName;
                dr["所属院线"] = item.cinemaChainName;
                dr["影院编码"] = item.cinemaCode;
                dr["影院名称"] = item.cinemaName;
                dr["场次编码"] = item.sessionCode;
                dr["影片编码"] = item.filmCode;
                dr["影片名称"] = item.filmName;
                dr["发行版本"] = item.screenName;
                dr["影厅编码"] = item.screenCode;
                dr["放映日期"] = item.sessionDateTime;
                dr["总人数"] = item.totalAudience;
                dr["总票房"] = item.totalBoxOffice;
                dt.Rows.Add(dr);
            }
            try
            {
                FITTHelper.BulkToDB(tableName, dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public override DataTable GetAsyncTable()
        {
            throw new NotImplementedException();
        }


        public DateTime ConvertDataTime(string time)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(time + "0000");  //说明下，时间格式为13位后面补加4个"0"，如果时间格式为10位则后面补加7个"0",至于为什么我也不太清楚，也是仿照人家写的代码转换的
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow); //得到转换后的时间
            return dtResult;
        }

        public long ConvertJSTime(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (dt.Ticks - startTime.Ticks) / 10000;//除10000调整为13位
        }

        public DateTime SDate()
        {
            var sdate = int.Parse(ConfigurationOperator.GetValue("syear"));
            var sql = @"select isnull(max(放映日期),'1900-01-01')日期  from " + tableName + " where year(放映日期)=" + sdate;
            var riqi = FITTHelper.GetSingleVal(sql).ToString();
            if (DateTime.Parse(riqi) == DateTime.Parse("1900-01-01"))
            {
                return new DateTime(sdate, 1, 1);
            }
            else
            {
                return DateTime.Parse((riqi));
            }
        }
        public int SPage(DateTime sdate)
        {
            var sql = @"select isnull(max(当前页),0) 当前页 from " + tableName + " where year(放映日期)=" + sdate.Year + " and month(放映日期)=" + sdate.Month + " and day(放映日期)=" + sdate.Day;
            var page = int.Parse(FITTHelper.GetSingleVal(sql).ToString());
            return page == 0 ? page : page + 1;
        }
    }
}


