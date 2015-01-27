
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Web;
using System.Linq;

namespace QN
{
    public class SyncHttp
    {
        /// <summary>
        /// ͬ����ʽ����http get����
        /// </summary>
        /// <param name="url">����·��</param>
        /// <param name="queryString">�������</param>
        /// <returns></returns>
        public string HttpGet(string url, string queryString)
        {
            string responseData = null;

            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + queryString;
            }

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;

            StreamReader responseReader = null;

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
                webRequest = null;
            }

            return responseData;
        }

        /// <summary>
        /// ͬ����ʽ����http post����
        /// </summary>
        /// <param name="url">����·��</param>
        /// <param name="queryString">�������</param>
        /// <returns></returns>
        public string HttpPost(string url, string queryString)
        {
            StreamWriter requestWriter = null;
            StreamReader responseReader = null;

            string responseData = null;

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;

            try
            {
                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(queryString);
                requestWriter.Close();
                requestWriter = null;

                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (requestWriter != null)
                {
                    requestWriter.Close();
                    requestWriter = null;
                }

                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader = null;
                }

                webRequest.GetResponse().GetResponseStream().Close();
                webRequest = null;
            }

            return responseData;
        }

        /// <summary>
        /// ͬ����ʽ����http post���󣬿���ͬʱ�ϴ��ļ�
        /// </summary>
        /// <param name="url">����·��</param>
        /// <param name="queryString">�������</param>
        /// <param name="files">��Ҫ�ϴ����ļ�</param>
        /// <returns></returns>
        public string HttpPostWithFile(string url, string queryString, List<Parameter> files)
        {
            Stream requestStream = null;
            StreamReader responseReader = null;
            string responseData = null;
            string boundary = DateTime.Now.Ticks.ToString("x");
            url += '?' + queryString;
            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                Stream memStream = new MemoryStream();

                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                List<Parameter> listParams = HttpUtil.GetQueryParameters(queryString);

                foreach (Parameter param in listParams)
                {
                    if (param.Name != "content")
                    {
                        string formitem = string.Format(formdataTemplate, param.Name, param.Value);
                        byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                    else
                    {
                        string formitem = string.Format(formdataTemplate, param.Name, System.Web.HttpUtility.UrlDecode(param.Value));
                        byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                memStream.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"{2}\"\r\n\r\n";

                foreach (Parameter param in files)
                {
                    string name = param.Name;
                    string filePath = param.Value;
                    string file = Path.GetFileName(filePath);
                    string contentType = HttpUtil.GetContentType(file);

                    string header = string.Format(headerTemplate, name, file, contentType);
                    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                    memStream.Write(headerbytes, 0, headerbytes.Length);

                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }

                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    fileStream.Close();
                }

                webRequest.ContentLength = memStream.Length;

                requestStream = webRequest.GetRequestStream();

                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
                requestStream = null;

                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                    requestStream = null;
                }

                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader = null;
                }

                webRequest.GetResponse().GetResponseStream().Close();
                webRequest = null;
            }

            return responseData;
        }

        /// <summary>
        ///  ͬ����ȡ������Ӧ
        /// </summary>
        /// <param name="url">����·��</param>
        /// <param name="postData">�������</param>
        /// <returns></returns>
        public string GetHttpStream(string url, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);

            try
            {
                // ���ò���
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();


                //�������󲢻�ȡ��Ӧ��Ӧ����
                response = request.GetResponse() as HttpWebResponse;

                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);


                //���ؽ��
                string content = sr.ReadToEnd();
                string err = string.Empty;

                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                System.Web.HttpContext.Current.Response.Write(err);

                return string.Empty;
            }
        }

        /// <summary>
        /// ͬ�������ļ�
        /// </summary>
        /// <param name="url">�ļ�url</param>
        /// <param name="savePath">�ļ��ı��ر���·��</param>
        public void HttpDownload(string url, string savePath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream stream = response.GetResponseStream();
            byte[] buffer = new byte[1024];

            using (System.IO.FileStream fs = new FileStream(savePath, FileMode.Create))
            {
                int len = 0;
                while ((len = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, len);
                };
            }
        }

        /// <summary>
        /// Post��ʽͬ�������ļ���������Ҫ�����֤
        /// </summary>
        /// <param name="url">�ļ�url</param>
        /// <param name="savePath">�ļ��ı��ر���·��</param>
        /// <param name="user">�û���</param>
        /// <param name="user">����</param>
        /// <param name="fileName"></param>
        public bool HttpPostDownload(string url, string queryString, string savePath, string authKey, out string fileName)
        {
            StreamWriter requestWriter = null;

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 600000;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            webRequest.Headers.Add("AUTH_KEY", authKey);

            try
            {
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(queryString);
                requestWriter.Close();
                requestWriter = null;

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[1024];

                fileName = GetFileNameFromHeaders(response.Headers);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new Exception("Զ�̷����������˴�����ļ����ơ�");
                }

                string newFile = Path.Combine(savePath, fileName);

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
                using (System.IO.FileStream fs = new FileStream(newFile, FileMode.Create))
                {
                    int len = 0;
                    while ((len = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, len);
                    };
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (requestWriter != null)
                {
                    requestWriter.Close();
                    requestWriter = null;
                }

                webRequest.GetResponse().GetResponseStream().Close();
                webRequest = null;
            }

            return true;
        }

        /// <summary>
        /// ����һ�����󣬲����ظ�����
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queryString"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public HttpWebResponse PutRequest(string url, string queryString, Dictionary<string, string> headers)
        {
            StreamWriter requestWriter = null;

            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 600000;
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            foreach (string k in headers.Keys)
            {
                webRequest.Headers.Add(k, headers[k]);
            }

            try
            {
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(queryString);
                requestWriter.Close();
                requestWriter = null;

                return (HttpWebResponse)webRequest.GetResponse();
            }
            catch
            {
                webRequest.GetResponse().GetResponseStream().Close();
                webRequest = null;

                throw;
            }
            finally
            {
                if (requestWriter != null)
                {
                    requestWriter.Close();
                    requestWriter = null;
                }

            }
        }

        /// <summary>
        /// ��ȡ�ļ�������Ӧͷ�е��ļ���
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public string GetFileNameFromHeaders(WebHeaderCollection headers)
        {
            try
            {
                return headers["Content-Disposition"].Split(';')
                        .Where(m => m.Contains("filename"))
                        .First().Split('=')[1];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// �ж�ĳ����Ӧ�Ƿ����ļ�����
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool IsResponseFile(WebHeaderCollection headers)
        {
            try
            {
                return headers["Content-Disposition"].Split(';')
                        .Any(m => m.Contains("attachment"));
            }
            catch
            {
                return false;
            }
        }
    }
}
