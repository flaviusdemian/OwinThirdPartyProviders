using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Security.Providers
{
    public class FileReaderBusinessLogic
    {
        public static string[] ReadFiles(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return null;
        }
    }
}
