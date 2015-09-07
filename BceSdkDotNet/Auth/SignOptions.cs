using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaiduBce.Auth
{
    public class SignOptions
    {
        public const int DefaultExpirationInSeconds = 1800;

        public HashSet<string> HeadersToSign { get; set; }

        public DateTime Timestamp { get; set; }

        public int ExpirationInSeconds { get; set; }

        public SignOptions()
        {
            ExpirationInSeconds = DefaultExpirationInSeconds;
        }
    }
}