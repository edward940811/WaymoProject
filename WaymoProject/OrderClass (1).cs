using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaymoProject
{
    class OrderClass
    {
        String senderID;
        Int32 cardNo;
        String receiverID;
        Int32 amount;
        Int32 unitPrice;
        DateTime time;

        public void setSenderID(String id)
        {
            this.senderID = id;
        }
        public String getSenderID()
        {
            return senderID;
        }
        public void setCardNo(Int32 no)
        {
            this.cardNo = no;
        }
        public Int32 getCardNo()
        {
            return cardNo;
        }
        public void setReceiverID(String receiver)
        {
            this.receiverID = receiver;
        }
        public String getReceiverID()
        {
            return receiverID;
        }
        public void setAmount(Int32 am)
        {
            this.amount = am;
        }
        public Int32 getAmount()
        {
            return amount;
        }
        public void setUnitPrice(Int32 price)
        {
            this.unitPrice = price;
        }
        public Int32 getUnitPrice()
        {
            return unitPrice;
        }
        public void setDateTime(DateTime date)
        {
            this.time = date;
        }
        public DateTime getTime()
        {
            return time;
        }
    }
}
