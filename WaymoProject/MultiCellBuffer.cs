using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace WaymoProject
{
    public class MultiCellBuffer
    {
        private String[] bufferCell = new String[] {"","",""};
        private static Semaphore pool;
        private Int32 semaphoreValue;
        public MultiCellBuffer()
        {
            pool = new Semaphore(3,3);
            semaphoreValue = 3;
        }
        public void setOneCell(String orderString)
        {
            lock (this)
            {
                if (semaphoreValue==0)
                {
                    try
                    {
                        Monitor.Wait(this,1000);
                    }
                    catch
                    {
                        Console.WriteLine("error");
                    }
                }
                pool.WaitOne(1000);
                //Console.WriteLine("Thread {0} enter the semaphore", Thread.CurrentThread.Name);
                for(int i = 0; i < bufferCell.Length; i++)
                {
                    if ("".Equals(bufferCell[i]))
                    {
                        bufferCell[i] = orderString;
                        break;
                    }
                }
                semaphoreValue--;
                Monitor.PulseAll(this);
                //Console.WriteLine("now the space is"pool.Release());
                //Console.WriteLine("Thread {0} wake up and put order in buffer.", Thread.CurrentThread.Name);
            }
        }
        public String getOneCell(String plantName)
        {
            String value="";
            lock (this)
            {
                if (semaphoreValue==3)
                {
                    try
                    {
                        Monitor.Wait(this,1000);
                    }
                    catch
                    {
                        Console.WriteLine("error");
                    }
                }
                //check data if the receiver is correct & receive
                for(int i = 0; i < bufferCell.Length; i++)
                {
                    if (!"".Equals(bufferCell[i]))
                    {
                        String[] s=bufferCell[i].Split('|');
                        if(!"".Equals(s[2])&&plantName.Equals(s[2]))
                        {
                            value = bufferCell[i];
                            bufferCell[i] = "";
                            break;
                        } 
                    }
                }
                Monitor.PulseAll(this);
                //邏輯問題,因為DEALER只有sale時才會買東西(SET資料進buffer),若plant產生不是SALE的數字,plant會無限等待SET程式死亡
                if (semaphoreValue != 3 && !"".Equals(value))
                {
                    semaphoreValue++;
                    //Console.WriteLine("now the space is {0}", pool.Release() + 1);
                    return value;
                }else
                {
                    return "0";
                }    
            }
        }
    }
}
