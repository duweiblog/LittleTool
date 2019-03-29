using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using static HY.SpliderService.ChinaSarftSplider;

namespace HY.SpliderService
{
    /// <summary>
    /// 备案立项导入
    /// </summary>
    public class ImportNewFilm
    {
        private XmlHelper xmlHelper = new XmlHelper();
        private Thread catchThread;
        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        private ChinaSarftSplider splider = new ChinaSarftSplider();
        private string Page_URL = "http://dy.chinasarft.gov.cn/";
        public void Environment()
        {
            var directory = System.AppDomain.CurrentDomain.BaseDirectory + "SpliderLog";
            var file = directory + "\\splider.xml";
            FileHelper.CreateDirectory(directory);
        }
        public void Begin()
        {
            try
            {
                Environment();
                RunCatch();
                //if (catchThread == null)
                //    catchThread = new Thread(RunCatch);

                //if (catchThread.ThreadState != ThreadState.Running)
                //    catchThread.Start(); //线程一开始运作 
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);//记录日志文件
            }
        }

        public void RunCatch()
        {
            int ConfigurationOperator = int.Parse(ConfigurationManager.AppSettings.Get("CatchingDatePer"));
            //while (true)
            //{
            try
            {
                SetTip("服务已启动！");
                splider.Domain = Page_URL;
                var page = splider.DownloadPage(Page_URL);
                List<Article> articles = splider.AnalysisHomePage(page);
                if (articles.Count > 0)
                {
                    for (int i = articles.Count - 1; i >= 0; i--)
                    {
                        if (xmlHelper.IsCatch(articles[i].ArticleID))
                            articles.Remove(articles[i]);
                    }
                }
                if (articles.Count > 0)
                {
                    var notices = splider.AnalysisPage(articles);
                    InsertDB(notices);
                    articles.ForEach(x =>
                    {
                        xmlHelper.InsertLog(x.ArticleID, DateTime.Now);
                        SetTip("新增文章ID：" + x.ArticleID);
                    });
                    SetTip("本次新增" + notices.Count + "部影片！");
                }
                else
                {
                    SetTip("没有新增文章！");
                }
            }
            catch (ThreadAbortException ex)
            {
                LogHelper.WriteLog(ex.Message);//记录日志文件
                catchThread.Interrupt();
                //break;
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(e.Message);//记录日志文件
                catchThread.Interrupt();
                //break;
            }

            //    SetTip("下次启动时间：" + DateTime.Now.AddMinutes(ConfigurationOperator));
            //    Thread.Sleep(ConfigurationOperator * 60 * 1000);
            //}
        }
        private void InsertDB(List<Notice> notices)
        {
            var dt = DbHelperSQL.Query("select * from KS_U_xpdq where 1!=1").Tables[0];
            var dateNow = DateTime.Now;
            foreach (var item in notices)
            {
                DataRow dr = dt.NewRow();
                dr["TID"] = "20149988779582";
                dr["Title"] = item.FilmName;
                dr["FullTitle"] = item.NoticeNo;
                dr["Intro"] = item.FilmName;
                dr["KS_Type"] = item.FilmKind;
                dr["KS_Writers"] = item.ScreenWriter;
                dr["ArticleContent"] = item.Result;
                dr["KS_Province"] = item.Area;
                dr["KS_Xunci"] = item.Xunci;
                dr["KS_ShowDate"] = item.ShowDate;
                dr["Author"] = item.Author;
                dr["Origin"] = item.Author;

                #region 为空字段  或者默认 
                dr["KS_regdate"] = dateNow;//TODO
                dr["ShowComment"] = 0;
                dr["KS_Producers"] = "";
                dr["KS_TheIssuer"] = "";
                dr["KS_Starring"] = "";
                dr["KS_Director"] = "";
                dr["KS_EndDate"] = DBNull.Value;
                dr["KS_Result"] = "";
                dr["TitleType"] = "";
                dr["KS_State"] = "";
                dr["KS_Standard"] = "";
                dr["KS_TimeLength"] = "";
                dr["PageTitle"] = "";
                dr["Hits"] = 0;
                dr["HitsByDay"] = 0;
                dr["HitsByWeek"] = 0;
                dr["HitsByMonth"] = 0;
                dr["LastHitsTime"] = DBNull.Value;
                dr["Rank"] = "★";
                dr["AddDate"] = dateNow;
                dr["ModifyDate"] = dateNow;
                dr["JSID"] = "";
                dr["TemplateID"] = "{@TemplateDir}/新片档期/栏目页.html";
                dr["WapTemplateID"] = "{@TemplateDir}/3G/xpdq/show.html";
                dr["Fname"] = "";
                dr["RefreshTF"] = 1;
                dr["Inputer"] = "admin";
                dr["PhotoUrl"] = "";
                dr["PicNews"] = 1;
                dr["Changes"] = 0;
                dr["Recommend"] = 0;
                dr["Rolls"] = 0;
                dr["Strip"] = 0;
                dr["Popular"] = 0;
                dr["Verific"] = 1;
                dr["Slide"] = 0;
                dr["Comment"] = 0;
                dr["IsTop"] = 0;
                dr["IsVideo"] = 0;
                dr["DelTF"] = 0;
                dr["PostID"] = 0;
                dr["PostTable"] = "KS_Comment";
                dr["CmtNum"] = 0;
                dr["IsSign"] = 0;
                dr["SignUser"] = "";
                dr["SignDateLimit"] = 0;
                dr["SignDateEnd"] = dateNow;
                dr["Province"] = "";
                dr["City"] = "";
                dr["MapMarker"] = "";
                dr["InfoPurview"] = 0;
                dr["ArrGroupID"] = "";
                dr["ReadPoint"] = 0;
                dr["ChargeType"] = 0;
                dr["PitchTime"] = 24;
                dr["ReadTimes"] = 10;
                dr["DividePercent"] = 0;
                dr["SEOTitle"] = "";
                dr["SEOKeyWord"] = "";
                dr["SEODescript"] = "";
                dr["RelatedID"] = 0;
                dr["TitleFontColor"] = "";
                dr["TitleFontType"] = "";
                dr["OTID"] = "";
                dr["OId"] = 0;
                dr["OrderId"] = 0;
                dr["AvgScore"] = 0;
                dr["KeyWords"] = "";
                #endregion

                dt.Rows.Add(dr);
            }
            DbHelperSQL.BulkToDB("KS_U_xpdq", dt);
        }

        public void SetTip(string msg)
        {
            var log = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + msg;
            LogHelper.WriteLog(msg);//记录日志文件
        }
    }
}
