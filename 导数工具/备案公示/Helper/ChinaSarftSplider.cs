using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Xml;

namespace HY.SpliderService
{
    public class ChinaSarftSplider
    {
        public static int BeginLine = 5;
        public HttpResult HttpResult { get; set; }
        public static HttpHelper http = new HttpHelper();
        #region 插入脚本
        public string InsertSql = @"INSERT  INTO [KS_U_xpdq]
                                                ( [TID] ,
                                                  [Title] ,
                                                  [FullTitle] ,
                                                  [Intro] ,
                                                  [KS_Type] ,
                                                  [KS_Writers] ,
                                                  [ShowComment] ,
                                                  [ArticleContent] ,
                                                  [KS_Producers] ,
                                                  [KS_TheIssuer] ,
                                                  [KS_Province] ,
                                                  [KS_Xunci] ,
                                                  [KS_ShowDate] ,
                                                  [KS_Starring] ,
                                                  [KS_Director] ,
                                                  [KS_EndDate] ,
                                                  [KS_Result] ,
                                                  [KS_regdate] ,
                                                  [KS_State] ,
                                                  [KS_Standard] ,
                                                  [KS_TimeLength] ,
                                                  [PageTitle] ,
                                                  [Author] ,
                                                  [Origin] ,
                                                  [TitleType] ,
                                                  [Rank] ,
                                                  [Hits] ,
                                                  [HitsByDay] ,
                                                  [HitsByWeek] ,
                                                  [HitsByMonth] ,
                                                  [LastHitsTime] ,
                                                  [AddDate] ,
                                                  [ModifyDate] ,
                                                  [JSID] ,
                                                  [TemplateID] ,
                                                  [WapTemplateID] ,
                                                  [Fname] ,
                                                  [RefreshTF] ,
                                                  [Inputer] ,
                                                  [PhotoUrl] ,
                                                  [PicNews] ,
                                                  [Changes] ,
                                                  [Recommend] ,
                                                  [Rolls] ,
                                                  [Strip] ,
                                                  [Popular] ,
                                                  [Verific] ,
                                                  [Slide] ,
                                                  [Comment] ,
                                                  [IsTop] ,
                                                  [IsVideo] ,
                                                  [DelTF] ,
                                                  [PostID] ,
                                                  [PostTable] ,
                                                  [CmtNum] ,
                                                  [IsSign] ,
                                                  [SignUser] ,
                                                  [SignDateLimit] ,
                                                  [SignDateEnd] ,
                                                  [Province] ,
                                                  [City] ,
                                                  [MapMarker] ,
                                                  [InfoPurview] ,
                                                  [ArrGroupID] ,
                                                  [ReadPoint] ,
                                                  [ChargeType] ,
                                                  [PitchTime] ,
                                                  [ReadTimes] ,
                                                  [DividePercent] ,
                                                  [SEOTitle] ,
                                                  [SEOKeyWord] ,
                                                  [SEODescript] ,
                                                  [RelatedID] ,
                                                  [TitleFontColor] ,
                                                  [TitleFontType] ,
                                                  [OTID] ,
                                                  [OId] ,
                                                  [OrderId] ,
                                                  [AvgScore] ,
                                                  [KeyWords]
                                                )
                                        VALUES  ( @TID ,
                                                  @Title ,
                                                  @FullTitle ,
                                                  @Intro ,
                                                  @KS_Type ,
                                                  @KS_Writers ,
                                                  @ShowComment ,
                                                  @ArticleContent ,
                                                  @KS_Producers ,
                                                  @KS_TheIssuer ,
                                                  @KS_Province ,
                                                  @KS_Xunci ,
                                                  @KS_ShowDate ,
                                                  @KS_Starring ,
                                                  @KS_Director ,
                                                  @KS_EndDate ,
                                                  @KS_Result ,
                                                  @KS_regdate ,
                                                  @KS_State ,
                                                  @KS_Standard ,
                                                  @KS_TimeLength ,
                                                  @PageTitle ,
                                                  @Author ,
                                                  @Origin ,
                                                  @TitleType ,
                                                  @Rank ,
                                                  @Hits ,
                                                  @HitsByDay ,
                                                  @HitsByWeek ,
                                                  @HitsByMonth ,
                                                  @LastHitsTime ,
                                                  @AddDate ,
                                                  @ModifyDate ,
                                                  @JSID ,
                                                  @TemplateID ,
                                                  @WapTemplateID ,
                                                  @Fname ,
                                                  @RefreshTF ,
                                                  @Inputer ,
                                                  @PhotoUrl ,
                                                  @PicNews ,
                                                  @Changes ,
                                                  @Recommend ,
                                                  @Rolls ,
                                                  @Strip ,
                                                  @Popular ,
                                                  @Verific ,
                                                  @Slide ,
                                                  @Comment ,
                                                  @IsTop ,
                                                  @IsVideo ,
                                                  @DelTF ,
                                                  @PostID ,
                                                  @PostTable ,
                                                  @CmtNum ,
                                                  @IsSign ,
                                                  @SignUser ,
                                                  @SignDateLimit ,
                                                  @SignDateEnd ,
                                                  @Province ,
                                                  @City ,
                                                  @MapMarker ,
                                                  @InfoPurview ,
                                                  @ArrGroupID ,
                                                  @ReadPoint ,
                                                  @ChargeType ,
                                                  @PitchTime ,
                                                  @ReadTimes ,
                                                  @DividePercent ,
                                                  @SEOTitle ,
                                                  @SEOKeyWord ,
                                                  @SEODescript ,
                                                  @RelatedID ,
                                                  @TitleFontColor ,
                                                  @TitleFontType ,
                                                  @OTID ,
                                                  @OId ,
                                                  @OrderId ,
                                                  @AvgScore ,
                                                  @KeyWords
                                                )";
        #endregion
        public string Domain = "";

