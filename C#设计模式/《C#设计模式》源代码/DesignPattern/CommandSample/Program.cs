using System;
using System.Configuration;
using System.Reflection;

namespace CommandSample
{
    /// <summary>
    ///  命令模式:1、命令  2、命令执行者 3、命令下发者
    ///  命令：定义命令执行的接口
    ///  命令执行者：实现命令接口，执行具体的命令
    ///  命令下发者：包含命令对象和下发命令接口
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            FunctionButton fb = new FunctionButton();//1、创建命令执行者

            Command command;
            //2、创建命令
            //读取配置文件
            string commandStr = ConfigurationManager.AppSettings["command"];
            //反射生成对象
            command = (Command)Assembly.Load("CommandSample").CreateInstance(commandStr);

            //3、下发命令给执行者
            //设置命令对象
            fb.Command = command;

            //4、执行命令
            fb.Click();
            CommandQueue queue = new CommandQueue();
            queue.AddCommand(command);
            Invoker leader = new Invoker(queue);
            leader.Call();


            Console.Read();
        }
    }
}
