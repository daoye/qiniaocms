using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN.Service
{
    /// <summary>
    /// 在线信息服务
    /// </summary>
    public class OnlineInfoService
    {
        //获取新闻动态
        private const string getNewsUrl = "http://online.qiniaocms.com/api/article/list";
        //获取微博
        private const string getMicroblogUrl = "http://online.qiniaocms.com/api/microblog/list";
        //获取作者
        private const string getContributorUrl = "http://online.qiniaocms.com/api/contributor/list";

        public IList<OnlineArticleDTO> GetNewsList()
        {
            try
            {
                string result = PostRequest(getNewsUrl, null);

                if (string.IsNullOrEmpty(result) || "-1".Equals(result))
                {
                    return null;
                }
                else
                {
                    return QJson.Deserialize<IList<OnlineArticleDTO>>(result);
                }
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }

            return new List<OnlineArticleDTO>();
        }

        public IList<OnlineMicroblogDTO> GetMicroblogList()
        {
            try
            {
                string result = GetRequest(getMicroblogUrl, null);

                if (string.IsNullOrEmpty(result) || "-1".Equals(result))
                {
                    return null;
                }
                else
                {
                    return QJson.Deserialize<IList<OnlineMicroblogDTO>>(result);
                }
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }

            return new List<OnlineMicroblogDTO>();
        }

        public IList<OnlineContributorDTO> GetContributorList()
        {
            try
            {
                string result = PostRequest(getContributorUrl, null);

                if (string.IsNullOrEmpty(result) || "-1".Equals(result))
                {
                    return null;
                }
                else
                {
                    return QJson.Deserialize<IList<OnlineContributorDTO>>(result);
                }
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }

            return new List<OnlineContributorDTO>();
        }

        private string PostRequest(string url, string parameters)
        {
            SyncHttp http = new SyncHttp();
            string httpResult = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(parameters))
                {
                    parameters = string.Empty;
                }
                else
                {
                    if (!parameters.StartsWith("&"))
                    {
                        parameters = "&" + parameters;
                    }
                }

                parameters = "version=" + System.Web.HttpContext.Current.Server.UrlEncode(R.Version) + parameters;
                httpResult = http.HttpPost(url, parameters);

                return httpResult;
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
                return null;
            }
        }

        private string GetRequest(string url, string parameters)
        {
            SyncHttp http = new SyncHttp();
            string httpResult = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(parameters))
                {
                    parameters = string.Empty;
                }
                else
                {
                    if (!parameters.StartsWith("&"))
                    {
                        parameters = "&" + parameters;
                    }
                }

                parameters = "version=" + System.Web.HttpContext.Current.Server.UrlEncode(R.Version) + parameters;
                httpResult = http.HttpGet(url, parameters);

                return httpResult;
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
                return null;
            }
        }
    }
}