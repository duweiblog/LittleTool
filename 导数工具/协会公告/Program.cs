using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HY.XieHuiImport
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionstring = ConfigurationOperator.GetValue("connectionstring");
            Console.OutputEncoding = Encoding.GetEncoding("gbk");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===================使用说明===================");
            Console.WriteLine("【协会公告】导入工具");
            Console.WriteLine("文件格式说明：");
            Console.WriteLine("文件名字从1开始，例如：1.txt，2.txt等。");
            Console.WriteLine("文件第一行：标题");
            Console.WriteLine("文件第二行：来源");
            Console.WriteLine("文件第三行：导语");
            Console.WriteLine("每段放到一行，不要换行");
            Console.WriteLine("图片命名规范：");
            Console.WriteLine("例如：1-1.jpg，1-2.jpg;2-1.jpg，2-2.jpg");
            Console.WriteLine("文件必须为utf-8编码，否则乱码！");
            Console.WriteLine("强制居中<center>XXXXXX</center>");
            Console.WriteLine("图片默认居中！");
            Console.WriteLine("当前数据库连接：" + connectionstring);
            Console.WriteLine("==============================================");
            Console.WriteLine("");
            Console.WriteLine("是否开始导入影片档期？【y/n】");
            Console.ForegroundColor = ConsoleColor.White;
            var key = Console.ReadLine().Trim().ToString();
            if (key.ToLower().Equals("y"))
            { 
                Console.WriteLine("确定文件为utf-8编码？");
                key = Console.ReadLine().Trim().ToString();
                if (key.ToLower().Equals("y"))
                {
                    ImportArticle import = new ImportArticle();
                    import.Begin();
                }
                else
                {
                    Console.WriteLine("输入任意键结束...");
                }
            }
            else
            {
                Console.WriteLine("输入任意键结束...");
            }

            Console.ReadKey();
        }
    }
}
