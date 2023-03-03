using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace Fast.Framework.Utils
{

    /// <summary>
    /// Email工具类
    /// </summary>
    public static class Email
    {

        /// <summary>
        /// 发送电子邮件 异步
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <param name="to">到</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="files">文件名称(附件)</param>
        /// <returns></returns>
        public static async Task SendEmailAsync(string host, int port, string user, string pwd, List<string> to, string subject, string body, List<string> files = null)
        {
            using (var smtpClient = new SmtpClient(host, port))
            {
                smtpClient.Credentials = new NetworkCredential(user, pwd);
                foreach (var t in to)
                {
                    using (var mail = new MailMessage(user, t, subject, body))
                    {
                        mail.IsBodyHtml = Regex.IsMatch(body, "html");
                        mail.BodyEncoding = Encoding.UTF8;
                        mail.SubjectEncoding = Encoding.UTF8;
                        if (files != null && files.Count > 0)
                        {
                            foreach (var fileName in files)
                            {
                                if (File.Exists(fileName))
                                {
                                    mail.Attachments.Add(new Attachment(fileName)
                                    {
                                        NameEncoding = Encoding.UTF8
                                    });
                                }
                                else
                                {
                                    throw new ArgumentException($"文件名称:{fileName}不存在");
                                }
                            }
                        }
                        await smtpClient.SendMailAsync(mail);
                    }
                }
            }
        }
    }
}
