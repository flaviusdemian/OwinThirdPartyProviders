using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailUtilsRazor
{
    internal class EmailGenerator
    {
        #region Constants

        internal const string REGEX_EMAIL = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
        internal const string LAUNCHPAD_SENDER_EMAIL = "Admin@HackTM.com";

        #endregion

        internal static SmtpClient CreateSmtpClient()
        {
            SmtpClient smtpClient = new SmtpClient();
            //smtpClient.EnableSsl = true;
            //smtpClient.UseDefaultCredentials = false;
            //smtpClient.Port = SMTP_SERVER_PORT;
            //NetworkCredential loginInfo = new NetworkCredential();// (SENDER_EMAIL_ACCOUNT, SENDER_EMAIL_PASSWORD);
            //smtpClient.Credentials = loginInfo;
            return smtpClient;
        }

        /// <summary>
        /// .Creates an email message using a html body
        /// </summary>
        /// <param name="toEmail"> receiver</param>
        /// <param name="subject"> the subject of the email</param>
        /// <param name="htmlBody">the body of the email</param>
        /// <param name="extractAndAddText">choose wheter you want to extract the text from the html and include that text into the mail</param>
        /// <returns>the mail message to be sent</returns>
        internal static MailMessage CreateHtmlMailMessage(string toEmail, string subject, string htmlBody, bool extractAndAddText = false)
        {
            MailMessage mail = null;

            #region Input Validation

            if (string.IsNullOrWhiteSpace(toEmail))//|| string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(htmlBody))
            {
                return mail;
            }

            Regex regex = new Regex(REGEX_EMAIL);
            if (!regex.IsMatch(toEmail))  // || !regex.IsMatch(LAUNCHPAD_SENDER_EMAIL)
            {
                return mail;
            }

            #endregion

            if (extractAndAddText)
            {
                string textBody = EmailTemplateResolver.GetTextEmailBodyFromHtmlString(htmlBody);
                mail = CreateMailMessageWithoutDestination(subject, htmlBody, textBody);
            }
            else
            {
                mail = CreateMailMessageWithoutDestination(subject, htmlBody, null);
            }

            if (mail != null)
            {
                mail.To.Add(toEmail);
            }
            return mail;
        }

        internal static MailMessage CreateHtmlMailMessage(List<string> toEmails, string subject, string htmlBody, bool extractAndAddText = false)
        {
            MailMessage mail = null;

            #region Input Validation

            if (toEmails == null || toEmails.Count == 0)// || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(htmlBody))
            {
                return mail;
            }

            Regex regex = new Regex(REGEX_EMAIL);
            for (int i = 0; i < toEmails.Count; i++)
            {
                if (!regex.IsMatch(toEmails[i]))
                {
                    return mail;
                }
            }

            //if (!regex.IsMatch(LAUNCHPAD_SENDER_EMAIL)) // check the list of emails
            //{
            //    return mail;
            //}

            #endregion

            if (extractAndAddText)
            {
                string textBody = EmailTemplateResolver.GetTextEmailBodyFromHtmlString(htmlBody);
                mail = CreateMailMessageWithoutDestination(subject, htmlBody, textBody);
            }
            else
            {
                mail = CreateMailMessageWithoutDestination(subject, htmlBody, null);
            }

            if (mail != null)
            {
                foreach (string email in toEmails)
                {
                    if (regex.IsMatch(email))
                    {
                        mail.Bcc.Add(email);
                    }
                }
                if (mail.Bcc.Count == 0)
                {
                    return null;
                }
            }
            return mail;
        }

        /// <summary>
        /// Creates a mail message wihtout destinatin. The subject needs to be set. One of the 2 bodies needs to be set otherwise the method
        /// will return null
        /// </summary>
        /// <param name="subject">subject of the message</param>
        /// <param name="htmlBody">the html body</param>
        /// <param name="textBody">the text body</param>
        /// <returns>mail message</returns>
        internal static MailMessage CreateMailMessageWithoutDestination(string subject, string htmlBody, string textBody)
        {
            MailMessage mail = null;

            #region Input Validation

            if (string.IsNullOrWhiteSpace(subject) || (string.IsNullOrWhiteSpace(htmlBody) && string.IsNullOrWhiteSpace(textBody)))
            {
                return mail;
            }

            Regex regex = new Regex(REGEX_EMAIL);
            if (!regex.IsMatch(LAUNCHPAD_SENDER_EMAIL))
            {
                return mail;
            }

            #endregion
            mail = new MailMessage();
            mail.From = new MailAddress(LAUNCHPAD_SENDER_EMAIL);
            mail.Subject = subject;

            if (!string.IsNullOrWhiteSpace(textBody))
            {
                AlternateView textView = AlternateView.CreateAlternateViewFromString(textBody);
                textView.ContentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Plain);// text/plain
                mail.AlternateViews.Add(textView);
            }

            if (!string.IsNullOrWhiteSpace(htmlBody))
            {
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody);
                htmlView.ContentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Html);
                mail.AlternateViews.Add(htmlView);
            }

            return mail;
        }
    }
}
