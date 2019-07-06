using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace FtpWebApplication1.Controllers
{
    public class AdminController : Controller
    {

        public ActionResult Index()
        {
            //string trustedString = System.Web.HttpUtility.HtmlAttributeEncode("InputUrl");

            return View();
        }



        public ActionResult download()
        {

            return View();
        }

        

        [HttpPost]
        public string upload(string fileName)
        {
            //string path = "http://localhost:20231/web/upload/";

            //return forwardToWeb(url, Uri.EscapeDataString(binaryBlockBase64), fileName);

            try
            {
                foreach (string upload in Request.Files)
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "files";
                    //string _filename = Path.GetFileName(Request.Files[upload].FileName);
                    string resultPath = Path.Combine(path, fileName);
                    //Request.Files[upload].SaveAs(resultPath);
                    string inputContent;
                    using (BinaryReader b = new BinaryReader(Request.Files[upload].InputStream))
                    {
                        byte[] binData = b.ReadBytes(Request.Files[upload].ContentLength);

                        BinaryWriter bw = null;
                        try
                        {
                            bw = new BinaryWriter(new FileStream(resultPath, FileMode.Append));
                        }
                        catch (IOException e)
                        {
                            if (bw != null)
                                bw.Close();
                            return "\n Cannot create file";
                        }
                        try
                        {
                            bw.Write(binData);
                            bw.Close();
                        }
                        catch (IOException e)
                        {
                            if (bw != null)
                                bw.Close();
                            return "Cannot write to file";
                        }

                    }
                    return "uploaded";
                } //foreach

                return "empty file";
            }
            catch (Exception ee)
            {
                return "server side error";
            }


        }
        
        private string appendToFile(string binaryBlockBase64, string fileName)
        {
            BinaryWriter bw = null;

            try
            {
                string fileaddress = System.Web.Hosting.HostingEnvironment.MapPath(@"~/files/" + fileName);
                bw = new BinaryWriter(new FileStream(fileaddress, FileMode.Append));
            }
            catch (IOException e)
            {
                if (bw != null)
                    bw.Close();
                return "\n Cannot create file";
            }


            try
            {
                byte[] data = Convert.FromBase64String(Server.UrlDecode(binaryBlockBase64));
                bw.Write(data);
                bw.Close();
            }
            catch (IOException e)
            {
                if (bw != null)
                    bw.Close();
                return "Cannot write to file";
            }

            return "Worte successfully";

        }



        


        string forwardToWeb(string url, string data,string filename)
        {

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Timeout = 60000;
            request.Method = HttpMethod.Post.Method;
            request.AllowAutoRedirect = true;
            request.ContentType = "application/x-www-form-urlencoded";


            byte[] byteArray = Encoding.UTF8.GetBytes("binaryBlockBase64="+data+"&fileName="+filename);
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            

            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
                if (response == null || request.HaveResponse == false)
                {
                    if (response != null)
                        response.Close();
                    return "Error: Url not found";
                }

                var encoding = Encoding.GetEncoding(response.CharacterSet);
                StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                string result = reader.ReadToEnd();
                response.Close();
                if (reader != null)
                    reader.Close();

                return result;
            }
            catch (WebException er)
            {
                if (response != null) 
                    response.Close(); 
                if (er.Response != null)
                    return new StreamReader(er.Response.GetResponseStream()).ReadToEnd();

                return "Error: Response = null";
            }

        }

        


    }
}