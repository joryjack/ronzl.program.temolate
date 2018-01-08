using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronzl.Framework.Contract
{
    public class ValidateCode
    {
        public static string CreateCode()
        {

            string formatString = "0,1,2,3,4,5,6,7,8,9";
            return GetRandom(formatString, 6);
        }

        private static string GetRandom(string formatString, int len)
        {
            string codeString = string.Empty;
            string[] strArray = formatString.Split(new char[] { ',' });
            Random random = new Random();
            for (int i = 0; i < len; i++)
            {
                int index = random.Next(0x186a0) % strArray.Length;
                codeString = codeString + strArray[index].ToString();
            }
            return codeString;
        }





    }
}
