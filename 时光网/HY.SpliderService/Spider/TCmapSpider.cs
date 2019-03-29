using HY.SpiderService.Spider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using HY.SpiderService;
using HtmlAgilityPack;
using HY.SpiderService.Module;

namespace HY.SpiderService.Spider
{
    public class TCmapSpider : BaseSpider
    {
        private string url = "http://www.tcmap.com.cn/";
        private int count = 0;
        private string Start = ConfigurationOperator.GetValue("Start").ToLower();
        public override void Begin()
        {
            Console.WriteLine("任务启动...");
            try
            {
                var html = DownloadPage(url);
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);///html/body/table[2]/tbody/tr/td[1]/div[3]/div[1]
                HtmlNodeCollection diqu = htmlDocument.DocumentNode.SelectNodes("//div[@class='ht']/b/a");
                var tempHref = "";
                //省 
                var diqus = new List<HtmlNode>();
                var begin = false;
                foreach (var item in diqu)
                {
                    var sheng = item.GetAttributeValue("href", "").Replace("/", "");
                    //通过配置文件，配置开始省份
                    if (sheng.ToLower().Equals(Start))
                    {
                        begin = true;
                    }
                    if (begin)
                    {
                        diqus.Add(item);
                    }
                }
                foreach (var item in diqus)
                {
                    var a = item;
                    if (a != null)
                    {

                        var sheng = a.GetAttributeValue("href", "");
                        var href = url + sheng;
                        //省自治区
                        InsertHref(href, 1);
                        if (!string.IsNullOrEmpty(href.TrimStr()))
                        {
                            var area = new Area();
                            var shiHtml = DownloadPage(href);
                            HtmlDocument shiHtmlDocument = new HtmlDocument();
                            shiHtmlDocument.LoadHtml(shiHtml);
                            var cityDoms = shiHtmlDocument.DocumentNode.SelectNodes("//*[@id='page_left']/table[1]/tr");
                            //市、区
                            for (int i = 1; i < cityDoms.Count; i++)
                            {
                                var city = cityDoms[i];
                                var cityNodeHref = city.SelectSingleNode(".//strong/a");
                                if (cityNodeHref == null)
                                    continue;
                                tempHref = cityNodeHref.GetAttributeValue("href", "");
                                if (!tempHref.Contains(sheng)) tempHref = sheng + tempHref;
                                var cityHref = url + tempHref;
                                InsertHref(cityHref, 2);

                                var cityHtml = DownloadPage(cityHref);
                                HtmlDocument cityHtmlDocument = new HtmlDocument();
                                cityHtmlDocument.LoadHtml(cityHtml);
                                var cityDetailNodes = cityHtmlDocument.DocumentNode.SelectNodes("//*[@id='page_left']/div[5]/table");
                                if (cityDetailNodes == null)
                                    continue;
                                foreach (var cityNode in cityDetailNodes)
                                {
                                    var hrefs = cityNode.SelectNodes(".//strong/a");
                                    if (hrefs == null)
                                        continue;
                                    foreach (var hrefItem in hrefs)
                                    {
                                        tempHref = hrefItem.GetAttributeValue("href", "");
                                        if (!tempHref.Contains(sheng)) tempHref = sheng + tempHref;
                                        var hrefItemStr = url + tempHref;
                                        var jiedaoHtml = DownloadPage(hrefItemStr);
                                        HtmlDocument jiedaoHtmlDocument = new HtmlDocument();
                                        jiedaoHtmlDocument.LoadHtml(jiedaoHtml);

                                        var xStr = jiedaoHtmlDocument.DocumentNode.SelectSingleNode("//*[@id='page_left']/div[3]/div[2]/table");
                                        if (xStr == null)
                                            continue;
                                        if (xStr.InnerText.Contains("人口数量") && xStr.InnerText.Contains("人口密度"))
                                        {
                                            //县
                                            InsertHref(hrefItemStr, 3);
                                        }
                                        var xiangzhens = jiedaoHtmlDocument.DocumentNode.SelectNodes("//*[@id='page_left']/div[5]/table");
                                        if (xiangzhens == null)
                                            continue;

                                        foreach (var xiangzhen in xiangzhens)
                                        {
                                            var xiangzhenhrefs = xiangzhen.SelectNodes(".//strong/a");
                                            if (xiangzhenhrefs == null)
                                                continue;
                                            foreach (var xiangzhenhref in xiangzhenhrefs)
                                            {
                                                tempHref = xiangzhenhref.GetAttributeValue("href", "");
                                                if (!tempHref.Contains(sheng)) tempHref = sheng + tempHref;
                                                var xiangzhenHrefItemStr = url + tempHref;
                                                var xiangzhenHtml = DownloadPage(xiangzhenHrefItemStr);
                                                HtmlDocument xiangzhenHtmlDocument = new HtmlDocument();
                                                xiangzhenHtmlDocument.LoadHtml(xiangzhenHtml);//*[@id="page_left"]/div[3]/div[2]/table
                                                var xzSr = xiangzhenHtmlDocument.DocumentNode.SelectSingleNode("//*[@id='page_left']/div[3]/div[2]/table");
                                                if (xzSr == null)
                                                    continue;
                                                if (xzSr.InnerText.Contains("人口数量") && xzSr.InnerText.Contains("人口密度"))
                                                {
                                                    //乡镇
                                                    InsertHref(xiangzhenHrefItemStr, 4);
                                                }
                                            }
                                        }
                                    }
                                }

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"已经停止,异常：{ex.Message}");
            }
            Console.ReadKey();
        }

        private void InsertDB(Area item)
        {
            var dt = MtimeHelper.Query("select * from 地区表 where 1!=1").Tables[0];
            var dateNow = DateTime.Now;

            DataRow dr = dt.NewRow();
            dr["地名"] = string.IsNullOrEmpty(item.地名) ? "" : item.地名;
            dr["隶属"] = string.IsNullOrEmpty(item.隶属) ? "" : item.隶属;
            dr["行政代码"] = string.IsNullOrEmpty(item.行政代码) ? "" : item.行政代码;
            dr["代码前6位"] = string.IsNullOrEmpty(item.代码前6位) ? "" : item.代码前6位;
            dr["辖区面积"] = string.IsNullOrEmpty(item.辖区面积) ? "" : item.辖区面积;
            dr["人口密度"] = string.IsNullOrEmpty(item.人口密度) ? "" : item.人口密度;
            dr["人口数量"] = string.IsNullOrEmpty(item.人口数量) ? "" : item.人口数量;
            dr["邮政编码"] = string.IsNullOrEmpty(item.邮政编码) ? "" : item.邮政编码;
            dt.Rows.Add(dr);
            MtimeHelper.BulkToDB("地区表", dt);
        }
        private void InsertHref(string href, int level)
        {
            Console.WriteLine("已经发现" + ++count + "个行政区划");
            var dt = MtimeHelper.Query("select * from 地区Href where 1!=1").Tables[0];
            var dateNow = DateTime.Now;

            DataRow dr = dt.NewRow();
            dr["Href"] = href;
            dr["AreaLevel"] = level;
            dr["CreateDate"] = DateTime.Now;
            dt.Rows.Add(dr);
            MtimeHelper.BulkToDB("地区Href", dt);
        }
        private object AnalysisHtml(string html, string filmName, int filmID)
        {
            throw new NotImplementedException();
        }

        public override string DownloadPage(string url)
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
            return HttpResult.Html;
        }

        public override DataTable GetAsyncTable()
        {
            throw new NotImplementedException();
        }
    }
}
