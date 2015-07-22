using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaiduBce.Util
{
    internal static class DateUtils
    {
        public static string formatAlternateIso8601Date(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK");
        }
    }
}
