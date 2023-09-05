using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public static class Helpers
    {
        public static string MakeString(this string input)
        {
            return input.Replace(",", "").Replace("$", "").Replace(".", ",").Replace("-0", "-").Replace("+0", "+");
        }
    }
}
