using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace WaymoProject
{
    //Stockmarket1 and Stockmarket2 are for two different plants
    //each is runned by a thread that alters the stock price 
    //through random number deciding to rise or to fall
    public class Stockmarket
    {
        public static Random rng = CarProject.rng;
        Double stockprice = rng.Next(90, 140);
        public void stockFunc()
        {   
            for (int i = 0; i < 10; i++)
            {
                Int32 financeevents = rng.Next(0, 10);
                Thread.Sleep(50);
                ////Console.WriteLine(stockprice);
                if (financeevents < 6)   // stockfalls
                {
                    stockprice = stockprice * 0.82;
                }
                else if (financeevents > 6)   // stockrises
                    stockprice *= 1.15;
                else if (financeevents == 6)   // financial crisis
                    stockprice *= 0.7;

                if (stockprice < 70)    // in case of too bias
                    stockprice = 100;
                if (stockprice > 180)
                    stockprice = 140;
                //Console.WriteLine("stock price rn:{0}",stockprice);
            }
        }

        public double getstockprice()
        {
            //Console.WriteLine("returned price:{0}", stockprice);
            return stockprice;
        }
    }
    public class Stockmarket2
    {
        public static Random rng2 = CarProject.rng;
        Double stockprice = rng2.Next(90, 140);

        public void stockFunc()
        {
            for (int i = 0; i < 10; i++)
            {
                Int32 financeevents = rng2.Next(0, 10);
                Thread.Sleep(500);
                ////Console.WriteLine(stockprice);
                if (financeevents < 6)   // stockfalls
                {
                    stockprice = stockprice * 0.82;
                }
                else if (financeevents > 6)   // stockrises
                    stockprice *= 1.15;
                else if (financeevents == 6)   //financial crisis
                    stockprice *= 0.7;

                if (stockprice < 70)    // in case of too bias
                    stockprice = 100;
                if (stockprice > 180)
                    stockprice = 140;
            }
        }

        public double getstockprice()
        {
            return stockprice;
        }
    }
}
