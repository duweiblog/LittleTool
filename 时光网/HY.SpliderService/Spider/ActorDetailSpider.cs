using HtmlAgilityPack;
using HY.SpiderService.Module;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HY.SpiderService.Spider
{
    public class ActorDetailSpider : BaseSpider
    {
        public HttpResult HttpResult { get; set; } 
        public override void Begin()
        {
            Console.WriteLine("任务启动...");
            try
            {
                var dt = GetAsyncTable();
                for (var i = 0; i <= dt.Rows.Count; i++)
                {
                    try
                    {
                        Thread.Sleep(200);
                        var url = dt.Rows[i]["ActorHref"].ToString();
                        var ActorID = dt.Rows[i]["ActorID"].ToString();
                        var FilmID = dt.Rows[i]["FilmID"].ToString();
                        Console.WriteLine($"开始获取ActorID={ActorID}...");
                        var html = DownloadPage(url);
                        if (html.Contains("302 Found"))
                        {
                            continue;
                        }
                        var act = AnalysisHtml(html, ActorID, url);
                        act.FilmID = int.Parse(FilmID);
                        InsertDB(act);
                        Console.WriteLine($"插入数据库...");
                    }
                    catch (Exception ex)
                    {
                        //i--;
                        Console.WriteLine(ex.Message);
                        Thread.Sleep(1000);
                        //Console.WriteLine("按任意键继续！");
                        //Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"已经停止,异常：{ex.Message}");
            }
            Console.ReadKey();
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
                //WebProxy = new WebProxy(pxy.ProxyIP, int.Parse(pxy.Port))
            };
            HttpResult = http.GetHtml(item);
            var rs = HttpResult.Html;
            return HttpResult.Html;
        }
        public void InsertDB(ActorDetail act)
        {
            var dt = MtimeHelper.Query(@"SELECT *
                                          FROM [dbo].[ActorDetail]
                                         where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            DataRow dr = dt.NewRow();
            dr["ActorID"] = string.IsNullOrEmpty(act.ActorID) ? "无" : act.ActorID;
            dr["ActorName"] = string.IsNullOrEmpty(act.ActorName) ? "无" : act.ActorName.Transferred();
            dr["ActorOtherName"] = string.IsNullOrEmpty(act.ActorOtherName) ? "无" : act.ActorOtherName.Transferred();
            dr["ActorHref"] = string.IsNullOrEmpty(act.ActorHref) ? "无" : act.ActorHref.Transferred();
            dr["ActorType"] = string.IsNullOrEmpty(act.ActorType) ? "无" : act.ActorType.Transferred();
            dr["ActorBirth"] = act.ActorBirth;
            dr["ActorPic"] = string.IsNullOrEmpty(act.ActorPic) ? "无" : act.ActorPic.Transferred();
            dr["FilmID"] = act.FilmID;
            dt.Rows.Add(dr);
            MtimeHelper.BulkToDB("ActorDetail", dt);
        }
        public ActorDetail AnalysisHtml(string html, string ActorID, string href)
        {
            ///html/body/div[5]/div/div/div[1]/div/dl[1]
            var MovieList = new List<ActorDetail>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);///*[@id="per_head"]/div[3]/div
            HtmlNodeCollection yanyuan = htmlDocument.DocumentNode.SelectNodes("//*[@id='per_head']/div[3]/div");
            //*[@id='personDetailRegion']/div[1]/span/a/img
            HtmlNode img = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='personDetailRegion']/div[1]/span/a/img");
            HtmlNode birthday = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='birthdayRegion']");

            HtmlNode detail = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='per_head']/div[3]/div");
            ActorDetail actDetail = new ActorDetail();
            actDetail.ActorID = ActorID;
            actDetail.ActorHref = href;
            try
            {
                try
                {
                    var name = detail.ChildNodes[1].InnerText.ToString().Trim();
                    actDetail.ActorName = name.Transferred();
                    var othername = detail.ChildNodes[3].InnerText.ToString().Trim();
                    actDetail.ActorOtherName = othername.Transferred();

                    var ActorType = detail.ChildNodes[5].InnerText.ToString().Trim();
                    actDetail.ActorType = ActorType.Transferred();
                }
                catch (Exception ex) { }
                if (img != null)
                {
                    var pic = img.GetAttributeValue("src", "").Replace("\\\"", "");
                    if (!string.IsNullOrEmpty(pic))
                    {
                        http.HttpDownload(pic, "d:\\时光网\\ActorPic\\" + actDetail.ActorName + ".jpg");
                        actDetail.ActorPic = actDetail.ActorName + ".jpg";
                    }
                }
                if (birthday != null)
                {
                    var birStr = birthday.GetAttributeValue("birth", "").Replace("\\\"", "");
                    try
                    {
                        actDetail.ActorBirth = string.IsNullOrEmpty(birStr) ? new DateTime(1780, 1, 1) : DateTime.Parse(birStr);
                        if (actDetail.ActorBirth < new DateTime(1780, 1, 1))
                        {
                            actDetail.ActorBirth = new DateTime(1780, 1, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        actDetail.ActorBirth = new DateTime(1780, 1, 1);
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return actDetail;
        }

        /// <summary>
        /// 下载页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public override DataTable GetAsyncTable()
        {
            return MtimeHelper.Query(@"select * from (select  row_number()over(partition by Actorhref order by ActorID asc) num,* from Actor  )tab where tab.num=1 and ActorID>=(select (isnull(max(ActorID),0)+1)ActorID from ActorDetail)  order by ActorID asc").Tables[0];
        }
    }
}