        /// <summary>
        /// 下载页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string DownloadPage(string url)
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
            System.Threading.Thread.Sleep(300);
            HttpResult = http.GetHtml(item);
            return HttpResult.Html;
        }

        public List<Article> AnalysisHomePage(string page)
        {
            var ArticleList = new List<Article>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(page);            //*[@id="main"]/div[5]/div[2]/div[2]/div[2]/div[2]/ul
            HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectNodes("//*[@id='main']/div[5]/div[2]/div[2]/div[2]/div[2]/ul/li");

            if (collection.Count < 2)
            {
                BeginLine = collection.Count;
            }

            for (int i = 0; i < BeginLine; i++)
            {
                var node = collection[i].SelectSingleNode("a");
                var article = new Article();
                article.ArticleTitle = node.InnerText;
                article.ArticleHref = node.GetAttributeValue("href", "");
                var beginIndex = article.ArticleHref.IndexOf("?id=") + 4;
                var endIndex = article.ArticleHref.IndexOf("&templateId=");
                var subLength = endIndex - beginIndex;
                article.ArticleID = article.ArticleHref.Substring(beginIndex, subLength);
                ArticleList.Add(article);
            }

            return ArticleList;
        }
        /// <summary>
        /// 解析页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Notice> AnalysisPage2(List<Article> ArticleList)
        {
            var NoticeList = new List<Notice>();
            foreach (var item in ArticleList)
            {
                var loadPage = DownloadPage(Domain + item.ArticleHref);
                //*[@id="divf1"]/table/tbody/tr[1]
                HtmlDocument tempDocument = new HtmlDocument();
                tempDocument.LoadHtml(loadPage);


                HtmlNode pubDate = tempDocument.DocumentNode.SelectSingleNode("//*[@id='main']/div[2]/div[2]/div/div/div[2]/div[3]/p[2]");
                var pubDateStr = pubDate.InnerText.Trim();
                pubDateStr = pubDateStr.Substring(0, pubDateStr.IndexOf(","));
                var pubDateArr = pubDateStr.Split(new[] { "-" }, StringSplitOptions.None);
                var sdate = DateTime.Parse(pubDateArr[0]);
                var edate = DateTime.Parse(sdate.Year + "年" + pubDateArr[1]);
                var xunci = GetXunCi(sdate, edate);


                HtmlNodeCollection filmKindColection = tempDocument.DocumentNode.SelectNodes("//*[@id='main']/div[2]/div[2]/div/div/div[2]/div[4]/div[4]/ul/li");
                var filmKindList = new List<FilmKind>();
                foreach (var filmKind in filmKindColection)
                {
                    filmKindList.Add(SetFilmKind(filmKind));
                }

                foreach (var filmKind in filmKindList)
                {
                    HtmlNodeCollection tablecollection = tempDocument.DocumentNode.SelectNodes("//*[@id='" + filmKind.KindID + "']/table/tr");
                    for (int i = 1; i < tablecollection.Count; i++)//第一行为标题，忽略
                    {
                        NoticeList.Add(AnalysisNotice(tablecollection[i], filmKind.KindName, xunci, edate));
                    }
                }
            }
            return NoticeList;
        }

