using HY.XieHuiImport;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Linq;

namespace HY.XieHuiImport
{
    public class ImportArticle
    {
        private Thread catchThread;
        private string DirectoryPath = System.AppDomain.CurrentDomain.BaseDirectory + "File";
        private string PicturePath = System.AppDomain.CurrentDomain.BaseDirectory + "File\\picture";
        public void Begin()
        {
            try
            {

                Environment();
                if (catchThread == null)
                    catchThread = new Thread(RunCatch);

                if (catchThread.ThreadState != ThreadState.Running)
                    catchThread.Start(); //线程一开始运作 
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);//记录日志文件
            }
        }

        public void RunCatch()
        {
            try
            {
                LogHelper.WriteLog("开始解析文件！");
                var picPath = DirectoryPath + "\\picture";
                //var picName = "";
                var files = Directory.GetFiles(DirectoryPath);
                var fileList = new List<string>();
                var pictureList = new List<string>();
                var pictureNameList = new List<string>();
                foreach (var file in files)
                {
                    if (Path.GetExtension(file).Equals(".txt"))
                    {
                        fileList.Add(file);
                    }
                    else
                    {
                        pictureList.Add(file);
                        pictureNameList.Add(Path.GetFileNameWithoutExtension(file));
                    }
                }
                LogHelper.WriteLog(string.Format("本次发现{0}个文件，{1}个图片", fileList.Count, pictureList.Count));
                LogHelper.WriteLog("文件排序！");
                //文件排序
                fileList = fileList.OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x))).ToList();

                for (int i = 0; i < fileList.Count; i++)
                {
                    //图片索引,第一张图片为封面
                    var picIndex = 1;
                    var fileIndex = Path.GetFileNameWithoutExtension(fileList[i]);
                    //picName = fileIndex + "-" + picIndex;
                    var noticeList = new List<Notice>();

                    LogHelper.WriteLog(string.Format("正在解析文章：{0}", Path.GetFileName(fileList[i])));
                    using (StreamReader sr = new StreamReader(new FileStream(fileList[i], FileMode.Open)))
                    {
                        var notice = new Notice();
                        notice.Title = sr.ReadLine().Trim();//标题
                        notice.Author = sr.ReadLine().Trim();//来源
                        notice.Intro = sr.ReadLine().Trim();//导语
                        notice.ArticleContent = "";
                        var strLine = sr.ReadLine().Trim();

                        while (strLine != null)
                        {
                            strLine = strLine.Trim();
                            if (string.IsNullOrEmpty(strLine))
                            {
                                notice.ArticleContent += "<p></p>";
                            }
                            else
                            {
                                //if (picName == strLine)//图片名称必须和文件名对应。
                                //图片段落
                                if (pictureNameList.IndexOf(strLine) >= 0)//图片名字和文件名没有关系，图片存在即可。
                                {
                                    var picExist = pictureList.Find(x => Path.GetFileName(x).IndexOf(strLine) >= 0);
                                    if (string.IsNullOrEmpty(picExist))
                                        throw new Exception(string.Format("没有找到{0}对应图片", strLine));
                                    else
                                    {
                                        var newPicName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(picExist);
                                        var newPath = Path.Combine(picPath, newPicName);
                                        var currMonth = DateTime.Now.Month;
                                        var contentPic = "/UploadFiles/" + DateTime.Now.Year + "-" + (currMonth < 10 ? "0" + currMonth.ToString() : currMonth.ToString()) + "/2/" + newPicName;
                                        File.Copy(picExist, newPath);
                                        if (picIndex == 1)
                                        {
                                            notice.PhotoUrl = contentPic;
                                        }
                                        notice.ArticleContent += "<center><img title=\"" + strLine + "\" src=\"" + contentPic + "\"/></center>";
                                        picIndex++;
                                        //picName = fileIndex + "-" + picIndex;
                                    }
                                }
                                else
                                {
                                    //判断是否为 center 标签
                                    if (strLine.IndexOf("<center>") == 0)
                                    {
                                        //居中段落
                                        notice.ArticleContent += strLine;
                                    }
                                    else
                                    {
                                        //普通段落
                                        notice.ArticleContent += "<p>" + strLine + "</p>";
                                    }
                                }
                            }
                            strLine = sr.ReadLine();
                        }
                        noticeList.Add(notice);
                    }
                    LogHelper.WriteLog(string.Format("正在将{0}个文章，插入到数据库。", noticeList.Count));
                    InsertDB(noticeList);
                    LogHelper.WriteLog(string.Format("正在将{0}个文章，插入到数据库，已完成。", noticeList.Count));
                }
                LogHelper.WriteLog("本次服务已完成。");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
            }
        }
        private void InsertDB(List<Notice> notices)
        {
            var dt = DbHelperSQL.Query("select * from KS_U_xpdq where 1!=1").Tables[0];
            var dateNow = DateTime.Now;

            var orderID = int.Parse(DbHelperSQL.GetSingleVal("SELECT MAX(OrderId) OrderId FROM dbo.KS_U_xpdq  WHERE TID='20147202338476'").ToString());
            var index = 0;
            foreach (var item in notices)
            {
                ++index;
                DataRow dr = dt.NewRow();
                dr["TID"] = "20147202338476";//行业动态
                dr["Title"] = item.Title;
                dr["Intro"] = item.Intro;
                dr["ArticleContent"] = item.ArticleContent;
                dr["Author"] = item.Author;
                dr["Origin"] = item.Author;
                dr["PhotoUrl"] = item.PhotoUrl;

                #region 为空字段  或者默认
                dr["KS_ShowDate"] = DBNull.Value;
                dr["KS_Province"] = "中国大陆";
                dr["KS_Type"] = "故事影片";
                dr["KS_Writers"] = "";
                dr["KS_Xunci"] = "";
                dr["FullTitle"] = "";
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
                dr["Rank"] = "★★★";
                dr["AddDate"] = dateNow;
                dr["ModifyDate"] = dateNow;
                dr["JSID"] = "";
                dr["TemplateID"] = "{@TemplateDir}/新片档期/内容页.html";
                dr["WapTemplateID"] = "{@TemplateDir}/3G/xpdq/show.html";
                dr["Fname"] = "26194.html";
                dr["RefreshTF"] = 1;
                dr["Inputer"] = "admin";
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
                dr["OrderId"] = orderID + index;
                dr["AvgScore"] = 0;
                dr["KeyWords"] = "";
                #endregion

                dt.Rows.Add(dr);
            }
            DbHelperSQL.BulkToDB("KS_U_xpdq", dt);
        }


        public void Environment()
        {
            FileHelper.CreateDirectory(DirectoryPath);
            FileHelper.CreateDirectory(PicturePath);
            FileHelper.DelFile(PicturePath);
        }
    }
    public class Notice
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 导语
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string ArticleContent { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Author { get; set; }

        public string PhotoUrl { get; set; }//取第一张图片URL 

    }
}
