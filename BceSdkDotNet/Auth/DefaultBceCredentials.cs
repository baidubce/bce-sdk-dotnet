using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaiduBce.Auth
{
    public class DefaultBceCredentials : IBceCredentials
    {
        public string AccessKeyId { get; private set; }
        public string SecretKey { get; private set; }

        public DefaultBceCredentials(string accessKeyId, string secretKey)
        {
            if (string.IsNullOrEmpty(accessKeyId))
            {
                throw new ArgumentNullException("accessKeyId should NOT be null or empty.");
            }
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException("secretKey should NOT be null or empty.");
            }
            this.AccessKeyId = accessKeyId;
            this.SecretKey = secretKey;
        }
    }
}