        /// <summary>
        /// 解析页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Notice> AnalysisPage3(string loadPage)
        {
            var NoticeList = new List<Notice>();
            //*[@id="divf1"]/table/tbody/tr[1]
            HtmlDocument tempDocument = new HtmlDocument();
            tempDocument.LoadHtml(DownloadPage(loadPage));


            HtmlNode pubDate = tempDocument.DocumentNode.SelectSingleNode("//*[@id='main']/div[2]/div[2]/div/div/div[2]/div[3]/p[2]");
            var pubDateStr = pubDate.InnerText.Trim();
            var endIndex = pubDateStr.IndexOf("-") + 7;

            pubDateStr = pubDateStr.Substring(0, endIndex);
            var pubDateArr = pubDateStr.Split(new[] { "-" }, StringSplitOptions.None);
            var sdate = DateTime.Parse(pubDateArr[0]);
            var edate = DateTime.Parse(sdate.Year + "年" + pubDateArr[1]);
            var xunci = GetXunCi(sdate, edate);


            HtmlNodeCollection filmKindColection = tempDocument.DocumentNode.SelectNodes("//*[@id='main']/div[2]/div[2]/div/div/div[2]/div[4]/div[4]/ul/li");
            var filmKindList = new List<FilmKind>();
            foreach (var filmKind in filmKindColection)
            {
                filmKindList.Add(SetFilmKind(filmKind));
            }

            foreach (var filmKind in filmKindList)
            {
                HtmlNodeCollection tablecollection = tempDocument.DocumentNode.SelectNodes("//*[@id='" + filmKind.KindID + "']/table/tr");
                for (int i = 1; i < tablecollection.Count; i++)//第一行为标题，忽略
                {
                    NoticeList.Add(AnalysisNotice(tablecollection[i], filmKind.KindName, xunci, edate));
                }
            }
            return NoticeList;
        }

