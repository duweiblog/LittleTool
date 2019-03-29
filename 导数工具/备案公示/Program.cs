using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using HY.SpliderService;

namespace HY.SpliderService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("输入【y/n】是否开始导入备案公示：");
            var key = Console.ReadLine().Trim().ToString();
            if (key.ToLower().Equals("y"))
            {
                ImportNewFilm pg = new ImportNewFilm();
                pg.Begin();
            }
            Console.WriteLine("输入任意键结束...");
            Console.ReadKey();
        }

        //static void Main1(string[] args)
        //{
        //    Console.WriteLine("输入是否开始获取影片详情【y/n】");
        //    var key = Console.ReadLine().Trim().ToString();
        //    if (key.ToLower().Equals("y"))
        //    {

        //        Console.WriteLine("输入1或者2：\n 1、1开始获取影片\n 2、2获取影片详情");
        //        key = Console.ReadLine().Trim().ToString();
        //        MtimeSplider mtime = new MtimeSplider();
        //        if (key == "1")
        //        { 
        //            mtime.Begin();
        //        }
        //        else
        //        {
        //            mtime.BeginDetail();
        //        }
        //    }
        //    Console.WriteLine("输入任意键结束.....");
        //    Console.ReadKey();
        //}

        //static void Main3(string[] args)
        //{
        //    Console.WriteLine("输入是否开始获取影片详情【y/n】");
        //    var key = Console.ReadLine().Trim().ToString();
        //    if (key.ToLower().Equals("y"))
        //    {
        //        MtimeSplider mtime = new MtimeSplider();
        //        mtime.BeginDetail();
        //    }
        //    Console.WriteLine("输入任意键结束...");
        //    Console.ReadKey();

        //}

    }
}
