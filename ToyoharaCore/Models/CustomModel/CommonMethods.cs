using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToyoharaCore.Models.CustomModel
{
    public class CommonMethods
    {
        public static string ObjectToString(object obj)
        {
            if (obj == null) return "";
            return obj.ToString();
        }

        public static string HtmlToText(string htmlString)
        {
            if (htmlString == null) return String.Empty;
            char[] charArr = htmlString.ToCharArray();
            char[] result = { };
            Array.Resize(ref result, charArr.Length);
            bool CopyFlag = true;
            int j = 0;
            for (int i = 0; i < charArr.Length; i++)
            {
                if (charArr[i] == '<')
                {
                    CopyFlag = false;
                }
                if (CopyFlag) { result[j] = charArr[i]; j++; }
                if (charArr[i] == '>')
                {
                    CopyFlag = true;
                }
            }
            Array.Resize(ref result, j);

            return new string(result);
        }
    }
}
