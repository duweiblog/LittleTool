using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.SpiderService.Module
{
    public class PiaoFangFenTing
    {
        public string 省份 { get; set; }
        public string 所属院线 { get; set; }
        public string 影院编码 { get; set; }
        public string 影院名称 { get; set; }
        public string 场次编码 { get; set; }
        public string 影片编码 { get; set; }
        public string 影片名称 { get; set; }
        public string 发行版本 { get; set; }
        public string 影厅编码 { get; set; }
        public DateTime 放映日期 { get; set; } 
        public int 总人数 { get; set; }
        public decimal 总票房 { get; set; }
    }
}
