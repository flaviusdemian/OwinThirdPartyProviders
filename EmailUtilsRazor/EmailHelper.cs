using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailUtilsRazor
{
    public class EmailHelper
    {
        /// <summary>
        /// Send a single email 
        /// </summary>
        /// <param name="toEmail">receiver</param>
        /// <param name="subject">subject</param>
        /// <param name="body">content</param>
        /// <param name="isHtml">if it is a html content or not</param>
        /// <returns>the sucess status of the operation</returns>
        public static bool SendEmailToAddress(string toEmail, string subject, string body, bool isHtml)
        {
            bool result = false;
            #region Input Validation

            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
            {
                return false;
            }

            Regex regex = new Regex(EmailGenerator.REGEX_EMAIL);
            if (!regex.IsMatch(EmailGenerator.LAUNCHPAD_SENDER_EMAIL) || !regex.IsMatch(toEmail))
            {
                return false;
            }

            #endregion

            try
            {
                MailMessage mail = null;
                if (isHtml)
                {
                    mail = EmailGenerator.CreateMailMessageWithoutDestination(subject, body, null);
                }
                else
                {
                    mail = EmailGenerator.CreateMailMessageWithoutDestination(subject, null, body);
                }

                if (mail != null)
                {
                    //smtpClient.Send(mail);
                    mail.To.Add(toEmail);
                    SmtpClient smtpClient = EmailGenerator.CreateSmtpClient();
                    if (smtpClient != null)
                    {
                        smtpClient.Send(mail);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return result;
        }


        /// <summary>
        /// This method sends an Html mail to a user. If you coose to generate the text view from the html body that text will also be included in the mail. 
        /// The email will then be a multipart email, containing both Html and Text views.
        /// </summary>
        /// <param name="toEmail">the email to which to send</param>
        /// <param name="subject">the subject of the email</param>
        /// <param name="htmlBody">the body of the email</param>
        /// <param name="extractAndAddText">choose wheter you want to extract the text from the html and include that text into the mail</param>
        /// <returns>the sucess status of the operation</returns>
        public static bool SendHtmlEmailToAddress(string toEmail, string subject, string htmlBody, bool extractAndAddText = false)
        {
            #region Input Validation

            bool result = false;
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(htmlBody))
            {
                return false;
            }

            //Regex regex = new Regex(REGEX_EMAIL);
            //if (!regex.IsMatch(LAUNCHPAD_SENDER_EMAIL) || !regex.IsMatch(toEmail))
            //{
            //    return false;
            //}

            #endregion
            try
            {
                MailMessage mail = EmailGenerator.CreateHtmlMailMessage(toEmail, subject, htmlBody, extractAndAddText);

                if (mail != null)
                {
                    SmtpClient smtpClient = EmailGenerator.CreateSmtpClient();
                    if (smtpClient != null)
                    {
                        smtpClient.Send(mail);
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// Sends an email from a path
        /// </summary>
        /// <param name="toEmail">receiver</param>
        /// <param name="subject">the subject of the email</param>
        /// <param name="htmlBodyPath">path to the body</param>
        /// <param name="extractAndAddText">choose wheter you want to extract the text from the html and include that text into the mail</param>
        /// <returns>the sucess status of the operation</returns>
        public static bool SendHtmlEmailFromPath(string toEmail, string subject, string htmlBodyPath, bool extractAndAddText = false)
        {
            bool result = false;
            #region Input validation
            if (string.IsNullOrWhiteSpace(toEmail) ||
                string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(htmlBodyPath))
            {
                return result;
            }
            #endregion

            string bodyHtml = null;
            bodyHtml = EmailTemplateResolver.GetBodyFromFilePath(htmlBodyPath);
            result = SendHtmlEmailToAddress(toEmail, subject, bodyHtml, extractAndAddText);

            return result;
        }

        /// <summary>
        /// Sends an email by using a path that indicates to the body 
        /// </summary>
        /// <param name="toEmail">receiver</param>
        /// <param name="subject">the subject of the email</param>
        /// <param name="htmlBodyTemplatePath">path of the html template</param>
        /// <param name="extractAndAddText">choose wheter you want to extract the text from the html and include that text into the mail</param>
        /// <returns>the sucess status of the operation</param>
        /// <param name="model">the model that holds the values to be inserted in the email template</param>
        /// <returns>true if the process was successful</returns>
        public static bool SendHtmlTemplatedEmailFromPath(string toEmail, string subject, string htmlBodyTemplatePath, bool extractAndAddText, dynamic model)
        {
            bool result = false;
            #region Input validation
            if (string.IsNullOrWhiteSpace(toEmail) ||
                string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(htmlBodyTemplatePath))
            {
                return result;
            }
            #endregion

            string bodyHtml = null;
            bodyHtml = EmailTemplateResolver.CreateCustomTemplatedBodyFromFilePath(htmlBodyTemplatePath, model);
            result = SendHtmlEmailToAddress(toEmail, subject, bodyHtml, extractAndAddText);

            return result;
        }
    }
}
