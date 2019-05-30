using System;

namespace MediatorSample
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //定义中介者对象
		    ConcreteMediator mediator;
		    mediator = new ConcreteMediator();
		
            //定义同事对象
		    Button addBT = new Button();
		    List list = new List();
	        ComboBox cb = new ComboBox();
	        TextBox userNameTB = new TextBox();

		    addBT.SetMediator(mediator);
		    list.SetMediator(mediator);
		    cb.SetMediator(mediator);
		    userNameTB.SetMediator(mediator);

		    mediator.addButton = addBT;
		    mediator.list = list;
		    mediator.cb = cb;
		    mediator.userNameTextBox = userNameTB;
		
		    addBT.Changed();
		    Console.WriteLine("-----------------------------");
		    list.Changed();
            /*
            //用新增具体中介者定义中介者对象
		    SubConcreteMediator mediator;
		    mediator = new SubConcreteMediator();
		    
            //创建按钮对象
		    Button addBT = new Button();
            //创建列表对象
            List list = new List();
            //创建复选框对象
            ComboBox cb = new ComboBox();
            //创建文本框对象
            TextBox userNameTB = new TextBox();
            //创建标签对象
            Label label = new Label();

            //按钮这是中介对象
		    addBT.SetMediator(mediator);
		    list.SetMediator(mediator);
		    cb.SetMediator(mediator);
		    userNameTB.SetMediator(mediator);
		    label.SetMediator(mediator);
		
            
		    mediator.addButton = addBT;
		    mediator.list = list;
		    mediator.cb = cb;
		    mediator.userNameTextBox = userNameTB;
		    mediator.label = label;
			
		    addBT.Changed();
		    Console.WriteLine("-----------------------------");
		    list.Changed();
            */

            Console.Read();
        }
    }
}
