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
    public class DefaultBceSessionCredentials : DefaultBceCredentials, IBceSessionCredentials
    {

        /// <summary>
        /// The BCE session token.
        /// </summary>
        public string SessionToken { get; private set; }

        /// <summary>
        /// Constructs a new SessionCredentials object, with the specified access key id, secret key and session token.
        /// </summary>
        /// <param name="accessKeyId"> the BCE access key id. </param>
        /// <param name="secretKey"> the BCE secret access key. </param>
        /// <param name="sessionToken"> the BCE session token. </param>
        /// <exception cref="System.ArgumentNullException">
        /// The accessKeyId, secretKey or sessionToken should not be null or empty.
        /// </exception>
        public DefaultBceSessionCredentials(string accessKeyId, string secretKey, string sessionToken)
            : base(accessKeyId, secretKey)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                throw new ArgumentNullException("sessionToken should NOT be null or empty.");
            }
            this.SessionToken = sessionToken;
        }
    }
}