using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BaiduBce.Http;

namespace BaiduBce.Util
{
    internal static class HttpUtils
    {
        private static readonly string[] PercentEncodedStrings =
            Enumerable.Range(0, 256).Select(v => "%" + v.ToString("X2")).ToArray();

        static HttpUtils()
        {
            for (char i = 'a'; i <= 'z'; i++)
            {
                PercentEncodedStrings[i] = i.ToString();
            }
            for (char i = 'A'; i <= 'Z'; i++)
            {
                PercentEncodedStrings[i] = i.ToString();
            }
            for (char i = '0'; i <= '9'; i++)
            {
                PercentEncodedStrings[i] = i.ToString();
            }
            PercentEncodedStrings['-'] = "-";
            PercentEncodedStrings['.'] = ".";
            PercentEncodedStrings['_'] = "_";
            PercentEncodedStrings['~'] = "~";
        }

        public static string NormalizePath(string path)
        {
            return HttpUtils.Normalize(path).Replace("%2F", "/");
        }

        public static string Normalize(string value)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in Encoding.UTF8.GetBytes(value))
            {
                builder.Append(PercentEncodedStrings[b & 0xFF]);
            }
            return builder.ToString();
        }

        public static string GenerateHostHeader(Uri uri)
        {
            string host = uri.Host;
            if (HttpUtils.IsUsingNonDefaultPort(uri))
            {
                host += ":" + uri.Port;
            }
            return host;
        }

        public static bool IsUsingNonDefaultPort(Uri uri)
        {
            string scheme = uri.Scheme.ToLower();
            int port = uri.Port;
            if (port <= 0)
            {
                return false;
            }
            if (scheme == BceConstants.Protocol.Http)
            {
                return port != BceConstants.Protocol.HttpDefaultPort;
            }
            if (scheme == BceConstants.Protocol.Https)
            {
                return port != BceConstants.Protocol.HttpsDefaultPort;
            }
            return false;
        }

        public static string GetCanonicalQueryString(IDictionary<string, string> parameters, bool forSignature)
        {
            if (parameters.Count == 0)
            {
                return "";
            }

            List<string> parameterStrings = new List<string>();
            foreach (KeyValuePair<string, string> entry in parameters)
            {
                string key = entry.Key;
                if (forSignature &&
                    BceConstants.HttpHeaders.Authorization.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (key == null)
                {
                    throw new ArgumentNullException("parameter key should NOT be null");
                }
                string value = entry.Value;
                if (value == null)
                {
                    if (forSignature)
                    {
                        parameterStrings.Add(HttpUtils.Normalize(key) + '=');
                    }
                    else
                    {
                        parameterStrings.Add(HttpUtils.Normalize(key));
                    }
                }
                else
                {
                    parameterStrings.Add(HttpUtils.Normalize(key) + '=' + HttpUtils.Normalize(value));
                }
            }
            parameterStrings.Sort();
            return string.Join("&", parameterStrings.ToArray());
        }

        public static string AppendUri(string baseUri, params string[] pathComponents)
        {
            if (pathComponents.Length == 0)
            {
                return baseUri;
            }
            StringBuilder builder = new StringBuilder(baseUri.ToString().TrimEnd('/'));
            for (int i = 0; i < pathComponents.Length; ++i)
            {
                string path = pathComponents[i];
                if (i < pathComponents.Length - 1)
                {
                    path = path.TrimEnd('/');
                }
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.StartsWith("/"))
                    {
                        builder.Append('/');
                    }
                    builder.Append(HttpUtils.NormalizePath(path));
                }
            }
            return builder.ToString();
        }
    }
}
