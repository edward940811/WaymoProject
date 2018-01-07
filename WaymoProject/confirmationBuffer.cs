using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace WaymoProject
{
    public class confirmationBuffer
    {
        private String bufferCell = "";
        private Boolean writeable = true;
        public void setOneCell(String confirmationString)
        {
            lock (this)
            {
                if (!writeable)
                {
                    try
                    {
                        Monitor.Wait(this);
                    }
                    catch
                    {
                        Console.WriteLine("error");
                    }
                }
                bufferCell = confirmationString;
                writeable = false;
                Monitor.PulseAll(this);
            }
        }
        public String getOneCell(String threadName)
        {
            lock (this)
            {
                if (writeable)
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
                if (!"".Equals(bufferCell))
                {
                    writeable = true;
                    String[] stringArr = bufferCell.Split('|');
                    if (threadName.Equals(stringArr[1]))
                    {
                        bufferCell = "";
                        System.TimeSpan diff1 = DateTime.Now.Subtract(Convert.ToDateTime(stringArr[2]));
                        var RetValue = string.Format("{0} seconds", diff1);
                        Console.WriteLine(DateTime.Now+" | "+ stringArr[2] +" | " + diff1.Seconds + "." + diff1.Milliseconds);
                        return stringArr[0]+ stringArr[1] ;
                    }
                }
                Monitor.PulseAll(this);
                return "0";
            }
        }
    }
}
