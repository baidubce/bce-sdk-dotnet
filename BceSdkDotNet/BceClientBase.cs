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
    public abstract class BceClientBase
    {
        private string serviceEndpointFormat;

        protected BceClientConfiguration config;

        internal BceHttpClient httpClient;

        public BceClientBase(BceClientConfiguration config, string serviceEndpointFormat)
        {
            this.serviceEndpointFormat = serviceEndpointFormat;
            this.config = BceClientConfiguration.CreateWithDefaultValues().Merge(config);
            this.httpClient = new BceHttpClient();
        }

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

        protected void CheckNotNull(Object obj,String message)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(message);
            }
        }
    }
}
