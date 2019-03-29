using HtmlAgilityPack;
using HY.SpiderService.Module;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HY.SpiderService.Spider
{
    public class FilmSpider : BaseSpider
    {  
        private int syear = int.Parse(ConfigurationOperator.GetValue("syear"));
        private int pageCount = int.Parse(ConfigurationOperator.GetValue("pageCount")); 
        public FilmSpider()
        {
        }

        public int GetAsyncPage(int year)
        {
            return int.Parse(MtimeHelper.GetSingleVal("select isnull(MAX(page),0) page  from Mtime_Film where [YEAR]= " + year).ToString());
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
            };
            HttpResult = http.GetHtml(item); 

            var rs = HttpResult.Html; 
            return HttpResult.Html;
        }

        /// <summary>
        /// 解析国家地区
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Movie> AnalysisCountry(string page, int pageIndex)
        {
            var MovieList = new List<Movie>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(page);
            HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectNodes("/ul/li/div");

            for (int i = 0; i < collection.Count(); i++)
            {
                var detail = collection[i].SelectSingleNode("div/div[1]/a");
                var img = collection[i].SelectSingleNode("div/div[1]/a/img");
                var movie = new Movie();
                movie.FilmName = img.GetAttributeValue("alt", "").Replace("\\\"", "");
                movie.PicUrl = img.GetAttributeValue("src", "").Replace("\\\"", "");
                movie.DetailUrl = detail.GetAttributeValue("href", "").Replace("\\\"", "");
                movie.Page = pageIndex;
                MovieList.Add(movie);
            }
            return MovieList;
        }
 
        public override void Begin()
        {
            Console.WriteLine("任务启动...");
            FilmSpider mtime = new FilmSpider();
            try
            {
                var i = mtime.GetAsyncPage(syear) + 1;
                for (; i <= pageCount; i++)
                {
                    try
                    {
                        Thread.Sleep(200);
                        Console.WriteLine($"开始获取第{i}页...");
                        var Ajax_CallBackArgument18 = i;
                        var Ajax_CallBackArgument9 = syear;
                        var Ajax_CallBackArgument10 = syear;
                        var time = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "11";
                        var url = "http://service.channel.mtime.com/service/search.mcs?Ajax_CallBack=true&Ajax_CallBackType=Mtime.Channel.Pages.SearchService&Ajax_CallBackMethod=SearchMovieByCategory&Ajax_CrossDomain=1&Ajax_RequestUrl=http%3A%2F%2Fmovie.mtime.com%2Fmovie%2Fsearch%2Fsection%2F%23pageIndex%3D1%26year%3D2018&t=" + time + "&Ajax_CallBackArgument0=&Ajax_CallBackArgument1=181210151826556601&Ajax_CallBackArgument2=0&Ajax_CallBackArgument3=0&Ajax_CallBackArgument4=0&Ajax_CallBackArgument5=0&Ajax_CallBackArgument6=0&Ajax_CallBackArgument7=0&Ajax_CallBackArgument8=&Ajax_CallBackArgument9=" + Ajax_CallBackArgument9 + "&Ajax_CallBackArgument10=" + Ajax_CallBackArgument10 + "&Ajax_CallBackArgument11=0&Ajax_CallBackArgument12=0&Ajax_CallBackArgument13=0&Ajax_CallBackArgument14=1&Ajax_CallBackArgument15=0&Ajax_CallBackArgument16=1&Ajax_CallBackArgument17=4&Ajax_CallBackArgument18=" + Ajax_CallBackArgument18 + "&Ajax_CallBackArgument19=0";
                        var html = mtime.DownloadPage(url);
                        if (html.Contains("302 Found"))
                        {
                            continue;
                        }
                        //var html = mtime.DownloadPage("http://service.channel.mtime.com/service/search.mcs?Ajax_CallBack=true&Ajax_CallBackType=Mtime.Channel.Pages.SearchService&Ajax_CallBackMethod=SearchMovieByCategory&Ajax_CrossDomain=1&Ajax_RequestUrl=http%3A%2F%2Fmovie.mtime.com%2Fmovie%2Fsearch%2Fsection%2F%23pageIndex%3D1%26year%3D2018&t=2018121015183125980&Ajax_CallBackArgument0=8mrd&Ajax_CallBackArgument1=181210151826556601&Ajax_CallBackArgument2=0&Ajax_CallBackArgument3=0&Ajax_CallBackArgument4=0&Ajax_CallBackArgument5=0&Ajax_CallBackArgument6=0&Ajax_CallBackArgument7=0&Ajax_CallBackArgument8=&Ajax_CallBackArgument9=2018&Ajax_CallBackArgument10=2018&Ajax_CallBackArgument11=0&Ajax_CallBackArgument12=0&Ajax_CallBackArgument13=0&Ajax_CallBackArgument14=1&Ajax_CallBackArgument15=0&Ajax_CallBackArgument16=1&Ajax_CallBackArgument17=4&Ajax_CallBackArgument18=2&Ajax_CallBackArgument19=0");
                        var beginIndex = html.IndexOf("<ul");
                        var endIndex = html.IndexOf("ul>");
                        Console.WriteLine($"{beginIndex}-{endIndex}");
                        var rs = html.Substring(beginIndex, endIndex - beginIndex + 3);
                        var ls = mtime.AnalysisCountry(html, i);
                        mtime.InsertDB(ls, syear);
                        Console.WriteLine($"获取第{i}页完成入库...");
                    }
                    catch (Exception ex)
                    {
                        i--;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("按任意键继续！");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"已经停止,异常：{ex.Message}");
            }
            Console.ReadKey();
        }
         
        public void InsertDB(List<Movie> movies, int year)
        {
            var dt = MtimeHelper.Query("select * from Mtime_Film where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            foreach (var item in movies)
            {
                DataRow dr = dt.NewRow();
                dr["FilmName"] = item.FilmName;
                dr["PicUrl"] = item.PicUrl;
                dr["DetailUrl"] = item.DetailUrl;
                dr["Page"] = item.Page;
                dr["Year"] = year;
                dt.Rows.Add(dr);
            }
            MtimeHelper.BulkToDB("Mtime_Film", dt);
        } 

        public override DataTable GetAsyncTable()
        {
            throw new NotImplementedException();
        }
    }
}
