using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace QN
{
    /// <summary>
    /// 邮件辅助类
    /// </summary>
    public static class QMail
    {
        private static SmtpClient _client = new SmtpClient();

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="from">发件人</param>
        /// <param name="to">收件人</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        public static void Send(string to, string subject, string body)
        {
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentNullException("to");
            }

            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.To.Add(to);
            msg.Subject = subject;
            msg.SubjectEncoding = Encoding.UTF8;
            msg.Body = body;
            msg.BodyEncoding = Encoding.UTF8;

            _client.Send(msg);
        }

        /// <summary>
        /// 使用另一个客户端实例替换默认SMTP客户端实例（注意，这是全局的）
        /// </summary>
        /// <param name="client"></param>
        public static void ReplaceClient(SmtpClient client)
        {
            lock (_client)
            {
                _client = client;
            }
        }

        /// <summary>
        /// 使用一个邮件模版发送邮件
        /// </summary>
        /// <param name="templateid"></param>
        /// <param name="culture">语言culture</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subjecArgs">主题参数</param>
        /// <param name="bodyArgs">内容参数</param>
        public static void Send(string templateid, string culture, string to, string[] subjecArgs, string[] bodyArgs)
        {
            QMailTemplate template = LoadTemplate(templateid, culture);

            if (null == template)
            {
                throw new QRunException("无法找到邮件模版：" + templateid + "，语言类型为：" + culture);
            }

            if (null != subjecArgs)
            {
                template.subject = string.Format(template.subject, subjecArgs);
            }
            if (null != bodyArgs)
            {
                template.body = string.Format(template.body, bodyArgs);
            }

            Send(to, template.subject, template.body);
        }

        /// <summary>
        /// 使用一个邮件模版发送邮件
        /// </summary>
        /// <param name="templateid"></param>
        /// <param name="culture">语言culture</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subjecArgs">主题参数</param>
        /// <param name="bodyArgs">内容参数</param>
        public static void Send(string templateid, string to, string[] subjecArgs, string[] bodyArgs)
        {
            Send(templateid, "zh-CN", to, subjecArgs, bodyArgs);
        }

        private static QMailTemplate LoadTemplate(string templateid, string culture)
        {
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "App_Data/email/" + culture + ".json");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("无法找到邮件模版文件。", path);
            }

            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();

                IList<QMailTemplate> result = QJson.Deserialize<IList<QMailTemplate>>(content);

                return result.FirstOrDefault(m => m.id == templateid);
            }
        }
    }
}