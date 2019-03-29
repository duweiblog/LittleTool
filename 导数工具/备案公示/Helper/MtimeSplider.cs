using HtmlAgilityPack;
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

namespace HY.SpliderService
{
    public class MtimeSplider
    {
        public static int BeginLine = 2;
        public HttpResult HttpResult { get; set; }
        public static HttpHelper http = new HttpHelper();
        public string Domain = "";
        private int proxyIndex = 1;

        public MtimeSplider()
        {
        }

        public static Proxy GetProxy()
        {
            var dt = DbHelperSQL.Query("select top 1 * from Proxy order by  updatedate asc").Tables[0];

            Proxy pxy = new Proxy();
            pxy.ID = dt.Rows[0]["ID"].ToString();
            pxy.Port = dt.Rows[0]["Port"].ToString();
            pxy.ProxyIP = dt.Rows[0]["ProxyIP"].ToString();

            return pxy;
        }


        public static DataTable GetMtimeFilm()
        {
            return DbHelperSQL.Query(@"select  ID, DetailUrl from Mtime_Film  a LEFT JOIN FilmInfo b ON a.ID=b.Mtime_FilmID
                                        WHERE b.Mtime_FilmID IS NULL").Tables[0];
        }

        public static void UpdateProxy(string id)
        {
            DbHelperSQL.ExecuteSql("update Proxy set updatedate=getdate() where id=" + id);
        }

        public static void DelProxy(string id)
        {
            DbHelperSQL.ExecuteSql("delete from  Proxy where id=" + id);
        }

        public int GetAsyncPage()
        {
            return int.Parse(DbHelperSQL.GetSingleVal("select isnull(MAX(page),0) page  from Mtime_Film ").ToString());
        }

        /// <summary>
        /// 下载页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string DownloadPage(string url)
        {
            var pxy = GetProxy();
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
            UpdateProxy(pxy.ID);
            Console.WriteLine($"正在使用代理:{pxy.ProxyIP}:{pxy.Port}");

            var rs = HttpResult.Html;
            if (rs == "无法连接到远程服务器")
            {
                DelProxy(pxy.ID);
                DownloadPage(url);
            }
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

        /// <summary>
        /// 解析影片详情页
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public void AnalysisDetail()
        {
            var dt = GetMtimeFilm();
            if (!Directory.Exists("d:\\FilmPic\\"))
            {
                Directory.CreateDirectory("d:\\FilmPic\\");
            }
            for (int n = 0; n < dt.Rows.Count; n++)
            {
                FilmInfo fi = new FilmInfo();
                var url = dt.Rows[n]["DetailUrl"].ToString();
                var id = dt.Rows[n]["ID"].ToString();
                Console.WriteLine("等待下载页面" + url);
                Thread.Sleep(200);
                var page = DownloadPage(url);
                Console.WriteLine("页面下载完成");
                var MovieList = new List<Movie>();
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(page);
                HtmlNode filmPic = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[1]/div/div/div/a/img");
                fi.FilmPic = filmPic == null ? "" : filmPic.GetAttributeValue("src", "");
                //*[@id="db_head"]/div[2]/div/div[1]/h1
                HtmlNode filmName = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[2]/div/div[1]/h1");
                fi.FilmName = filmName == null ? "" : filmName.InnerText;
                if (!string.IsNullOrEmpty(fi.FilmPic))
                {
                    var picName = fi.FilmName + Path.GetExtension(fi.FilmPic);
                    http.HttpDownload(fi.FilmPic, "d:\\FilmPic\\" + picName);
                    fi.FilmPic = picName;
                }
                //*[@id="db_head"]/div[2]/div/div[1]/p[1]/a
                HtmlNode filmYear = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[2]/div/div[1]/p[1]/a");
                fi.FilmYear = filmYear == null ? "" : filmYear.InnerText;
                //*[@id="db_head"]/div[2]/div/div[1]/p[2]
                HtmlNode filmOtherYear = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[2]/div/div[1]/p[2]");
                fi.FilmOtherName = filmOtherYear == null ? "" : filmOtherYear.InnerText;
                //*[@id="db_head"]/div[2]/div/div[2]/a
                HtmlNodeCollection filmTypes = htmlDocument.DocumentNode.SelectNodes("//*[@id='db_head']/div[2]/div/div[2]/a[@property='v:genre']");
                var types = new List<string>();
                if (filmTypes != null)
                {
                    foreach (var item in filmTypes)
                    {
                        types.Add(item.InnerText);
                    }
                }
                fi.FilmTypes = string.Join("|", types.ToArray());
                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[1]/a
                HtmlNode filmDirector = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='movie_warp']/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[1]/a");
                fi.FilmDirector = filmDirector == null ? "" : filmDirector.InnerText;
                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[2]/a[1]
                HtmlNodeCollection filmScreenwriter = htmlDocument.DocumentNode.SelectNodes("//*[@id='movie_warp']/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[2]/a");

                var screenwriter = new List<string>();
                if (filmScreenwriter != null)
                {
                    foreach (var item in filmScreenwriter)
                    {
                        if (item.InnerText.Trim().Equals("..."))
                            continue;
                        screenwriter.Add(item.InnerText);
                    }
                }
                fi.FilmScreenwriter = string.Join("|", screenwriter.ToArray());

                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[3]/a[1]
                HtmlNode coutry = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='movie_warp']/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[3]/a[1]");
                fi.FilmCountry = coutry == null ? "" : coutry.InnerText;
                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dt/p[1]
                HtmlNode filmBrief = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='movie_warp']/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dt/p[1]");
                fi.FilmBrief = filmBrief == null ? "" : filmBrief.InnerText;
                //*[@id="db_head"]/div[2]/div/div[2]/span
                HtmlNode filmLenth = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[2]/div/div[2]/span");
                fi.FilmLenth = filmLenth == null ? "" : filmLenth.InnerText;

                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/div/dl/dd/p[1]/a
                HtmlNodeCollection filmActors = htmlDocument.DocumentNode.SelectNodes("//*[@id='movie_warp']/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/div/dl/dd/p[1]/a");

                var actors = new List<string>();
                if (filmActors != null)
                {
                    foreach (var item in filmActors)
                    {
                        if (item.InnerText.Trim().Equals("..."))
                            continue;
                        actors.Add(item.InnerText);
                    }
                }
                fi.FilmActor = string.Join("|", actors.ToArray());
                fi.FilmID = Guid.NewGuid().ToString();
                fi.Mtime_FilmID = id;
                InsertFilm(fi);
            }
        }

