using HY.SpiderService;
using HY.SpiderService.Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.SpiderService.Spider
{
    public class YingYuanSpider : BaseSpider
    {
        private int pageSize = 2000;
        public override void Begin()
        {
            Console.WriteLine("输入开始时间和结束时间。");
            var sdate = DateTime.Parse(Console.ReadLine());
            var edate = DateTime.Parse(Console.ReadLine());
            //按天

            var jssDate = ConvertJSTime(sdate);
            var jseDate = ConvertJSTime(edate);
            //var url = "https://gjdyzjb.cn/srfs/w/cinemaChainBoxOfficeInquiry/s?page=" + 0 + "&size=" + pageSize + "&s_fromDay=" + jssDate + "&s_toDay=" + jseDate + "&s_everyDay=true&s_sort=cinema_code&sort=cinema_code,asc";
            var url = "https://gjdyzjb.cn/srfs/w/statisticsForCinemaInfo/s?page=" + 0 + "&size=" + pageSize + "&s_fromDay=" + jssDate + "&s_toDay=" + jseDate + "&sort=cinemaCode,desc";
            var html = DownloadPage(url);
            var jsonObject = JsonConvert.DeserializeObject<JsonObjectYingYuan>(html);
            //每天有多少页
            for (int j = SPage(); j < jsonObject.pageable.totalPages; j++)
            {
                Console.WriteLine("第{" + (j) + "}页");
                url = "https://gjdyzjb.cn/srfs/w/statisticsForCinemaInfo/s?page=" + j + "&size=" + pageSize + "&s_fromDay=" + jssDate + "&s_toDay=" + jseDate + "&sort=cinemaCode,desc";
                html = DownloadPage(url);
                jsonObject = JsonConvert.DeserializeObject<JsonObjectYingYuan>(html);
                InsertDB(jsonObject.data, j);
                if (jsonObject.pageable.last)
                    break;
            }

            Console.WriteLine("完成！输入任意键结束！");
        }

        private void InsertDB(List<YingYuan> items, int j)
        {
            var dt = FITTHelper.Query("select * from 影院专资表 where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            foreach (var item in items)
            {
                var date = ConvertDataTime(item.businessDate);
                DataRow dr = dt.NewRow();
                dr["影院编码"] = item.cinemaCode;
                dr["省份"] = item.provinceName;
                dr["影院简称"] = item.cinemaName;
                dr["影院工商注册名称"] = item.officalName;
                dr["所属院线"] = item.cinemaChainName;
                dr["是否电脑售票"] = item.pcsell;
                dr["售票软件名称"] = item.softwareName;
                dr["营业状态"] = int.Parse(item.businessStatus.Trim());
                dr["厅数"] = int.Parse(item.totalScreenCount.Trim());
                dr["座位数"] = int.Parse(item.totalSeatCount.Trim());
                dr["部数"] = int.Parse(item.totalFilm.Trim());
                dr["场次"] = int.Parse(item.totalSession.Trim());
                dr["人数"] = int.Parse(item.totalAudience.Trim());
                dr["票房"] = decimal.Parse(item.totalBoxOffice.Trim());
                dr["上报天数"] = int.Parse(item.reportDays.Trim());
                dr["当前页"] = j;
                dt.Rows.Add(dr);
            }
            try
            {
                FITTHelper.BulkToDB("影院专资表", dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
            HttpResult = http.GetHtml(item);
            return HttpResult.Html;
        }

        public override DataTable GetAsyncTable()
        {
            throw new NotImplementedException();
        }

        public int SPage()
        {
            var sql = @"select isnull(max(当前页),0) 当前页 from 影院专资表";
            var page = int.Parse(FITTHelper.GetSingleVal(sql).ToString());
            return page == 0 ? page : page + 1;
        }
    }
}
