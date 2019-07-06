using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace FtpWebApplication1
{
    public class FtpTest
    {


        public static string download(int threadId, int FileId)
        {
            string filename = "file0_" + FileId + ".rar";
            string RealFilename = "file"+threadId+"_" + FileId + ".rar";
            string ftpFileName = "ftp://localhost/" + filename ;
            string ftpRealFileName = "ftp://localhost/" + RealFilename;
            try
            {
                
                if (!doesFileExistsOnFtp(ftpFileName))
                    return "file"+filename+" does not exist. -->> "+upload(ftpRealFileName);

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpFileName);
                ftpRequest.Credentials = new NetworkCredential("s.hatami", "siam@Siam6");
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                
                
                using (Stream ftpResponseStream = ftpRequest.GetResponse().GetResponseStream())
                {
                    //string fileaddress = System.Web.Hosting.HostingEnvironment.MapPath(@"~/files/") + ftpRealFileName; // valid for threads
                    //if (File.Exists(fileaddress))
                    //    File.Delete(fileaddress);

                    //using (FileStream file = System.IO.File.Create(fileaddress))
                    //{
                    byte[] buffer = new byte[10240];
                    int read;
                    while ((read = ftpResponseStream.Read(buffer, 0, buffer.Length)) > 0)
                        ;// file.Write(buffer, 0, read);

                    //    file.Close();
                    //}
                }

                return "Read";
            }
            catch (WebException er)
            {
                String status = ((FtpWebResponse)er.Response).StatusDescription;
                return "Read file"+ filename + " - Error status("+status+"): "+ er.Message;
            }
        }



        public static string upload(string ftpFileName)
        {
            FtpWebResponse response=null;
            try
            {
                string localfile = System.Web.Hosting.HostingEnvironment.MapPath(@"~/files/file.rar"); // valid for threads

                using (FileStream fileStream = new FileStream(
                          localfile, FileMode.Open, FileAccess.Read))
                {
                    int oneMB = 1048576;
                    byte[] buffer = new byte[oneMB];
                    int read;
                    
                    while ((read = fileStream.Read(buffer, 0, oneMB)) > 0)
                    {
                        FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(ftpFileName);
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

                        response = (FtpWebResponse)request.GetResponse();
                        bool ftpSuccessful = response.StatusDescription.StartsWith("226") ? true : false;
                        response.Close();
                    }
                    fileStream.Close();
                }

                return "Uploaded";
            }
            catch (WebException er)
            {
                if (response != null)
                    response.Close();
                String status = ((FtpWebResponse)er.Response).StatusDescription;
                return "Upload - Error("+ status + "): " + er.Message;
            }
        }


        private static bool doesFileExistsOnFtp(string fileName)
        {
            var request = (FtpWebRequest)WebRequest.Create(fileName);
            request.Credentials = new NetworkCredential("s.hatami", "siam@Siam6");
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }

    }
}