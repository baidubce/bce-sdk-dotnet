// Copyright (c) 2014 Baidu.com, Inc. All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
// an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using log4net;
using BaiduBce.Internal;
using BaiduBce.Http;
using BaiduBce.Util;
using System.Text;
using System.Web;

namespace BaiduBce.Auth
{
    public class BceV1Signer : ISigner
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (BceV1Signer));

        private const string BceAuthVersion = "bce-auth-v1";

        private const string DefaultEncoding = "UTF-8";
        private static readonly HashSet<string> defaultHeadersToSign = new HashSet<string>();

        static BceV1Signer()
        {
            defaultHeadersToSign.Add(BceConstants.HttpHeaders.Host.ToLower());
            defaultHeadersToSign.Add(BceConstants.HttpHeaders.ContentLength.ToLower());
            defaultHeadersToSign.Add(BceConstants.HttpHeaders.ContentType.ToLower());
            defaultHeadersToSign.Add(BceConstants.HttpHeaders.ContentMd5.ToLower());
        }

        public string Sign(InternalRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request should NOT be null.");
            }

            var config = request.Config;
            if (config == null)
            {
                throw new ArgumentNullException("request.Config should NOT be null.");
            }

            var credentials = config.Credentials;
            if (credentials == null)
            {
                throw new ArgumentNullException("request.Config.Credentials should NOT be null.");
            }

            var options = request.Config.SignOptions;
            if (options == null)
            {
                options = new SignOptions();
            }

            string accessKeyId = credentials.AccessKeyId;
            string secretAccessKey = credentials.SecretKey;

            var timestamp = options.Timestamp;
            if (timestamp == DateTime.MinValue)
            {
                timestamp = DateTime.Now;
            }

            string authString =
                BceAuthVersion + "/" + accessKeyId + "/"
                + DateUtils.FormatAlternateIso8601Date(timestamp) + "/" + options.ExpirationInSeconds;

            string signingKey = BceV1Signer.Sha256Hex(secretAccessKey, authString);
            // Formatting the URL with signing protocol.
            string canonicalURI = BceV1Signer.GetCanonicalURIPath(HttpUtility.UrlDecode(request.Uri.AbsolutePath));
            // Formatting the query string with signing protocol.
            string canonicalQueryString = HttpUtils.GetCanonicalQueryString(request.Parameters, true);
            // Sorted the headers should be signed from the request.
            SortedDictionary<string, string> headersToSign =
                BceV1Signer.GetHeadersToSign(request.Headers, options.HeadersToSign);
            // Formatting the headers from the request based on signing protocol.
            string canonicalHeader = BceV1Signer.GetCanonicalHeaders(headersToSign);
            string signedHeaders = "";
            if (options.HeadersToSign != null)
            {
                signedHeaders = string.Join(";", headersToSign.Keys.ToArray()).Trim().ToLower();
            }

            string canonicalRequest =
                request.HttpMethod + "\n" + canonicalURI + "\n" + canonicalQueryString + "\n" + canonicalHeader;

            // Signing the canonical request using key with sha-256 algorithm.
            string signature = BceV1Signer.Sha256Hex(signingKey, canonicalRequest);

            string authorizationHeader = authString + "/" + signedHeaders + "/" + signature;

            log.Debug(string.Format(
                "CanonicalRequest:{0}\tAuthorization:{1}",
                canonicalRequest.Replace("\n", "[\\n]"),
                authorizationHeader));

            return authorizationHeader;
        }

        private static string GetCanonicalURIPath(String path)
        {
            if (path == null)
            {
                return "/";
            }
            else if (path.StartsWith("/"))
            {
                return HttpUtils.NormalizePath(path);
            }
            else
            {
                return "/" + HttpUtils.NormalizePath(path);
            }
        }

        private static string GetCanonicalHeaders(SortedDictionary<string, string> headers)
        {
            if (headers.Count == 0)
            {
                return "";
            }

            List<string> headerStrings = new List<string>();
            foreach (KeyValuePair<string, string> entry in headers)
            {
                string key = entry.Key;
                if (key == null)
                {
                    continue;
                }
                string value = entry.Value;
                if (value == null)
                {
                    value = "";
                }
                headerStrings.Add(HttpUtils.Normalize(key.Trim().ToLower()) + ':' + HttpUtils.Normalize(value.Trim()));
            }
            headerStrings.Sort();

            return string.Join("\n", headerStrings.ToArray());
        }

        private static SortedDictionary<string, string> GetHeadersToSign(
            IDictionary<string, string> headers, HashSet<string> headersToSign)
        {
            var ret = new SortedDictionary<string, string>();
            if (headersToSign != null)
            {
                var tempSet = new HashSet<string>();
                foreach (string header in headersToSign)
                {
                    tempSet.Add(header.Trim().ToLower());
                }
                headersToSign = tempSet;
            }
            foreach (KeyValuePair<string, string> entry in headers)
            {
                string key = entry.Key;
                if (!string.IsNullOrEmpty(entry.Value))
                {
                    if ((headersToSign == null && BceV1Signer.IsDefaultHeaderToSign(key))
                        || (headersToSign != null
                            && headersToSign.Contains(key.ToLower())
                            && !BceConstants.HttpHeaders.Authorization.Equals(key, StringComparison.OrdinalIgnoreCase)))
                    {
                        ret.Add(key, entry.Value);
                    }
                }
            }
            return ret;
        }

        private static bool IsDefaultHeaderToSign(string header)
        {
            header = header.Trim().ToLower();
            return header.StartsWith(BceConstants.HttpHeaders.BcePrefix) ||
                   BceV1Signer.defaultHeadersToSign.Contains(header);
        }

        private static string Sha256Hex(string signingKey, string stringToSign)
        {
            try
            {
                var hash = new HMACSHA256(Encoding.UTF8.GetBytes(signingKey));
                return EncodeHex(hash.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            }
            catch (Exception e)
            {
                throw new BceClientException("Fail to generate the signature", e);
            }
        }

        public static string[] HexTable = Enumerable.Range(0, 256).Select(v => v.ToString("x2")).ToArray();

        private static string EncodeHex(byte[] data)
        {
            var sb = new StringBuilder();
            foreach (var b in data)
            {
                sb.Append(HexTable[b]);
            }
            return sb.ToString();
        }
    }
}