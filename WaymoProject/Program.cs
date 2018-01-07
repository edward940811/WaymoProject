using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace WaymoProject
{
    public delegate void priceCutEvent(Int32 pr);

    public class Plant
    {
        Int32 carsAvailable = 100;
        Int32 orderstaken = 0;
        object locker = new Object();
        static Random rng = new Random();
        public static event priceCutEvent priceCut;
        private static Int32 carPrice = 200;
        private static Int32 priceSaleToDealers = 501;
        String order;
        static String plantName1;
        OrderProcessing orderprocessing;
        //start thread here
        public void PlantFunc()
        {
            plantName1 = Thread.CurrentThread.Name;
            Console.WriteLine("The default price is {0}", priceSaleToDealers);
            for (Int32 i=0; i < 5; i++)
            {
                //Plant links to the stockmarket for stock price for calculation
                //on the price
                Thread.Sleep(1500);
                double stockprice = CarProject.stockmarket.getstockprice(); 
                Int32 p = PricingModel(orderstaken,carsAvailable,stockprice);
                Console.WriteLine("{0} new price is {1}",Thread.CurrentThread.Name,p);
                Plant.carPriceChange(p);
                order = CarProject.Cellbuffer.getOneCell(Thread.CurrentThread.Name);
                if (!"0".Equals(order))
                { 
                    lock (locker)
                    {
                        carsAvailable--;
                        orderstaken++;
                    }
                    Console.WriteLine("brian received the order {0} ", order );
                    orderprocessing = new OrderProcessing(Decoder(order));

                }    
            } 
        }
        //result array stores the values according to the '|'
        //which the string is originally SenderID|CarNo|RecieverID|Amount|UnitPrice|Datetime
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
        public Int32 PricingModel(Int32 orderstaken, Int32 carsAvailable , double stockprice)
        {
            ////  mod  ////
            double carpricewithstock = carPrice * stockprice / 100;
            carPrice = (int)(carpricewithstock + ((101-carsAvailable) * 4 / 100));   
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
            return plantName1;
        }
    }
    public class CarDealer
    {
        OrderClass order = new OrderClass();
        String orderString;
        static Random rng = new Random();
        ConcurrentDictionary<Int32, Boolean> carStatus = new ConcurrentDictionary<Int32, Boolean>();
        public void carDealerFunc()
        {
            Plant carPlant = new Plant();
            Plant2 carPlant2 = new Plant2();
            String receiverName;
            Int32 parkingLot = 10;
            Int32 creditCard = rng.Next(5000, 7000);
            Int32 carPrice;
            Int32 carPrice2;  //Plant2 carprice
            Int32 dealPrice;
            Int32 plantIndex;
            for (Int32 i = 0; i < 10; i++)
            {
                Thread.Sleep(rng.Next(500,1000));
                carPrice = carPlant.getPrice();
                carPrice2 = carPlant2.getPrice();
                if (carPrice > carPrice2)
                { 
                    dealPrice = carPrice2;
                    plantIndex = 2;
                }
                else {
                    dealPrice = carPrice;
                    plantIndex = 1;
                }
                //check if the car in the sell list 
                if (!carStatus.ContainsKey(dealPrice))
                {
                    carStatus.TryAdd(dealPrice, false);
                }
                //the dealer only buy the sale car or the car price is lower than 200
                //the first dealer which have the empty places can buy the car, the others need to wait for next price cut.
                lock (order)
                {
                    if (parkingLot != 0 && carStatus[dealPrice] == false)
                    {
                        if (plantIndex == 1)
                        {
                            receiverName = carPlant.getPlantName();
                        }
                        else
                        {
                            receiverName = carPlant2.getPlantName();
                        }                 
                        try
                        {
                            order.setSenderID(Thread.CurrentThread.Name);
                            order.setCardNo(creditCard);
                            order.setReceiverID(receiverName);
                            order.setAmount(1);
                            order.setUnitPrice(dealPrice);
                            order.setDateTime(DateTime.Now);
                            CarProject.Cellbuffer.setOneCell(this.Encoder(order));
                        }
                        finally
                        {
                            parkingLot--;
                            carStatus[dealPrice] = true;
                            Console.WriteLine("dealer {0} buy the car, the empty lot is {1}, the price is {2}", Thread.CurrentThread.Name, parkingLot, dealPrice);
                        }
                    }
                }
                String confirmString=CarProject.confirmBuffer.getOneCell(Thread.CurrentThread.Name);
                if (!"0".Equals(confirmString))
                {
                    Console.WriteLine(confirmString);
                }
            }
        }
        //event handler
        //remind the dealers there has a price cut
        public void carOnSale(Int32 p)
        {
            Console.WriteLine("{0} car is on sale or price is low : {1}", Thread.CurrentThread.Name, p);
            //may cause some problem
            //plantName = Thread.CurrentThread.Name;
        }
        //convert the orderObject into a String
        private String Encoder(OrderClass order)
        {
            orderString = order.getSenderID() + "|" + order.getCardNo() + "|" + order.getReceiverID() + "|" + order.getAmount()+"|"+ order.getUnitPrice()+"|"+order.getTime();
            Console.WriteLine(orderString);
            return orderString;
        }
        public String getOrderString()
        {
            return orderString;
        }
    }
    public class CarProject
    {
        public static MultiCellBuffer Cellbuffer = new MultiCellBuffer();
        public static confirmationBuffer confirmBuffer = new confirmationBuffer();
        public static Random rng = new Random();
        public static Stockmarket stockmarket = new Stockmarket();
        public static Stockmarket2 stockmarket2 = new Stockmarket2();
        static void Main(String [] args)
        {
            Plant carPlant = new Plant();
            Plant2 carPlant2 = new Plant2();
            Thread plant = new Thread(new ThreadStart(carPlant.PlantFunc));
            Thread plant2 = new Thread(new ThreadStart(carPlant2.PlantFunc));
            //
            Thread stocks = new Thread(new ThreadStart(stockmarket.stockFunc));
            Thread stocks2 = new Thread(new ThreadStart(stockmarket2.stockFunc));
            //
            stocks.Start();
            stocks2.Start();
            plant.Name = "brian";
            plant2.Name = "edward";
            plant.Start();
            plant2.Start();
            CarDealer dealer = new CarDealer();
            Plant.priceCut += new priceCutEvent(dealer.carOnSale);
            Plant2.priceCut += new priceCutEvent(dealer.carOnSale);
            Thread[] dealers = new Thread[2];
            for(int i = 0; i < 2; i++)
            {
                dealers[i] = new Thread(new ThreadStart(dealer.carDealerFunc));
                dealers[i].Name = (i + 1).ToString();
                dealers[i].Start();
            }

            Console.ReadLine();
        }
    }
}
