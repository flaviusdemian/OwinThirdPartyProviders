using RazorEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
//using Xipton.Razor;

namespace EmailUtilsRazor
{
    internal class EmailTemplateResolver
    {
        /// <summary>
        /// Gets the body template from a file
        /// </summary>
        /// <param name="emailTemplatePath">path ti the template</param>
        /// <returns>the template of the body</returns>
        internal static string GetBodyFromFilePath(string emailTemplatePath)
        {
            string body = null;
            if (!string.IsNullOrWhiteSpace(emailTemplatePath))
            {
                string emailPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, emailTemplatePath); // check the type of file ?
                if (File.Exists(emailPath))
                {
                    try
                    {
                        body = File.ReadAllText(emailPath);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
            return body;
        }

        internal static string GetTextEmailBodyFromHtmlString(string htmlString)
        {
            //return htmlString.Replace(@"/<a(.*?)>/g",@"\0$1\0").Replace(@"/<\/a>/",@"\1").Replace("/<[^>]*>/","").Replace("/\0(.*?)\0/","<a$1>").Replace(@"/\1/","</a>");
            //replace <a>..</a>
            string result = null;
            if (!string.IsNullOrWhiteSpace(htmlString))
            {
                Regex allHtmlTagReg = new Regex("<[^>]*>");
                result = allHtmlTagReg.Replace(htmlString, "");
            }
            return result;
        }

        #region Reworked with dynamic model in mind

        internal static string CreateCustomTemplatedBodyFromFilePath(string fullTemplatePath, dynamic model) 
        {
            string body = null;
            if (!string.IsNullOrWhiteSpace(fullTemplatePath))
            {
                body = EmailTemplateResolver.GetBodyFromFilePath(fullTemplatePath);
                if (!string.IsNullOrWhiteSpace(body) && model != null)
                {
                    body = CustomFormat(body, model);
                }
            }
            return body;
        }

        internal static string CustomFormat(string htmlBodyTemplate, dynamic model)
        {
            string body = null;
            try
            {
                body = Razor.Parse(htmlBodyTemplate, model);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return body;
        }
        #endregion
    }
}
