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
using System.Reflection;
using System.Threading;
using BaiduBce.Auth;
using BaiduBce.Internal;
using BaiduBce.Model;
using BaiduBce.Util;
using log4net;


namespace BaiduBce.Http
{
    internal class BceHttpClient
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BceHttpClient));

        public HttpWebResponse Execute(InternalRequest request)
        {
            BceClientConfiguration config = request.Config;
            if (config.Credentials != null)
            {
                request.Headers[BceConstants.HttpHeaders.Authorization] = config.Signer.Sign(request);
            }
            HttpWebRequest httpWebRequest = CreateHttpWebRequest(request);
            PopulateRequestHeaders(request, httpWebRequest);
            if (request.Content != null)
            {
                httpWebRequest.AllowWriteStreamBuffering = false;
                using (Stream requestStream = WebRequestExtension.GetRequestStreamWithTimeout(httpWebRequest))
                {
                    var buffer = new byte[(int) config.SocketBufferSizeInBytes];
                    int bytesRead = 0;
                    int totalBytesRead = 0;
                    long contentLength = GetContentLengthFromInternalRequest(request);
                    try
                    {
                        while ((bytesRead = request.Content.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (contentLength > 0 && (bytesRead + totalBytesRead) >= contentLength)
                            {
                                requestStream.Write(buffer, 0, (int) (contentLength - totalBytesRead));
                                break;
                            }
                            else
                            {
                                requestStream.Write(buffer, 0, bytesRead);
                                totalBytesRead += bytesRead;
                            }
                        }
                    }
                    catch (NotSupportedException e)
                    {
                        //a very strange phenomenon:
                        //if bos server is down, we will receive a NotSupportedException when execute requestStream.Write(...)
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("error when put data.", e);
                        }
                    }
                    if (request.Content.CanSeek)
                    {
                        request.Content.Seek(0, SeekOrigin.Begin);
                    }
                }

            }
            try
            {
                return WebRequestExtension.GetResponseWithTimeout(httpWebRequest) as HttpWebResponse;
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
            string uri = request.Uri.AbsoluteUri;
            string encodedParams = HttpUtils.GetCanonicalQueryString(request.Parameters, false);
            if (encodedParams.Length > 0)
            {
                uri += "?" + encodedParams;
            }
            var httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
            httpWebRequest.Timeout = config.TimeoutInMillis ?? BceClientConfiguration.DefaultTimeoutInMillis;
            httpWebRequest.ReadWriteTimeout =
                config.ReadWriteTimeoutInMillis ?? BceClientConfiguration.DefaultReadWriteTimeoutInMillis;
            if (request.Range != null && request.Range.Length == 2)
            {
                AddRange(httpWebRequest, request.Range);
            }
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
                httpWebRequest.ServicePoint.UseNagleAlgorithm = (bool) config.UseNagleAlgorithm;
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

        private static long GetContentLengthFromInternalRequest(InternalRequest request)
        {
            string contentLengthString;
            if (request.Headers.TryGetValue(BceConstants.HttpHeaders.ContentLength, out contentLengthString))
            {
                long contentLength;
                if (long.TryParse(contentLengthString, out contentLength))
                {
                    return contentLength;
                }
            }
            return -1;
        }

        /// <summary>
        /// defaulst addRange method only take int32 args, can not work if a file is bigger than 2G
        /// </summary>
        /// <param name="httpWebRequest"></param>
        /// <param name="range"></param>
        private static void AddRange(HttpWebRequest httpWebRequest, long[] range)
        {
            Type t = typeof (HttpWebRequest);
            object[] args = new object[3];
            args[0] = "bytes";
            args[1] = range[0].ToString();
            args[2] = range[1].ToString();
            MethodInfo[] mi = t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var methodInfo in mi)
            {
                if (methodInfo.Name == "AddRange")
                {
                    methodInfo.Invoke(httpWebRequest, args);
                }
            }
        }
    }
}