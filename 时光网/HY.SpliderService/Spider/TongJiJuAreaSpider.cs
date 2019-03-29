using HtmlAgilityPack;
using HY.SpiderService.Module;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.SpiderService.Spider
{
    public class TongJiJuAreaSpider : BaseSpider
    {
        public Object ObjectLock = new Object();
        private string UrlBefore = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2017/";
        public override void Begin()
        {
            //var url = "https://gjdyzjb.cn/srfs/w/cinemaChainBoxOfficeInquiry/s?page=" + 0 + "&size=" + pageSize + "&s_fromDay=" + jssDate + "&s_toDay=" + jseDate + "&s_everyDay=true&s_sort=cinema_code&sort=cinema_code,asc";
            var url = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2017/index.html";
            var html = DownloadPage(url);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);///html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr[4]/td[1]/a
            HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr/td/a");
            foreach (HtmlNode item in collection)
            {
                var name = item.InnerText.Trim();
                Console.WriteLine($"{name}");
                var area = new TongJiJuArea();
                area.AreaName = name;
                InsertDB(area);
                var href = UrlBefore + item.GetAttributeValue("href", "").Trim();
                GetCityArea(href, name);
            }
            Console.WriteLine("完成！输入任意键结束！");
        }

        private void InsertDB(params TongJiJuArea[] items)
        {
            var dt = FITTHelper.Query("select * from 地区行政区划表B where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            foreach (var item in items)
            {
                DataRow dr = dt.NewRow();
                dr["地区代码"] = item.AreaCode;
                dr["地区全称"] = item.AreaName;
                dr["城乡分类"] = string.IsNullOrEmpty(item.TypeCode) ? "" : item.TypeCode;
                dr["省"] = string.IsNullOrEmpty(item.Sheng) ? "" : item.Sheng;
                dr["市"] = string.IsNullOrEmpty(item.Shi) ? "" : item.Shi;
                dr["县"] = string.IsNullOrEmpty(item.Xian) ? "" : item.Xian;
                dr["乡"] = string.IsNullOrEmpty(item.Xiang) ? "" : item.Xiang;
                dr["村"] = string.IsNullOrEmpty(item.Chun) ? "" : item.Chun;
                dt.Rows.Add(dr);
            }
            try
            {
                FITTHelper.BulkToDB("地区行政区划表B", dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public override string DownloadPage(string url)
        {
            lock (ObjectLock)
            {
                var item = new HttpItem()
                {
                    URL = url,
                    Encoding = Encoding.GetEncoding("gb2312"),//编码格式
                    Method = "get",//URL
                    Cookie = "",//字符串Cookie
                    UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E)",//用户的浏览器类型，版本，操作系统     可选项有默认值
                    Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/msword, application/vnd.ms-excel, application/vnd.ms-powerpoint, */*",//    可选项有默认值
                    ContentType = "application/x-www-form-urlencoded",//返回类型
                };
                HttpResult = http.GetHtml(item);
                var rs = HttpResult.Html;
            }
            return HttpResult.Html;
        }

        public void GetCityArea(string url, string sheng)
        {
            var html = DownloadPage(url);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);///html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr[2]
            HtmlNodeCollection city = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='citytr']");
            if (city == null)
            {
                HtmlNodeCollection countytr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='countytr']");
                if (countytr != null)
                {
                    GetCountyArea(url, sheng, "");
                }
                HtmlNodeCollection towntr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='towntr']");
                if (towntr != null)
                {
                    GetTownArea(url, sheng, "", "");
                }

                HtmlNodeCollection villagetr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='villagetr']");
                if (villagetr != null)
                {
                    GetVillageArea(url, sheng, "", "", "");
                }
                return;
            }
            ///html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr[3]
            //HtmlNodeCollection village = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='villagetr']");
            var list = new List<TongJiJuArea>();
            if (city != null && city.Count > 0)
            {
                foreach (HtmlNode item in city)
                {
                    try
                    {
                        var nodes = item.SelectNodes(".//a");
                        if (nodes == null)
                        {
                            nodes = item.SelectNodes(".//td");
                        }
                        var codeStr = nodes[0].InnerText.Trim();
                        var nameStr = nodes[1].InnerText.Trim();
                        var hrefStr = UrlBefore + codeStr.Substring(0, 2) + "/" + codeStr.Substring(0, 4) + ".html";
                        var area = new TongJiJuArea();
                        area.AreaCode = codeStr;
                        area.AreaName = nameStr;
                        area.Sheng = sheng;
                        InsertDB(area);
                        Console.WriteLine(sheng + nameStr);
                        GetCountyArea(hrefStr, sheng, nameStr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(item.InnerHtml);
                    }
                }
            }
        }

        public void GetCountyArea(string url, string sheng, string shi)
        {
            var html = DownloadPage(url);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            //////html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr[2]
            HtmlNodeCollection countytr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='countytr']");
            if (countytr == null)
            {
                HtmlNodeCollection towntr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='towntr']");
                if (towntr != null)
                {
                    GetTownArea(url, sheng, "", "");
                }

                HtmlNodeCollection villagetr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='villagetr']");
                if (villagetr != null)
                {
                    GetVillageArea(url, sheng, "", "", "");
                }
                return;
            }
            var list = new List<TongJiJuArea>();
            if (countytr != null && countytr.Count > 0)
            {
                foreach (HtmlNode item in countytr)
                {
                    try
                    {
                        var nodes = item.SelectNodes(".//a");
                        if (nodes == null)
                        {
                            nodes = item.SelectNodes(".//td");
                        }
                        var codeStr = nodes[0].InnerText.Trim();
                        var nameStr = nodes[1].InnerText.Trim();
                        var hrefStr = UrlBefore + codeStr.Substring(0, 2) + "/" + codeStr.Substring(2, 2) + "/" + codeStr.Substring(0, 6) + ".html";
                        var area = new TongJiJuArea();
                        area.AreaCode = codeStr;
                        area.AreaName = nameStr;
                        area.Sheng = sheng;
                        area.Shi = shi;
                        InsertDB(area);
                        //Console.WriteLine(sheng + shi + nameStr);
                        GetTownArea(hrefStr, sheng, shi, nameStr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(item.InnerHtml);
                    }
                }
            }
        }

        public void GetTownArea(string url, string sheng, string shi, string xian)
        {
            var html = DownloadPage(url);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            ///html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr[2]
            HtmlNodeCollection towntr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='towntr']");
            if (towntr == null)
            {
                HtmlNodeCollection villagetr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='villagetr']");
                if (villagetr != null)
                {
                    GetVillageArea(url, sheng, "", "", "");
                }
                return;
            }
            var list = new List<TongJiJuArea>();
            if (towntr != null && towntr.Count > 0)
            {
                foreach (HtmlNode item in towntr)
                {
                    try
                    {
                        var nodes = item.SelectNodes(".//a");
                        if (nodes == null)
                        {
                            nodes = item.SelectNodes(".//td");
                        }
                        var codeStr = nodes[0].InnerText.Trim();
                        var nameStr = nodes[1].InnerText.Trim();
                        var hrefStr = UrlBefore + "/" + codeStr.Substring(0, 2) + "/" + codeStr.Substring(2, 2) + "/" + "/" + codeStr.Substring(4, 2) + "/" + codeStr.Substring(0, 9) + ".html";
                        var area = new TongJiJuArea();
                        area.AreaCode = codeStr;
                        area.AreaName = nameStr;
                        area.Sheng = sheng;
                        area.Shi = shi;
                        area.Xian = xian;
                        area.Xiang = nameStr;
                        InsertDB(area);
                        //Console.WriteLine(sheng + shi + xian + nameStr);
                        GetVillageArea(hrefStr, sheng, shi, xian, nameStr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(item.InnerHtml);
                    }
                }
            }
        }
        public void GetVillageArea(string url, string sheng, string shi, string xian, string xiang)
        {
            var html = DownloadPage(url);
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            ///html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr[2]
            HtmlNodeCollection villagetr = htmlDocument.DocumentNode.SelectNodes("//html/body/table[2]/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tr[@class='villagetr']");
            if (villagetr == null)
                return;
            var list = new List<TongJiJuArea>();
            if (villagetr != null && villagetr.Count > 0)
            {
                foreach (HtmlNode item in villagetr)
                {
                    try
                    {
                        var tds = item.SelectNodes("td");
                        var codeStr = tds[0].InnerText.Trim();
                        var typeStr = tds[1].InnerText.Trim();
                        var nameStr = tds[2].InnerText.Trim();
                        var area = new TongJiJuArea();
                        area.AreaCode = codeStr;
                        area.TypeCode = typeStr;
                        area.AreaName = nameStr;
                        area.Sheng = sheng;
                        area.Shi = shi;
                        area.Xian = xian;
                        area.Xiang = xiang;
                        area.Chun = nameStr;
                        //Console.WriteLine(sheng + shi + xian + xiang + nameStr);
                        list.Add(area);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(item.InnerHtml);
                    }
                }
                InsertDB(list.ToArray());
            } 
        }
        public override DataTable GetAsyncTable()
        {
            throw new NotImplementedException();
        }
    }
    public class Object { }
}
