
//public class  


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
    public class FilmDetailSpider : BaseSpider
    { 
        public HttpResult HttpResult { get; set; } 
        private string FilmPicPath = "d:\\时光网\\FilmPic\\";

        public FilmDetailSpider()
        {
        }

        public static DataTable GetAsyncRow()
        {
            //return MtimeHelper.Query(@"select  ID, DetailUrl from Mtime_Film  a LEFT JOIN FilmInfo b ON a.ID=b.Mtime_FilmID
            //                            WHERE b.Mtime_FilmID IS NULL
            //                            order by ID asc").Tables[0];

            return MtimeHelper.Query(@"select  ID, DetailUrl from Mtime_Film where ID>(select  max(Mtime_FilmID)  from FilmInfo)").Tables[0];
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
            //UpdateProxy(pxy.ID);
            //Console.WriteLine($"正在使用代理:{pxy.ProxyIP}:{pxy.Port}");

            var rs = HttpResult.Html;
            //if (rs == "无法连接到远程服务器")
            //{
            //    DelProxy(pxy.ID);
            //    DownloadPage(url);
            //}
            return HttpResult.Html;
        }
         
        /// <summary>
        /// 解析影片详情页
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public override void Begin()
        {
            var dt = GetAsyncRow();
            if (!Directory.Exists(FilmPicPath))
            {
                Directory.CreateDirectory(FilmPicPath);
            }
            for (int n = 0; n < dt.Rows.Count; n++)
            {
                FilmInfo fi = new FilmInfo();
                var url = dt.Rows[n]["DetailUrl"].ToString();
                fi.FilmHref = url;
                var id = dt.Rows[n]["ID"].ToString();
                Console.WriteLine("等待下载id=" + id);
                Thread.Sleep(200);
                var page = DownloadPage(url);
                if (page.Contains("302 Found"))//没有找到影片信息
                {
                    continue;
                }
                Console.WriteLine("下载完成");
                var MovieList = new List<Movie>();
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(page);
                HtmlNode filmPic = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[1]/div/div/div/a/img");
                fi.FilmPic = filmPic == null ? "" : filmPic.GetAttributeValue("src", "");
                //*[@id="db_head"]/div[2]/div/div[1]/h1
                HtmlNode filmName = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[2]/div/div[1]/h1");
                fi.FilmName = filmName == null ? "" : filmName.InnerText.Trim();
                if (string.IsNullOrEmpty(fi.FilmName))
                    continue;
                fi.FilmName = fi.FilmName.Transferred();
                if (!string.IsNullOrEmpty(fi.FilmPic))
                {
                    var picName = fi.FilmName + Path.GetExtension(fi.FilmPic);
                    http.HttpDownload(fi.FilmPic, FilmPicPath + picName);
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

                //if (filmTypes != null)
                //{
                //    foreach (var item in filmTypes)
                //    {
                //        types.Add(item.InnerText);
                //    }
                //}
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

                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[3]/strong

                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[3]/a[1]
                HtmlNode coutry = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='movie_warp']/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dd[3]/a[1]");
                fi.FilmCountry = coutry == null ? "" : coutry.InnerText;
                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dt/p[1]
                HtmlNode filmBrief = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='movie_warp']/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/dl/dt/p[1]");
                fi.FilmBrief = filmBrief == null ? "" : filmBrief.InnerText;
                //*[@id="db_head"]/div[2]/div/div[2]/span
                HtmlNode filmLenth = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[2]/div/div[2]/span");
                fi.FilmLenth = filmLenth == null ? "" : filmLenth.InnerText;
                //*[@id='db_head']/div[2]/div/div[2]
                fi.ShangYing = DateTime.Parse("1900-01-01");//默认
                var types = new List<string>();
                var titleNode = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='db_head']/div[2]/div/div[2]");
                if (titleNode != null)
                {
                    HtmlNodeCollection filmTitle = titleNode.ChildNodes;
                    for (int i = 0; i < filmTitle.Count; i++)
                    {
                        var filedType = filmTitle[i].GetAttributeValue("property", "").Trim();
                        switch (filedType)
                        {
                            case "v:runtime":
                                {
                                    //时长
                                    fi.FilmLenth = filmTitle[i].InnerText.Trim();
                                    break;
                                }
                            case "v:genre":
                                {
                                    //题材
                                    var ticai = filmTitle[i].InnerText.Trim();
                                    types.Add(ticai);
                                    break;
                                }
                            case "v:initialReleaseDate":
                                {
                                    //上映时间
                                    try
                                    {
                                        fi.ShangYing = DateTime.Parse(filmTitle[i].GetAttributeValue("content", "1900-01-01").Trim());
                                        if (fi.ShangYing < DateTime.Parse("1800-01-01"))
                                        {
                                            fi.ShangYing = DateTime.Parse("1900-01-01");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        fi.ShangYing = DateTime.Parse("1900-01-01");
                                    }
                                    break;
                                }
                            case "":
                                {
                                    var content = filmTitle[i].InnerText.Trim();
                                    if (content != "-" || content != "-" || content.Contains("上映"))
                                    {
                                        var ctts = content.Split(new[] { "-" }, StringSplitOptions.None);
                                        if (ctts.Length > 0)
                                        {
                                            fi.FilmCountry = ctts[0].Replace("上映", "");
                                        }
                                        if (ctts.Length > 1)
                                        {
                                            fi.ZhiShi = ctts[1];
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
                //是否有题材
                if (types.Count > 0)
                    fi.FilmTypes = string.Join("|", types.ToArray());
                //*[@id="movie_warp"]/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/div/dl/dd/p[1]/a
                HtmlNodeCollection filmActors = htmlDocument.DocumentNode.SelectNodes("//*[@id='movie_warp']/div[2]/div[3]/div/div[4]/div[2]/div[1]/div[2]/div[1]/div/dl/dd/p[1]/a");
                //*[@id="db_head"]/div[2]/div/div[2]/a[4]
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
                fi.Mtime_FilmID = id;
                InsertFilm(fi);
            }
        }

        public void InsertFilm(FilmInfo item)
        {
            var dt = MtimeHelper.Query("select * from FilmInfo where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            DataRow dr = dt.NewRow();
            dr["FilmName"] = item.FilmName.Transferred();
            dr["FilmTypes"] = item.FilmTypes.Transferred();
            dr["FilmLenth"] = item.FilmLenth;
            dr["FilmDirector"] = item.FilmDirector.Transferred();
            dr["FilmActor"] = item.FilmActor.Transferred();
            dr["FilmBrief"] = item.FilmBrief.Transferred();
            dr["FilmYear"] = item.FilmYear;
            dr["FilmOtherName"] = item.FilmOtherName.Transferred();
            dr["CreateDate"] = DateTime.Now;
            dr["FilmCountry"] = item.FilmCountry.Transferred();
            dr["FilmScreenwriter"] = item.FilmScreenwriter.Transferred();
            dr["Mtime_FilmID"] = item.Mtime_FilmID;
            dr["FilmPic"] = item.FilmPic.Transferred();
            dr["ShangYing"] = item.ShangYing;
            dr["ZhiShi"] = item.ZhiShi.Transferred();
            dr["FilmHref"] = item.FilmHref.TrimStr();
            dt.Rows.Add(dr);
            MtimeHelper.BulkToDB("FilmInfo", dt);
        }

        public override DataTable GetAsyncTable()
        {
            throw new NotImplementedException();
        }
    }
}
