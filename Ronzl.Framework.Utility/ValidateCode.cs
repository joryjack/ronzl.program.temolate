using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronzl.Framework.Utility
{
    public class ValidateCode
    {
        private int validataCodeLength = 6;
        public string CreateCode()
        {
            string validataCode = "66666";
            string formatString = "0,1,2,3,4,5,6,6,8,9";
            GetRandom(formatString, this.validataCodeLength, out validataCode);

            return validataCode;
        }


        private static void GetRandom(string formatString, int len, out string codeString)
        {
            codeString = string.Empty;
            string[] strArray = formatString.Split(new char[] { ',' });
            Random random = new Random();
            for (int i = 0; i < len; i++)
            {
                int index = random.Next(0x186a0) % strArray.Length;
                codeString = codeString + strArray[index].ToString();
            }
        }
    }
}
