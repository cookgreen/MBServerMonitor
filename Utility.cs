using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBServerMonitor
{
    public class Utility
    {
        public static string FilterSpecialChars(string str)
        {
            str = str.Contains("\'") ? str.Replace("\'", "\'\'") : str;

            return str;
        }
    }
}
