using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.SpiderService.Module
{
    public class ActorDetail : IBaseModule
    {
        public string DetailID { get; set; }
        public string ActorID { get; set; }
        public string ActorName { get; set; }
        public string ActorOtherName { get; set; }
        public string ActorHref { get; set; }
        public string ActorType { get; set; }
        public DateTime ActorBirth { get; set; }
        public string ActorPic { get; set; }
        public int FilmID { get; set; }
    }
}
