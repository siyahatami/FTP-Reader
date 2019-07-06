using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Threading;

namespace FtpWebApplication1
{
    public class myFtpRangeStream
    {

        public bool canDownloadFileSuccessfully()
        {

            long length = GetFileLength("FullPath", "userName", "password", false);
            long offset = 0;
            int retryCount = 10;
            int? readTimeout = 5 * 60 * 1000; //five minutes

            while (retryCount > 0)
                using (Stream responseStream = GetFileAsStream("FullPath", "userName", "password", false, offset,
                    requestTimeout: readTimeout != null ? readTimeout.Value : Timeout.Infinite))
                {

                    using (FileStream fileStream = new FileStream("filePath", FileMode.Append))
                    {
                        byte[] buffer = new byte[4096];
                        try
                        {
                            int bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                            while (bytesRead > 0)
                            {
                                fileStream.Write(buffer, 0, bytesRead);
                                bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                            }
                            return true;
                        }
                        catch (System.Net.WebException)
                        {
                            // Do nothing - consume this exception to force a new read of the rest of the file
                        }
                    }


                    offset = File.Exists("filePath") ? new FileInfo("filePath").Length : 0;
                    retryCount--;

                    if (offset == length)
                        return true;

                }

            return true;
        }

        public static Stream GetFileAsStream(
            string ftpUrl, string username, string password, bool usePassive, long offset, int requestTimeout)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);

            request.KeepAlive = false;
            request.ReadWriteTimeout = requestTimeout;
            request.Timeout = requestTimeout;
            request.ContentOffset = offset;
            request.UsePassive = usePassive;
            request.UseBinary = true;
            request.Credentials = new System.Net.NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            Stream fileResponseStream;
            FtpWebResponse fileResponse = (FtpWebResponse)request.GetResponse();
            fileResponseStream = fileResponse.GetResponseStream();

            return fileResponseStream;
        }


        public static long GetFileLength(string ftpUrl, string username, string password, bool usePassive)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);

            request.KeepAlive = false;
            request.UsePassive = usePassive;
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            using (FtpWebResponse lengthResponse = (FtpWebResponse)request.GetResponse())
                return lengthResponse.ContentLength;

        }




    }
}