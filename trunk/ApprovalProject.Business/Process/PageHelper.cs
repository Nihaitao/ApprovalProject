using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

public class PageHelper
{
    /// <summary>
    /// 延迟毫秒数
    /// </summary>
    protected int defer;
    /// <summary>
    /// 与请求相关的cookie（用于保持session）
    /// </summary>
    public CookieContainer cookies = new CookieContainer();

    public PageHelper()
    {
        this.cookies = new CookieContainer();
        //默认为1秒
        this.defer = 1000;
    }

    /// <summary>
    /// 发送Post类型请求
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="postData">参数</param>
    /// <returns></returns>
    public WebResponse doPost(string url, string postData)
    {
        try
        {
            Thread.Sleep(this.defer);
            byte[] paramByte = Encoding.UTF8.GetBytes(postData); // 转化
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Referer = "";
            webRequest.Accept = "application/x-shockwave-flash, image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-silverlight, */*";
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; CIBA)";
            webRequest.ContentLength = paramByte.Length;
            webRequest.CookieContainer = this.cookies;
            //webRequest.Timeout = 5000;
            Stream newStream = webRequest.GetRequestStream();
            newStream.Write(paramByte, 0, paramByte.Length);    //写入参数
            newStream.Close();
            return webRequest.GetResponse();
        }
        catch (Exception ce)
        {
            throw ce;
        }
    }

    /// <summary>
    /// 发送Post类型请求
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="postData">参数</param>
    /// <returns></returns>
    public WebResponse doPost(string url, string postData, string referer)
    {
        try
        {
            Thread.Sleep(this.defer);
            byte[] paramByte = Encoding.UTF8.GetBytes(postData); // 转化
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = this.cookies;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Referer = referer;

            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; CIBA)";
            webRequest.ContentLength = paramByte.Length;
            webRequest.Timeout = 5000;
            Stream newStream = webRequest.GetRequestStream();
            newStream.Write(paramByte, 0, paramByte.Length);    //写入参数
            newStream.Close();
            return webRequest.GetResponse();
        }
        catch (Exception ce)
        {
            throw ce;
        }
    }
    /// <summary>
    /// 发送Get类型请求
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="postData">参数</param>
    /// <returns></returns>
    public string doGet(string Url)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        catch (Exception ce)
        {
            throw ce;
        }
    }


    /// <summary>
    /// 发送Get类型请求
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="postData">参数</param>
    /// <returns></returns>
    public WebResponse doGetWeb(string url)
    {
        try
        {
            //Thread.Sleep(1000);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = this.cookies;
            webRequest.Method = "get";
            webRequest.Timeout = 5000;
            return webRequest.GetResponse();
        }
        catch (Exception)
        {
            return null;
            //throw ce;
        }
    }

    /// <summary>
    /// 根据相应返回字符串
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public string ResponseToString(WebResponse response)
    {
        try
        {
            Encoding encoding = Encoding.Default;
            Trace.WriteLine("------------------------------------------" +response.ContentType);
            string ContentType = response.ContentType.Trim();
            if (ContentType.IndexOf("utf-8",StringComparison.InvariantCultureIgnoreCase) != -1)
                encoding = Encoding.UTF8;
            else if (ContentType.IndexOf("utf-7", StringComparison.InvariantCultureIgnoreCase) != -1)
                encoding = Encoding.UTF7;
            else if (ContentType.IndexOf("unicode", StringComparison.InvariantCultureIgnoreCase) != -1)
                encoding = Encoding.Unicode;
            else if (ContentType.IndexOf("text/plain", StringComparison.InvariantCultureIgnoreCase) != -1)
                encoding = Encoding.UTF8;
            StreamReader stream = new StreamReader(response.GetResponseStream(), encoding);
            string code = stream.ReadToEnd();
            stream.Close();
            response.Close();
            return code;
        }
        catch (Exception ce)
        {
            throw ce;
        }
    }

    /// <summary>
    /// 根据相应返回字符串
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public string ResponseToStringWithGzip(WebResponse response)
    {
        try
        {
            Encoding encoding = Encoding.Default;
            string ContentType = response.ContentType.Trim().ToLower();
            if (ContentType.IndexOf("utf-8") != -1)
                encoding = Encoding.UTF8;
            else if (ContentType.IndexOf("utf-7") != -1)
                encoding = Encoding.UTF7;
            else if (ContentType.IndexOf("unicode") != -1)
                encoding = Encoding.Unicode;
            else if (ContentType.IndexOf("gbk") != -1)
                encoding = Encoding.GetEncoding("GBK");

            StreamReader stream;
            GZipStream gz = null;

            if (!string.IsNullOrEmpty(response.Headers["Content-Encoding"]) && response.Headers["Content-Encoding"].ToLower() == "gzip")
            {
                gz = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                stream = new StreamReader(gz, encoding);
            }
            else
            {
                stream = new StreamReader(response.GetResponseStream(), encoding);
            }
            string code = stream.ReadToEnd();
            stream.Close();
            if (gz != null)
                gz.Close();
            response.Close();
            return code;
        }
        catch (Exception ce)
        {
            throw ce;
        }
    }
    /// <summary>
    /// 模拟POST提交
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="uploadData">xml参数</param>
    /// <returns>返回结果</returns>
    public string PostJDWebHtml(String url, String uploadData)
    {
        HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        myHttpWebRequest.Method = "POST";
        myHttpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

        // Encode the data
        byte[] encodedBytes = Encoding.UTF8.GetBytes(uploadData);
        myHttpWebRequest.ContentLength = encodedBytes.Length;

        // Write encoded data into request stream
        Stream requestStream = myHttpWebRequest.GetRequestStream();
        requestStream.Write(encodedBytes, 0, encodedBytes.Length);
        requestStream.Close();

        HttpWebResponse result;

        try
        {
            result = (HttpWebResponse)myHttpWebRequest.GetResponse();
        }
        catch
        {
            return string.Empty;
        }

        if (result.StatusCode == HttpStatusCode.OK)
        {
            using (Stream mystream = result.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(mystream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        return null;

    }

}


