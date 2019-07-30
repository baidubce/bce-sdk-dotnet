// Copyright 2014 Baidu, Inc.
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
using System.IO;
using System.Net;
using System.Text;
using BaiduBce.Http;
using BaiduBce.Internal;
using BaiduBce.Model;
using BaiduBce.Util;

namespace BaiduBce
{

    /// <summary>
    /// Abstract base class for BCE service clients.
    /// 
    /// <para>
    /// Responsible for basic client capabilities that are the same across all BCE SDK Java clients
    /// (ex: setting the client endpoint).
    /// 
    /// </para>
    /// <para>
    /// Subclass names should be in the form of "com.baidubce.services.xxx.XxxClient", while "xxx" is the service ID and
    /// "Xxx" is the capitalized service ID.
    /// </para>
    /// </summary>
    public abstract class BceClientBase
    {
        /// <summary>
        /// The default service domain format for BCE.
        /// </summary>
        private string serviceEndpointFormat;

        /// <summary>
        /// The client configuration for this client.
        /// </summary>
        protected BceClientConfiguration config;

        /// <summary>
        /// Responsible for sending HTTP requests to services.
        /// </summary>
        internal BceHttpClient httpClient;

        /// <summary>
        /// Constructs a new AbstractBceClient with the specified client configuration.
        /// 
        /// <para>
        /// The constructor will extract serviceId from the class name automatically.
        /// And if there is no endpoint specified in the client configuration, the constructor will create a default one.
        /// 
        /// </para>
        /// </summary>
        /// <param name="config"> the client configuration. The constructor makes a copy of this parameter so that it is
        ///     safe to change the configuration after then. </param>
        /// <param name="serviceEndpointFormat"> the service domain name format. </param>
        public BceClientBase(BceClientConfiguration config, string serviceEndpointFormat)
        {
            this.serviceEndpointFormat = serviceEndpointFormat;
            this.config = BceClientConfiguration.CreateWithDefaultValues().Merge(config);
            this.httpClient = new BceHttpClient();
        }

        /// <summary>
        /// Returns the default target service endpoint.
        /// 
        /// </summary>
        /// <returns> the computed service endpoint </returns>
        /// <exception cref="FormatException"> if the endpoint specified in the client configuration is not a valid URI. </exception>
        public string ComputeEndpoint(BceClientConfiguration config)
        {
            if (config.Endpoint != null)
            {
                return config.Endpoint;
            }
            string protocol = config.Protocol ?? BceConstants.Protocol.Http;
            string region = config.Region ?? BceConstants.Region.Beijing;
            return string.Format(this.serviceEndpointFormat, protocol, region);
        }

        protected InternalRequest CreateInternalRequest(
            BceRequestBase request, string httpMethod, string[] pathComponents)
        {
            var internalRequest = new InternalRequest();
            internalRequest.Config = this.config.Merge(request.Config);
            if (request.Credentials != null)
            {
                internalRequest.Config.Credentials = request.Credentials;
            }
            internalRequest.Uri = new Uri(
                HttpUtils.AppendUri(this.ComputeEndpoint(internalRequest.Config), pathComponents));
            internalRequest.HttpMethod = httpMethod;
            var timestamp = internalRequest.Config.SignOptions.Timestamp;
            if (timestamp == DateTime.MinValue)
            {
                timestamp = DateTime.Now;
            }
            internalRequest.Headers[BceConstants.HttpHeaders.BceDate] = DateUtils.FormatAlternateIso8601Date(timestamp);
            internalRequest.Headers[BceConstants.HttpHeaders.Host] = HttpUtils.GenerateHostHeader(internalRequest.Uri);
            return internalRequest;
        }

        /// <summary>
        /// Converts the json string into UTF-8 string and set it as the content of internalRequest, which will be
        /// used as the http request body. Content-Length and Content-Type ard set accordingly.
        /// </summary>
        /// <param name="internalRequest">The request used to build the http request</param>
        /// <param name="json">The json string to be set as content</param>
        protected void FillRequestBodyForJson(InternalRequest internalRequest, string json)
        {
            byte[] jsonUTF8 = Encoding.UTF8.GetBytes(json);
            internalRequest.Headers[BceConstants.HttpHeaders.ContentLength] = jsonUTF8.Length.ToString();
            internalRequest.Headers[BceConstants.HttpHeaders.ContentType] = "application/json; charset=utf-8";
            internalRequest.Content = new MemoryStream(jsonUTF8);
        }

        /// <summary>
        /// convert httpWebResponse to BceResponse
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpWebResponse"></param>
        /// <returns></returns>
        protected virtual T ToObject<T>(HttpWebResponse httpWebResponse) where T : BceResponseBase, new()
        {
            var content = httpWebResponse.GetResponseStream();
            if (content != null)
            {
                T bceResponse = JsonUtils.ToObject<T>(new StreamReader(content));
                if (bceResponse == null)
                {
                    bceResponse = new T();
                }
                bceResponse.BceRequestId = httpWebResponse.Headers[BceConstants.HttpHeaders.BceRequestId];
                return bceResponse;
            }
            return default(T);
        }

        protected void CheckNotNull(Object obj, String message)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message);
            }
        }
    }
}