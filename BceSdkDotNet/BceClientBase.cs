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
using System.Net;
using System.IO;
using BaiduBce.Http;
using BaiduBce.Util;
using BaiduBce.Model;

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