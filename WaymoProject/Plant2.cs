using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;


namespace WaymoProject
{
    public class Plant2
    {
        Int32 carsAvailable = 100;
        Int32 orderstaken = 0;
        object locker = new Object();
        static Random rng = new Random();
        Random rng2 = CarProject.rng;
        public static event priceCutEvent priceCut;
        private static Int32 carPrice = 200;
        private static Int32 priceSaleToDealers = 501;
        String order;
        static String plantName2;
        OrderProcessing orderprocessing;
        //start thread here
        public void PlantFunc()
        {
            plantName2 = Thread.CurrentThread.Name;
            for (Int32 i = 0; i < 5; i++)
            {
                Thread.Sleep(1500);
                //// mod ////
                double stockprice = CarProject.stockmarket2.getstockprice();
                Int32 p = PricingModel(orderstaken, carsAvailable, stockprice);
                //// mod ////
                //Console.WriteLine("{0} new price is {1}", Thread.CurrentThread.Name, p);
                Plant2.carPriceChange(p);
                //Thread.Sleep(3000);
                order = CarProject.Cellbuffer.getOneCell(Thread.CurrentThread.Name);
                //Decoder(order);
                if (!"0".Equals(order))
                {
                    lock (locker)
                    {
                        carsAvailable--;
                        orderstaken++;
                    }
                    Console.WriteLine("edward received the order {0}", order);
                  orderprocessing = new OrderProcessing(Decoder(order));
                }
            }
        }

        private OrderClass Decoder(string s)
        {
            string[] stringSeparators = new string[] { "|" };
            string[] result;
            result = s.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            OrderClass decodedobj = new OrderClass();
            decodedobj.setSenderID(result[0]);
            decodedobj.setCardNo(Int32.Parse(result[1]));
            decodedobj.setReceiverID(result[2]);
            decodedobj.setAmount(Int32.Parse(result[3]));
            decodedobj.setUnitPrice(Int32.Parse(result[4]));
            decodedobj.setDateTime(Convert.ToDateTime(result[5]));
            return decodedobj;
        }
        //If price drop, remind the drop to dealer
        public static void carPriceChange(Int32 price)
        {
            if (price < priceSaleToDealers || price < 200)
            {
                if (priceCut != null)
                {
                    priceCut(price);
                }
                priceSaleToDealers = price;
            }
        }
        //deciding the car price in here
        public Int32 PricingModel(Int32 orderstaken, Int32 carsAvailable, double stockprice)
        {
            ////  mod  ////
            carPrice = (int)((carPrice * stockprice / 100) + ((101 - carsAvailable) * 4 / 100));

            return carPrice;
            ////  mod  ////
        }
        //return the price
        public Int32 getPrice()
        {
            return priceSaleToDealers;
        }
        public String getPlantName()
        {
            return plantName2;
        }
    }
}
