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
    public class TCmapDetailSpider : BaseSpider
    {
        private HttpResult HttpResult;
        private string url = "http://www.tcmap.com.cn/";
        private int count = 0;
        public override void Begin()
        {
            Console.WriteLine("任务启动...");
            try
            {
                var start = GetAsyncTable();
                if (start.Rows.Count <= 0)
                {
                    return;
                }

                foreach (DataRow item in start.Rows)
                {
                    try
                    {
                        var area = new Area();
                        var AreaLevel = int.Parse(item["AreaLevel"].ToString());
                        var Href = item["Href"].ToString();
                        var ID = item["ID"].ToString();
                        area.ID = ID;
                        Console.WriteLine($"正在下载数据{ID}");
                        var html = DownloadPage(Href);
                        HtmlDocument cityHtmlDocument = new HtmlDocument();
                        cityHtmlDocument.LoadHtml(html);
                        var cityDetailNode = cityHtmlDocument.DocumentNode.SelectSingleNode("//*[@id='page_left']/div[3]/div[2]/table/tr");
                        //*[@id="page_left"]/h1
                        if (AreaLevel == 1)
                        {
                            var diming = cityHtmlDocument.DocumentNode.SelectSingleNode("//*[@id='page_left']/h1").InnerText;
                            var mianji = cityHtmlDocument.DocumentNode.SelectSingleNode("//*[@id='page_left']/table[2]/tr/td[1]/table/tr[1]/td[2]");
                            var renkou = cityHtmlDocument.DocumentNode.SelectSingleNode("//*[@id='page_left']/table[2]/tr/td[1]/table/tr[2]/td[2]");

                            area.地名 = diming;
                            area.辖区面积 = mianji.InnerText.Replace("面积:", "");
                            area.人口数量 = renkou.InnerText.Replace("人口:", "");
                        }
                        else
                        {
                            //地名
                            var areaName = cityDetailNode.SelectSingleNode("//tr[1]/td[1]").InnerText.Replace("地名:", "");
                            //隶属
                            var lishu = cityDetailNode.SelectSingleNode("//tr[1]/td[2]").InnerText.Replace("隶属:", "");
                            //行政代码
                            var code = cityDetailNode.SelectSingleNode("//tr[2]/td[1]").InnerText.Replace("行政代码:", "");
                            //行政代码前6位
                            var code6 = cityDetailNode.SelectSingleNode("//tr[2]/td[2]").InnerText.Replace("代码前6位:", "");
                            //邮政编码
                            var youbian = cityDetailNode.SelectSingleNode("//tr[3]/td[2]").InnerText.Replace("邮政编码:", "");
                            //人口数量
                            var rksl = cityDetailNode.SelectSingleNode("//tr[5]/td[1]").InnerText.Replace("人口数量:", "");

                            //人口密度
                            var midu = "";
                            //辖区面积
                            var xqmj = "";
                            try
                            {
                                midu = cityDetailNode.SelectSingleNode("//tr[5]/td[2]").InnerText.Replace("人口密度:", "");
                                xqmj = cityDetailNode.SelectSingleNode("//tr[6]/td[1]").InnerText.Replace("辖区面积:", "");
                            }
                            catch (Exception ex) { }
                            area.地名 = areaName;
                            area.隶属 = lishu;
                            area.行政代码 = code;
                            area.代码前6位 = code6;
                            area.邮政编码 = youbian;
                            area.人口数量 = rksl;
                            area.人口密度 = midu;
                            area.辖区面积 = xqmj;
                        }
                        InsertDB(area);
                    }
                    catch (Exception ex) { }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"已经停止,异常：{ex.Message}");
            }
            //Console.ReadKey();
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
            dr["创建时间"] = DateTime.Now;
            dr["ID"] = item.ID;
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
            return MtimeHelper.Query("select * from 地区Href where ID>(  select isnull(max(ID),0) from 地区表)  order by ID asc").Tables[0];
        }
    }
}
