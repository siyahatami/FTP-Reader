using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FtpWebApplication1.Controllers
{
    public class testController : Controller
    {
        // GET: test
        public string Index()
        {
            DateTime startTime = DateTime.Now;
            string resul = "";
            string newLine = @"<br/>";
            Random r = new Random(DateTime.Now.Millisecond);
            SemaphoreSlim _sem = new SemaphoreSlim(0);

            

            int threadsCount = 50;
            for (int i = 1; i < threadsCount+1; i++)
            {
                int temp = i;
                new Thread(() => {
                    string response = FtpTest.download(temp, r.Next(1,threadsCount));
                    resul += "Thread " + temp.ToString() + ":  " + response + newLine;
                    _sem.Release();
                }).Start();
            }


            for (int i = 1; i < threadsCount+1; i++)
                _sem.Wait();


            return "started: Duration is ---->>>>> " + 
                DateTime.Now.Subtract(startTime).ToString() + "<br/>" +
                resul;
        }
    }
}