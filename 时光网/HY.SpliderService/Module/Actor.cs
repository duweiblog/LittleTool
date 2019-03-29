using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.SpiderService.Module
{

    public class Actor
    {

        public string FilmName { get; set; }
        public string ActorName { get; set; }
        public string ActorHref { get; set; }
        public string ActorType { get; set; }
        public int FilmID { get; set; }
    }

    public class 演职员表
    {
        public string 自动编号 { get; set; }
        public string 姓名 { get; set; }
        public string 性别 { get; set; }
        public string 导演 { get; set; }
        public string 演员 { get; set; }
        public string 制片 { get; set; }
        public string 编剧 { get; set; }
        public string 烟火 { get; set; }
        public string 武打 { get; set; }
        public string 出生日期 { get; set; }
        public string 毕业日期 { get; set; }
        public string 毕业院校 { get; set; }
        public string 个人简介 { get; set; }
    }

    public class 影片关系表
    {
        public string 自动编号 { get; set; }
        public string 影片编号 { get; set; }
        public string 影片名称 { get; set; }
        public string 关系 { get; set; }
        public string 关系编号 { get; set; }
        public string 关系名称 { get; set; }

    }
}
