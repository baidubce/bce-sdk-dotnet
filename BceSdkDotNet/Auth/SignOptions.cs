using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaiduBce.Auth
{
    /// <summary>
    /// Options for signing the request.
    /// 
    /// <para>
    /// There are 3 options available:
    /// <table>
    /// <tr>
    /// <th>Option</th>
    /// <th>Description</th>
    /// </tr>
    /// <tr>
    /// <td>headersToSign</td>
    /// <td>The set of headers to be signed. If this option is not set or set to null, only the following headers are signed
    /// <ul>
    /// <li>Host</li>
    /// <li>Content-Length</li>
    /// <li>Content-Type</li>
    /// <li>Content-MD5</li>
    /// <li>All headers starts with "x-bce-"</li>
    /// </ul>
    /// </td>
    /// </tr>
    /// <tr>
    /// <td>timestamp</td>
    /// <td>The time when the signature was created. If this option is not set or set to null, the signer will use the time
    /// when the sign method is invoked.</td>
    /// </tr>
    /// <tr>
    /// <td>expirationInSeconds</td>
    /// <td>The time until the signature will expire, which starts from the timestamp. By default, it is set to 1800 (half an
    /// hour). </td>
    /// </tr>
    /// *
    /// </table>
    /// </para>
    /// </summary>
    public class SignOptions
    {
        public const int DefaultExpirationInSeconds = 1800;

        /// <summary>
        /// The set of headers to be signed.
        /// </summary>
        public HashSet<string> HeadersToSign { get; set; }

        /// <summary>
        /// The time when the signature was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The time until the signature will expire.
        /// </summary>
        public int ExpirationInSeconds { get; set; }

        /// <summary>
        /// The default sign options, which is {headersToSign:null, timestamp:null, expirationInSeconds:1800}.
        /// </summary>
        public SignOptions()
        {
            ExpirationInSeconds = DefaultExpirationInSeconds;
        }
    }
}