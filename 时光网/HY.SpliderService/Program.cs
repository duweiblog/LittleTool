using System;
using System.Threading;
using HY.SpiderService.Spider;
using System.Collections;

namespace HY.SpiderService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "数据抓取工具";
            Console.WriteLine("支持数据：");
            Console.WriteLine("1、影片信息");
            Console.WriteLine("2、影片详细信息(图片D：\\时光网\\)");
            Console.WriteLine("3、演员");
            Console.WriteLine("4、演员详细信息(图片D：\\时光网\\)");
            Console.WriteLine("5、专资日票房");
            Console.WriteLine("6、发现地区表数据");
            Console.WriteLine("7、下载地区表数据");
            Console.WriteLine("8、下载机构表数据");
            Console.WriteLine("9、下载分厅分场票房数据");
            Console.WriteLine("输入序号【1-9】");
            var key = Console.ReadLine().Trim().ToString();
            switch (key)
            {
                case "1":
                    {
                        Console.Title = "影片信息";
                        Console.WriteLine("开始获取影片【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            while (true)
                            {
                                try
                                {
                                    new FilmSpider().Begin();
                                    Thread.Sleep(60000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        break;
                    }
                case "2":
                    {
                        Console.Title = "影片详细信息";
                        Console.WriteLine("开始获取影片详细【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            while (true)
                            {
                                try
                                {
                                    new FilmDetailSpider().Begin();
                                    Thread.Sleep(60000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        break;
                    }
                case "3":
                    {
                        Console.Title = "演员";
                        Console.WriteLine("开始获取演员【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            while (true)
                            {
                                try
                                {
                                    new ActorSpider().Begin();
                                    Thread.Sleep(60000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        break;
                    }
                case "4":
                    {
                        Console.Title = "演员详情";
                        Console.WriteLine("开始获取演员详情【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            while (true)
                            {
                                try
                                {
                                    new ActorDetailSpider().Begin();
                                    Thread.Sleep(60000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        break;
                    }
                case "5":
                    {
                        Console.Title = "专资日票房";
                        Console.WriteLine("专资日票房【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {

                            while (true)
                            {
                                try
                                {
                                    new ZhuanZiSpider().Begin();
                                    Thread.Sleep(60000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

                        }
                        break;
                    }
                case "6":
                    {
                        Console.Title = "发现地区表数据";
                        Console.WriteLine("地区表数据【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {

                            while (true)
                            {
                                try
                                {
                                    new TCmapSpider().Begin();
                                    Thread.Sleep(60000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

                        }
                        break;
                    }
                case "7":
                    {
                        Console.Title = "下载地区表数据";
                        Console.WriteLine("下载地区表数据【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            while (true)
                            {
                                try
                                {
                                    new TCmapDetailSpider().Begin();
                                    Thread.Sleep(60000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

                        }
                        break;
                    }
                case "8":
                    {
                        Console.Title = "下载机构表数据";
                        Console.WriteLine("下载机构表数据【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            while (true)
                            {
                                try
                                {
                                    new CompanySpider().Begin();
                                    Thread.Sleep(1000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

                        }
                        break;
                    }
                case "9":
                    {
                        Console.Title = "下载分厅分场票房数据";
                        Console.WriteLine("下载分厅分场票房数据【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            while (true)
                            {
                                try
                                {
                                    new PiaoFangTingSpider().Begin();
                                    Thread.Sleep(1000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

                        }
                        break;
                    }
                case "10":
                    {
                        Console.Title = "下载专资影院数据";
                        Console.WriteLine("下载专资影院数据【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            while (true)
                            {
                                try
                                {
                                    new YingYuanSpider().Begin();
                                    Thread.Sleep(1000);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

                        }
                        break;
                    }
                case "11":
                    {
                        Console.Title = "下载国家统计局区划代码";
                        Console.WriteLine("下载国家统计局区划代码【y/n】");
                        var isBegin = Console.ReadLine().Trim().ToString();
                        if (isBegin.ToLower().Trim().Equals("y"))
                        {
                            try
                            {
                                new TongJiJuAreaSpider().Begin();
                                //http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2017/11/01/14/110114012.html
                                //new TongJiJuAreaSpider().GetVillageArea("http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2017/11/01/14/110114012.html", "", "", "", "");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
            Console.WriteLine("输入任意键结束.....");
            Console.ReadKey();
        }
    }
}
