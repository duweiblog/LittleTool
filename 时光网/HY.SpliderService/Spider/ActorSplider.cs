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
    /// <summary>
    /// 演员 
    /// </summary>
    public class ActorSpider : BaseSpider
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
                        var FilmHref = dt.Rows[i]["FilmHref"].ToString();
                        var FilmID = int.Parse(dt.Rows[i]["FilmID"].ToString());
                        var FilmName = dt.Rows[i]["FilmName"].ToString();
                        Console.WriteLine($"开始获取ID={FilmID}...");
                        var html = DownloadPage(FilmHref);
                        if (html.Contains("302 Found"))
                        {
                            continue;
                        }
                        var act = AnalysisActor(html, FilmName, FilmID);
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
        public void InsertDB(List<Actor> act)
        {
            var dt = MtimeHelper.Query("select * from Actor where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            foreach (var item in act)
            {
                DataRow dr = dt.NewRow();
                dr["ActorName"] = string.IsNullOrEmpty(item.ActorName) ? "无" : item.ActorName.Transferred();
                dr["ActorHref"] = string.IsNullOrEmpty(item.ActorHref) ? "无" : item.ActorHref.Transferred();
                dr["ActorType"] = string.IsNullOrEmpty(item.ActorType) ? "无" : item.ActorType.Transferred();
                dr["FilmName"] = string.IsNullOrEmpty(item.FilmName) ? "无" : item.FilmName.Transferred();
                dr["FilmID"] = item.FilmID;
                dr["CreateDate"] = DateTime.Now;
                dt.Rows.Add(dr);
            }
            MtimeHelper.BulkToDB("Actor", dt);
        }
        public List<Actor> AnalysisActor(string html, string filmname, int FilmID)
        {
            ///html/body/div[5]/div/div/div[1]/div/dl[1]
            var MovieList = new List<Actor>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);////html/body/div[5]/div/div/div[1]/div/dl[1]/dd[1]/div[1]
            HtmlNodeCollection yanyuan = htmlDocument.DocumentNode.SelectNodes("//div[@class='actor_tit']");
            //演员
            for (int i = 1; i < yanyuan.Count(); i++)
            {
                try
                {
                    var aflag = yanyuan[i].ChildNodes[1].SelectSingleNode("a");
                    var act = new Actor();
                    act.ActorHref = aflag.GetAttributeValue("href", "").Replace("\\\"", "").Transferred();
                    act.FilmName = filmname.Transferred();
                    act.ActorType = "演员";
                    act.ActorName = (yanyuan[i].ChildNodes[1].ChildNodes[3]).ChildNodes[0].InnerHtml.Transferred();
                    act.FilmID = FilmID;
                    MovieList.Add(act);
                }
                catch (Exception ex)
                {

                }
            }

            HtmlNodeCollection daoyan = htmlDocument.DocumentNode.SelectNodes("//div[@class='credits_list']");
            //导演
            for (int i = 1; i < daoyan.Count(); i++)
            {
                try
                {
                    var act = new Actor();
                    act.ActorHref = daoyan[i].ChildNodes[3].ChildNodes[1].GetAttributeValue("href", "").Replace("\\\"", "");
                    act.FilmName = filmname;
                    act.ActorType = daoyan[i].ChildNodes[1].InnerText.Trim();
                    act.ActorName = daoyan[i].ChildNodes[3].InnerText.Trim();
                    act.FilmID = FilmID;
                    MovieList.Add(act);
                }
                catch (Exception ex)
                {

                }
            }
            return MovieList;
        }

        /// <summary>
        /// 下载页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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
        public override DataTable GetAsyncTable()
        {
            //return MtimeHelper.Query(@"select a.FilmID,a.Filmhref+'fullcredits.html' FilmHref,a.FilmName from   FilmInfo a left join Actor b on a.FilmID =b.FilmID
            //                            where b.FilmID is null	
            //                            order by a.FilmID asc").Tables[0];

            return MtimeHelper.Query(@" select  a.FilmID,a.Filmhref+'fullcredits.html' FilmHref,a.FilmName from FilmInfo a where filmID>(
                                        select  max(FilmID)  from Actor)").Tables[0];
        } 
    }
}
