using System;
using System.Text;

namespace OwinOAuthProvidersDemo
{
    public class RandomStringGenerator
    {
        public static string RandomString(int size)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}