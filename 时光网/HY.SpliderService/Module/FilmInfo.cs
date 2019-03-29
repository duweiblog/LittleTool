using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.SpiderService.Module
{

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
        public DateTime ShangYing { get; set; }
        public string ZhiShi { get; set; }

        public string FilmHref { get; set; }
    }
}
