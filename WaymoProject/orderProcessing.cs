using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace WaymoProject
{
    class OrderProcessing
    {
        private Thread thread;
        OrderClass order;
        Boolean creditCardStatus;
        public OrderProcessing(OrderClass orderObject)
        {
            order = orderObject;
            thread = new Thread(new ThreadStart(orderProcessFunc));
            thread.Start();
        }
        public void orderProcessFunc()
        {
            creditCardStatus = isCreditCardVaild();
            if (creditCardStatus == true)
            {
                Console.WriteLine("send the confirmation to the dealers");
                CarProject.confirmBuffer.setOneCell("order is completed" +"|"+order.getSenderID() + "|" + order.getTime());

            }else
            {
                Console.WriteLine("The order is rejected");
            }
        }
        public Boolean isCreditCardVaild()
        {
            if (order.getCardNo() >= 5000 && order.getCardNo() <= 7000)
            {
                return true;
            }else
            {
                return false;
            }
        } 
    }
}
