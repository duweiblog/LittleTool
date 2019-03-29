using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HY.SpiderService.Spider
{
    public class CompanySpider : BaseSpider
    {
        public override void Begin()
        {
            Console.WriteLine("任务启动...");
            FilmSpider mtime = new FilmSpider();
            try
            {
                var start = GetAsyncTable();
                if (start.Rows.Count <= 0)
                {
                    return;
                }
                foreach (DataRow item in start.Rows)
                {
                    var FilmHref = item["FilmHref"].ToString();
                    var FilmID = int.Parse(item["FilmID"].ToString());
                    var Filmname = item["Filmname"].ToString();
                    var htmlContent = DownloadPage(FilmHref);
                    Console.WriteLine($"开始获取FilmID={FilmID}...");
                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);
                    HtmlNodeCollection zhipian = htmlDocument.DocumentNode.SelectNodes("//*[@id='companyRegion']/dd/div[1]/div[1]/ul/li");
                    var zpType = "JG01"; var fxType = "JG02";
                    if (zhipian != null)
                        foreach (var jg in zhipian)
                        {
                            var jgNode = jg.SelectSingleNode(".//a");
                            var dqNode = jg.SelectSingleNode(".//span");
                            InsertDB(jgNode.InnerHtml.TrimStr().Transferred(), FilmID, zpType, dqNode.InnerHtml.TrimStr().Transferred());
                        }
                    HtmlNodeCollection faxing = htmlDocument.DocumentNode.SelectNodes("//*[@id='companyRegion']/dd/div[1]/div[2]/ul/li");
                    if (faxing != null)
                        foreach (var jg in faxing)
                        {
                            var jgNode = jg.SelectSingleNode(".//a");
                            var dqNode = jg.SelectSingleNode(".//span");
                            InsertDB(jgNode.InnerHtml.TrimStr().Transferred(), FilmID, fxType, dqNode.InnerHtml.TrimStr().Transferred());
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"已经停止,异常：{ex.Message}");
            }
        }
        public void InsertDB(string name, int filmID, string jgType, string diqu)
        {
            var dt = MtimeHelper.Query("select * from 机构表 where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            DataRow dr = dt.NewRow();
            dr["公司名称"] = name.Replace("&nbsp;","");
            dr["FilmID"] = filmID;
            dr["机构类别"] = jgType;
            dr["地区"] = string.IsNullOrEmpty(diqu) ? "" : diqu;
            dt.Rows.Add(dr);
            MtimeHelper.BulkToDB("机构表", dt);
        }
        public override string DownloadPage(string url)
        {
            var item = new HttpItem()
            {
                URL = url,
                Encoding = Encoding.GetEncoding("utf-8"),//编码格式
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
            return MtimeHelper.Query("select FilmID,Filmname,FilmHref+'details.html#company' FilmHref from FilmInfo where FilmID>(select isnull(max(FilmID),0) from 机构表)").Tables[0];
        }
    }
}
