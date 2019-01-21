using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace MNPOST.Utils
{
    public class RequestHandle
    {
       
        public static string SendPost(string url, string postData)
        {
            var data = Encoding.ASCII.GetBytes(postData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.Headers["Authorization"] = RequetToken();

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;

        }

        public static string RequetToken()
        {

            var postData = "grant_type=password&username=adminga@admin.com&password=Abc@1234";

            var data = Encoding.ASCII.GetBytes(postData);

            string url = APISource.ROOTURL + "mntoken";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var res = Json.Decode(responseString);

            return "bearer " + res.access_token;
        }
    }

    public class APISource
    {
        public static string ROOTURL = "http://miennampost.vn/";
    }

}