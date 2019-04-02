using System;

namespace InterpreterSample
{
    /// <summary>
    /// 解释器设计模式，做各种解释
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //string instruction = "down run 10 and left move 20";
            string instruction = "up move 5 and down run 10 and left move 5";
            InstructionHandler handler = new InstructionHandler();
            handler.Handle(instruction);
            string outString;
            outString = handler.Output();
            Console.WriteLine(outString);

            Console.Read();
        }
    }
}
