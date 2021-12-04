using System;
using System.Net;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.ComponentModel;

namespace AES_TeamTool.Utils
{
    public class MailHelper
    {
        public MailHelper()
        {
            MailMessageInstance = new MailMessage
            {
                From = new MailAddress("ccq_support_aes@corp.ats.net"),
                Subject = "New Changement Released",
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };
            MailMessageInstance.To.Add(new MailAddress("ccq_support_aes@cn.ats.net"));
            SmtpClientInstance = new SmtpClient
            {
                Credentials = new NetworkCredential("ccq_support_aes@corp.ats.net", "Atser123"),
                Host = "ccqmail1.infra.ats.net",
                EnableSsl = false,
            };
        }

        private MailMessage MailMessageInstance { get; set; }

        private SmtpClient SmtpClientInstance { get; set; }

        public bool SyncSendMail(List<string> toList, List<string> copyList, string builtHTML)
        {
            try
            {
                toList.ForEach(item => MailMessageInstance.To.Add(item));
                copyList.ForEach(item => MailMessageInstance.CC.Add(item));
                MailMessageInstance.Body = builtHTML;
                SmtpClientInstance.Send(MailMessageInstance);
                return true;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return false;
            }
            finally
            {
                MailMessageInstance.Dispose();
                SmtpClientInstance.Dispose();
            }
        }

        public bool AsyncSendMail(List<string> toList, List<string> copyList, string builtHTML)
        {
            try
            {
                SmtpClientInstance.SendCompleted += new SendCompletedEventHandler(SendMailResult);
                toList.ForEach(item => MailMessageInstance.To.Add(item));
                copyList.ForEach(item => MailMessageInstance.CC.Add(item));
                MailMessageInstance.Body = builtHTML;
                SmtpClientInstance.SendAsync(MailMessageInstance, MailMessageInstance.From);
                return true;
            }
            catch (Exception err)
            {
                LogHelper.WriteText(LogType.ERROR, err.Message);
                return false;
            }
            finally
            {
                MailMessageInstance.Dispose();
                SmtpClientInstance.Dispose();
            }
        }

        private void SendMailResult(object sender, AsyncCompletedEventArgs evt)
        {
            MailAddress token = (MailAddress)evt.UserState;
            if (evt.Cancelled)
            {
                LogHelper.WriteText(LogType.WARN, evt.UserState.ToString() + " Canceled");
            }
            if (evt.Error != null)
            {
                LogHelper.WriteText(LogType.ERROR, evt.Error.ToString());
            }
            else
            {
                LogHelper.WriteText(LogType.INFO, "Send Done");
            }
        }
    }

    public static class MailHTMLBuilder
    {
        public static MailHTMLType FormatType { get; set; }
        static MailHTMLBuilder()
        {
            FormatType = MailHTMLType.TABLE;
        }
        public static string BuildString(MailHTMLType type)
        {
            return null;
        }
    }

    public enum MailHTMLType
    {
        DIV = 1,
        TABLE = 2,
        PAGE = 3
    }

}