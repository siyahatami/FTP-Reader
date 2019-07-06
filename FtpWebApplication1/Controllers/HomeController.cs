using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FtpWebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }




        public string download()
        {
            FtpWebRequest ftpRequest = (FtpWebRequest) WebRequest.Create("ftp://localhost/file.rar");
            ftpRequest.Credentials = new NetworkCredential("s.hatami", "siam@Siam6");
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            
            // to download an specific range: use myFtpRangeStream class

            using (Stream ftpResponseStream = ftpRequest.GetResponse().GetResponseStream())
            {
                
                string fileaddress = System.Web.Hosting.HostingEnvironment.MapPath(@"~/files/file.rar");
                using (FileStream file = System.IO.File.Create(fileaddress))
                {
                    byte[] buffer = new byte[10240];
                    int read;
                    while ((read = ftpResponseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        
                        file.Write(buffer, 0, read);
                    }
                    file.Close();
                }
            }

            return "downloaded";
        }

       

        public string upload()
        {
            
            using (FileStream fileStream = new FileStream(
                    System.Web.Hosting.HostingEnvironment.MapPath(@"~/files/file.rar"), FileMode.Open, FileAccess.Read))
            {

                

                int oneMB = 1048576;
                byte[] buffer = new byte[oneMB];
                int read;
                while ((read = fileStream.Read(buffer, 0, oneMB)) > 0)
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create("ftp://localhost/file2.rar");
                    request.Method = WebRequestMethods.Ftp.AppendFile;
                    request.Credentials = new NetworkCredential("s.hatami", "siam@Siam6");
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.KeepAlive = false;

                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(buffer, 0, read);
                    reqStream.Flush();
                    reqStream.Close();
                    reqStream.Dispose();

                    var response = (FtpWebResponse) request.GetResponse();
                    bool ftpSuccessful = response.StatusDescription.StartsWith("226") ? true : false;
                }
                fileStream.Close();
            }

            return "Uploaded Successfully";
        }

    }
}