using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaiduBce.Auth
{
    /// <summary>
    /// Default implementation of the Credentials interface that allows callers to pass in the BCE access key and secret
    /// access in the constructor.
    /// </summary>
    public class DefaultBceCredentials : IBceCredentials
    {

        /// <summary>
        /// The BCE access key ID.
        /// </summary>
        public string AccessKeyId { get; private set; }

        /// <summary>
        /// The BCE secret access key.
        /// </summary>
        public string SecretKey { get; private set; }

        /// <summary>
        /// Constructs a new Credentials object, with the specified access key id and secret key.
        /// </summary>
        /// <param name="accessKeyId"> the BCE access key id. </param>
        /// <param name="secretKey">   the BCE secret access key.
        /// </param>
        /// <exception cref="System.ArgumentNullException"> The accessKeyId, secretKey should not be null or empty. </exception>
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