        /// <summary>
        /// 解析页面
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Notice> AnalysisPage(List<Article> ArticleList)
        {
            var NoticeList = new List<Notice>();
            foreach (var item in ArticleList)
            {
                var loadPage = DownloadPage(Domain + item.ArticleHref);
                //*[@id="divf1"]/table/tbody/tr[1]
                HtmlDocument tempDocument = new HtmlDocument();
                tempDocument.LoadHtml(loadPage);


                HtmlNode pubDate = tempDocument.DocumentNode.SelectSingleNode("//*[@id='main']/div[2]/div[2]/div/div/div[2]/div[3]/p[2]");
                var pubDateStr = pubDate.InnerText.Trim();
                pubDateStr = pubDateStr.Substring(0, pubDateStr.IndexOf(","));
                var pubDateArr = pubDateStr.Split(new[] { "-" }, StringSplitOptions.None);
                var sdate = DateTime.Parse(pubDateArr[0]);
                var edate = DateTime.Parse(sdate.Year + "年" + pubDateArr[1]);
                var xunci = GetXunCi(sdate, edate);


                HtmlNodeCollection filmKindColection = tempDocument.DocumentNode.SelectNodes("//*[@id='main']/div[2]/div[2]/div/div/div[2]/div[4]/div[4]/ul/li");
                var filmKindList = new List<FilmKind>();
                foreach (var filmKind in filmKindColection)
                {
                    filmKindList.Add(SetFilmKind(filmKind));
                }

                foreach (var filmKind in filmKindList)
                {
                    HtmlNodeCollection tablecollection = tempDocument.DocumentNode.SelectNodes("//*[@id='" + filmKind.KindID + "']/table/tr");
                    for (int i = 1; i < tablecollection.Count; i++)//第一行为标题，忽略
                    {
                        NoticeList.Add(AnalysisNotice(tablecollection[i], filmKind.KindName, xunci, edate));
                    }
                }
            }
            return NoticeList;
        }

        /// <summary>
        /// 获取旬次
        /// </summary>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <returns></returns>
        public string GetXunCi(DateTime sdate, DateTime edate)
        {
            var sDay = sdate.Day;
            var eDay = edate.Day;
            var isSx = false;
            var isZx = false;
            var isXx = false;
            var isSZx = false;
            var isZXx = false;
            var isSZXx = false;
            isSx = sDay >= 1 && eDay <= 10;//上旬
            isZx = sDay >= 11 && eDay <= 20;//中旬
            isXx = sDay >= 21 && eDay > 20;//下旬
            isSZx = sDay >= 1 && eDay <= 20;//上中旬
            isZXx = sDay >= 11 && eDay > 20;//中下旬
            isSZXx = sDay >= 1 && eDay > 20;//上中下旬

            if (isSx)
                return "(上旬)";
            if (isZx)
                return "(中旬)";
            if (isXx)
                return "(下旬)";
            if (isSZx)
                return "(上旬、中旬)";
            if (isZXx)
                return "(中旬、下旬)";
            if (isSZXx)
                return "(上旬、中旬、下旬)";
            return "";
        }

        public FilmKind SetFilmKind(HtmlNode filmKind)
        {
            var item = new FilmKind();
            var divID = filmKind.Id;
            item.KindID = divID.Replace("li", "div");
            item.KindName = filmKind.InnerText;
            return item;
        }

        public Notice AnalysisNotice(HtmlNode notice, string filmKind, string xunCi, DateTime showDate)
        {
            var index = 1;
            var tds = notice.SelectNodes("td");
            Notice newNotice = new Notice();
            if (tds != null && tds.Count > 0)
            {
                newNotice.NoticeNo = tds[index++].InnerText;
                newNotice.FilmName = tds[index++].InnerText;
                newNotice.Author = tds[index++].InnerText.Replace("、", "<br>");
                newNotice.ScreenWriter = tds[index++].InnerHtml;
                newNotice.Result = tds[index++].InnerText;
                newNotice.Area = tds[index++].InnerText;
                newNotice.FilmKind = filmKind;
                newNotice.Xunci = xunCi;
                newNotice.ShowDate = showDate;
                return newNotice;
            }
            else
            {
                return null;
            }
        }

        public class Article
        {
            public string ArticleHref { get; set; }
            public string ArticleTitle { get; set; }

            public string ArticleID { get; set; }
        }
        public class Notice
        {
            public string NoticeNo { get; set; }
            public string FilmName { get; set; }
            public string Author { get; set; }
            public string ScreenWriter { get; set; }
            public string Result { get; set; }
            public string Area { get; set; }
            public string FilmKind { get; set; }

            public DateTime ShowDate { get; set; }

            public string Xunci { get; set; }

        }

        public class FilmKind
        {
            public string KindID { get; set; }
            public string KindName { get; set; }
        }

    }
    public class XmlLog
    {
        public string ArticleID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
