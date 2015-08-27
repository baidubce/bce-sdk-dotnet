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
using System.IO;
using System.Net;

using BaiduBce.Auth;
using BaiduBce.Internal;
using BaiduBce.Model;
using BaiduBce.Util;


namespace BaiduBce.Http
{
    internal class BceHttpClient
    {

        public T Execute<T>(InternalRequest request)
        {
            HttpWebResponse httpWebResponse = Execute(request);
            using (httpWebResponse)
            {
                var content = httpWebResponse.GetResponseStream();
                if (content != null)
                {
                    return JsonUtils.ToObject<T>(new StreamReader(content));
                }
            }
            return default(T);
        }

        private HttpWebResponse Execute(InternalRequest request)
        {
            BceClientConfiguration config = request.Config;
            if (config.Credentials != null)
            {
                request.Headers[BceConstants.HttpHeaders.Authorization] = config.Signer.sign(request);
            }
            HttpWebRequest httpWebRequest = BceHttpClient.CreateHttpWebRequest(request);
            BceHttpClient.PopulateRequestHeaders(request, httpWebRequest);
            if (request.Content != null)
            {
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                using (Stream contentStream = request.Content)
                {
                    var buffer = new byte[(int)config.SocketBufferSizeInBytes];
                    int bytesRead = 0;
                    while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            try
            {
                return httpWebRequest.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.ProtocolError)
                {
                    throw e;
                }
                using (var response = e.Response as HttpWebResponse)
                {
                    throw BceServiceException.CreateFromHttpWebResponse(response);
                }
            }
        }

        private static HttpWebRequest CreateHttpWebRequest(InternalRequest request)
        {
            BceClientConfiguration config = request.Config;
            IBceCredentials credentials = config.Credentials;
            string uri = request.Uri.ToString();
            string encodedParams = HttpUtils.GetCanonicalQueryString(request.Parameters, false);
            if (encodedParams.Length > 0)
            {
                uri += "?" + encodedParams;
            }
            var httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
            httpWebRequest.Timeout = config.TimeoutInMillis ?? BceClientConfiguration.DefaultTimeoutInMillis;
            httpWebRequest.ReadWriteTimeout =
                config.ReadWriteTimeoutInMillis ?? BceClientConfiguration.DefaultReadWriteTimeoutInMillis;
            if (!string.IsNullOrEmpty(config.ProxyHost) && config.ProxyPort.GetValueOrDefault() > 0)
            {
                WebProxy proxy = new WebProxy(config.ProxyHost, config.ProxyPort.GetValueOrDefault());
                if (config.ProxyCredentials != null)
                {
                    proxy.Credentials = config.ProxyCredentials;
                }
                httpWebRequest.Proxy = proxy;
            }
            if (config.UseNagleAlgorithm != null)
            {
                httpWebRequest.ServicePoint.UseNagleAlgorithm = (bool)config.UseNagleAlgorithm;
            }
            httpWebRequest.ServicePoint.MaxIdleTime =
                config.MaxIdleTimeInMillis ?? BceClientConfiguration.DefaultMaxIdleTimeInMillis;
            httpWebRequest.ServicePoint.ConnectionLimit =
                config.ConnectionLimit ?? BceClientConfiguration.DefaultConnectionLimit;
            httpWebRequest.ServicePoint.Expect100Continue = request.Expect100Continue;
            httpWebRequest.Method = request.HttpMethod;
            return httpWebRequest;
        }

        private static void PopulateRequestHeaders(InternalRequest request, HttpWebRequest httpWebRequest)
        {
            httpWebRequest.UserAgent = request.Config.UserAgent;
            foreach (KeyValuePair<string, string> entry in request.Headers)
            {
                string key = entry.Key;
                if (key.Equals(BceConstants.HttpHeaders.ContentLength, StringComparison.CurrentCultureIgnoreCase))
                {
                    httpWebRequest.ContentLength = Convert.ToInt64(entry.Value);
                }
                else if (key.Equals(BceConstants.HttpHeaders.ContentType, StringComparison.CurrentCultureIgnoreCase))
                {
                    httpWebRequest.ContentType = entry.Value;
                }
                // can't direct set Host in httpwebrequest
                else if (!key.Equals(BceConstants.HttpHeaders.Host, StringComparison.CurrentCultureIgnoreCase))
                {
                    httpWebRequest.Headers[key] = entry.Value;
                }
            }
        }
    }
}
