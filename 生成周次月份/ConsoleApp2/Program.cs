using System;
using System.Collections.Generic;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Program pg = new Program();
            var weeks = pg.GetWeek(2019);

            foreach (var item in weeks)
            {
                Console.WriteLine(item.ToString());

            }
            var month = pg.GetMonth(2019);
            foreach (var item in month)
            {
                Console.WriteLine(item.ToString());

            } 
            Console.Read();
        }

        //生成某年自然周
        public List<DateModel> GetWeek(int year)
        {
            var dates = new List<DateModel>();
            var sdate = DateTime.Parse(year.ToString() + "-01" + "-01");
            var edate = DateTime.Parse(year.ToString() + "-12" + "-31");
            sdate = sdate.AddDays(-(int)sdate.DayOfWeek);

            var index = 0;
            var tdate = sdate;
            while (sdate.AddDays(7) <= edate)
            {
                var date = new DateModel();
                date.MonthWeek = ++index;
                date.sdate = sdate.AddDays(1);
                tdate = sdate.AddDays(7);
                date.edate = tdate;
                dates.Add(date);
                sdate = tdate;
            }
            return dates;
        }
        //生成某年自然月
        public List<DateModel> GetMonth(int year)
        {
            var dates = new List<DateModel>();
            var sdate = DateTime.Parse(year.ToString() + "-01" + "-01");
            var edate = DateTime.Parse(year.ToString() + "-12" + "-31");

            var index = 0;
            var tdate = sdate;
            while (sdate <= edate)
            {
                var date = new DateModel();
                date.MonthWeek = ++index;
                date.sdate = sdate;
                tdate = sdate.AddMonths(1).AddDays(-1);
                date.edate = tdate;
                dates.Add(date);
                sdate = tdate.AddDays(1);
            }
            return dates;
        }
    }
    public class DateModel
    {
        //周次月份
        public int MonthWeek { get; set; }
        //开始日期
        public DateTime sdate { get; set; }
        //开始日期
        public DateTime edate { get; set; }

        public override string ToString()
        {
            return MonthWeek.ToString() + ":" + sdate.ToString("yyyy-MM-dd") + "~" + edate.ToString("yyyy-MM-dd");
        }
    }
}
