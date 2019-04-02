using System;

namespace CoRSample
{
    /// <summary>
    ///  责任链模式：
    ///     有多个对象，每个对象都有对下一个对象的引用，这样就形成一条链，
    ///     请求在这条链上传递，直到某一对象处理该请求。
    ///  用于：
    ///     在隐瞒客户的条件下，对系统动态调整
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Approver wjzhang, gyang, jguo, meeting;
            //Approver rhuang;
            //rhuang = new Manager("黄蓉");

            wjzhang = new Director("张无忌");
            gyang = new VicePresident("杨过");
            jguo = new President("郭靖");
            meeting = new Congress("董事会");

            //创建职责链
            wjzhang.SetSuccessor(gyang);
            gyang.SetSuccessor(jguo);
            jguo.SetSuccessor(meeting);

            //wjzhang.SetSuccessor(rhuang); //将“黄蓉”作为“张无忌”的下家
            //rhuang.SetSuccessor(gyang); //将“杨过”作为“黄蓉”的下家
            //gyang.SetSuccessor(jguo);
            //jguo.SetSuccessor(meeting);


            //创建采购单
            PurchaseRequest pr1 = new PurchaseRequest(45000, 10001, "购买倚天剑");
            wjzhang.ProcessRequest(pr1);

            PurchaseRequest pr2 = new PurchaseRequest(60000, 10002, "购买《葵花宝典》");
            wjzhang.ProcessRequest(pr2);

            PurchaseRequest pr3 = new PurchaseRequest(160000, 10003, "购买《金刚经》");
            wjzhang.ProcessRequest(pr3);

            PurchaseRequest pr4 = new PurchaseRequest(800000, 10004, "购买桃花岛");
            wjzhang.ProcessRequest(pr4);

            Console.Read();
        }
    }
}