        public void Begin()
        {
            Console.WriteLine("任务启动...");
            MtimeSplider mtime = new MtimeSplider();
            try
            {
                var i = mtime.GetAsyncPage() + 1;
                for (; i <= 97; i++)
                {
                    try
                    {
                        Thread.Sleep(200);
                        Console.WriteLine($"开始获取第{i}页...");
                        var Ajax_CallBackArgument18 = i;
                        var Ajax_CallBackArgument9 = "2018";
                        var Ajax_CallBackArgument10 = "2018";
                        var time = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "11";
                        var url = "http://service.channel.mtime.com/service/search.mcs?Ajax_CallBack=true&Ajax_CallBackType=Mtime.Channel.Pages.SearchService&Ajax_CallBackMethod=SearchMovieByCategory&Ajax_CrossDomain=1&Ajax_RequestUrl=http%3A%2F%2Fmovie.mtime.com%2Fmovie%2Fsearch%2Fsection%2F%23pageIndex%3D1%26year%3D2018&t=" + time + "&Ajax_CallBackArgument0=&Ajax_CallBackArgument1=181210151826556601&Ajax_CallBackArgument2=0&Ajax_CallBackArgument3=0&Ajax_CallBackArgument4=0&Ajax_CallBackArgument5=0&Ajax_CallBackArgument6=0&Ajax_CallBackArgument7=0&Ajax_CallBackArgument8=&Ajax_CallBackArgument9=" + Ajax_CallBackArgument9 + "&Ajax_CallBackArgument10=" + Ajax_CallBackArgument10 + "&Ajax_CallBackArgument11=0&Ajax_CallBackArgument12=0&Ajax_CallBackArgument13=0&Ajax_CallBackArgument14=1&Ajax_CallBackArgument15=0&Ajax_CallBackArgument16=1&Ajax_CallBackArgument17=4&Ajax_CallBackArgument18=" + Ajax_CallBackArgument18 + "&Ajax_CallBackArgument19=0";
                        var html = mtime.DownloadPage(url);
                        //var html = mtime.DownloadPage("http://service.channel.mtime.com/service/search.mcs?Ajax_CallBack=true&Ajax_CallBackType=Mtime.Channel.Pages.SearchService&Ajax_CallBackMethod=SearchMovieByCategory&Ajax_CrossDomain=1&Ajax_RequestUrl=http%3A%2F%2Fmovie.mtime.com%2Fmovie%2Fsearch%2Fsection%2F%23pageIndex%3D1%26year%3D2018&t=2018121015183125980&Ajax_CallBackArgument0=8mrd&Ajax_CallBackArgument1=181210151826556601&Ajax_CallBackArgument2=0&Ajax_CallBackArgument3=0&Ajax_CallBackArgument4=0&Ajax_CallBackArgument5=0&Ajax_CallBackArgument6=0&Ajax_CallBackArgument7=0&Ajax_CallBackArgument8=&Ajax_CallBackArgument9=2018&Ajax_CallBackArgument10=2018&Ajax_CallBackArgument11=0&Ajax_CallBackArgument12=0&Ajax_CallBackArgument13=0&Ajax_CallBackArgument14=1&Ajax_CallBackArgument15=0&Ajax_CallBackArgument16=1&Ajax_CallBackArgument17=4&Ajax_CallBackArgument18=2&Ajax_CallBackArgument19=0");
                        var beginIndex = html.IndexOf("<ul");
                        var endIndex = html.IndexOf("ul>");
                        Console.WriteLine($"{beginIndex}-{endIndex}");
                        var rs = html.Substring(beginIndex, endIndex - beginIndex + 3);
                        var ls = mtime.AnalysisCountry(html, i);
                        mtime.InsertDB(ls);
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

        public void BeginDetail()
        {
            AnalysisDetail();
        }


        public void InsertDB(List<Movie> movies)
        {
            var dt = DbHelperSQL.Query("select * from Mtime_Film where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            foreach (var item in movies)
            {
                DataRow dr = dt.NewRow();
                dr["FilmName"] = item.FilmName;
                dr["PicUrl"] = item.PicUrl;
                dr["DetailUrl"] = item.DetailUrl;
                dr["Page"] = item.Page;
                dt.Rows.Add(dr);
            }
            DbHelperSQL.BulkToDB("Mtime_Film", dt);
        }
        public void InsertFilm(FilmInfo item)
        {
            var dt = DbHelperSQL.Query("select * from FilmInfo where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            DataRow dr = dt.NewRow();
            dr["FilmID"] = item.FilmID;
            dr["FilmName"] = item.FilmName;
            dr["FilmTypes"] = item.FilmTypes;
            dr["FilmLenth"] = item.FilmLenth;
            dr["FilmDirector"] = item.FilmDirector;
            dr["FilmActor"] = item.FilmActor;
            dr["FilmBrief"] = item.FilmBrief;
            dr["FilmYear"] = item.FilmYear;
            dr["FilmOtherName"] = item.FilmOtherName;
            dr["CreateDate"] = DateTime.Now;
            dr["FilmCountry"] = item.FilmCountry;
            dr["FilmScreenwriter"] = item.FilmScreenwriter;
            dr["Mtime_FilmID"] = item.Mtime_FilmID;
            dr["FilmPic"] = item.FilmPic;
            dt.Rows.Add(dr);
            DbHelperSQL.BulkToDB("FilmInfo", dt);
        }
        public class Movie
        {
            public string FilmName { get; set; }
            public string PicUrl { get; set; }
            public string DetailUrl { get; set; }
            public int Page { get; set; }
        }
        public class Proxy
        {
            public string ID { get; set; }
            public string ProxyIP { get; set; }
            public string Port { get; set; }
        }

        public class FilmInfo
        {
            public FilmInfo()
            {
                this.FilmID = string.Empty;
                this.FilmNo = string.Empty;
                this.FilmName = string.Empty;
                this.FilmKind = string.Empty;
                this.FilmProducer = string.Empty;
                this.FilmQuickFind = string.Empty;
                this.FilmLenth = string.Empty;
                this.FilmDirector = string.Empty;
                this.FilmActor = string.Empty;
                this.FilmBrief = string.Empty;
                this.FilmYear = string.Empty;
            }
            /// <summary>
            /// 影片主键ID
            /// </summary>
            public string FilmID { get; set; }
            /// <summary>
            /// 影片编号
            /// </summary>
            public string FilmNo { get; set; }
            /// <summary>
            /// 影片名称
            /// </summary>
            public string FilmName { get; set; }
            /// <summary>
            /// 影片片种
            /// </summary>
            public string FilmKind { get; set; }

            /// <summary>
            /// 影片题材
            /// </summary>
            public string FilmTypes { get; set; }

            /// <summary>
            /// 影片出品商
            /// </summary>
            public string FilmProducer { get; set; }
            /// <summary>
            /// 影片快速检索
            /// </summary>
            public string FilmQuickFind { get; set; }
            /// <summary>
            /// 影片时长
            /// </summary>
            public string FilmLenth { get; set; }
            /// <summary>
            /// 影片导演
            /// </summary>
            public string FilmDirector { get; set; }
            /// <summary>
            /// 影片主演
            /// </summary>
            public string FilmActor { get; set; }
            /// <summary>
            /// 影片简介
            /// </summary>
            public string FilmBrief { get; set; }
            /// <summary>
            /// 影片出品年代
            /// </summary>
            public string FilmYear { get; set; }

            /// <summary>
            /// 影片海报
            /// </summary>
            public string FilmPic { get; set; }
            /// <summary>
            /// 影片别名
            /// </summary>
            public string FilmOtherName { get; set; }
            /// <summary>
            /// 国别
            /// </summary>
            public string FilmCountry { get; set; }
            /// <summary>
            /// 编剧
            /// </summary>
            public string FilmScreenwriter { get; set; }

            public string Mtime_FilmID { get; set; }
        }
    }
}
