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

using log4net;

using BaiduBce.Internal;
using BaiduBce.Services.Sts.Model;
using BaiduBce.Model;
using BaiduBce.Util;

namespace BaiduBce.Services.Sts
{

    /// <summary>
    /// Provides the client for accessing the Baidu Security Token Service.
    /// </summary>
    public class StsClient : BceClientBase
    {
        private const string UrlPrefix = "/v1";
        private const string serviceEndpointFormat = "{0}://sts.{1}.baidubce.com";

        private ILog logger = LogManager.GetLogger(typeof(StsClient));

        /// <summary>
        /// Constructs a new client to invoke service methods on Sts.
        /// </summary>
        public StsClient()
            : this(new BceClientConfiguration() { Protocol = "https" })
        {
        }

        /// <summary>
        /// Constructs a new client using the client configuration to access Sts.
        /// </summary>
        /// <param name="config"> The client configuration options controlling how this client connects to STS
        ///     (e.g. retry counts, etc). </param>
        public StsClient(BceClientConfiguration config)
            : base(config, serviceEndpointFormat)
        {
        }

        /// <summary>
        /// Get a set of temporary security credentials representing your account, with default request options
        /// </summary>
        public GetSessionTokenResponse GetSessionToken()
        {
            return GetSessionToken(new GetSessionTokenRequest { });
        }

        /// <summary>
        /// <para>
        /// Get a set of temporary security credentials representing your account.
        /// </para>
        /// <para>
        /// An extra ACL string can be set in the request, which specify permissions for the returning credentials.
        /// </para> 
        /// </summary>
        /// <param name="request"> The GetSessionTokenRequest object that specifies all the parameters of this
        ///     operation. </param>
        public GetSessionTokenResponse GetSessionToken(GetSessionTokenRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(
                request, BceConstants.HttpMethod.Post, new string[] { UrlPrefix, "sessionToken" });
            if (request.DurationSeconds.HasValue)
            {
                internalRequest.Parameters["durationSeconds"] = request.DurationSeconds.ToString();
            }
            if (request.AccessControlList != null)
            {
                FillRequestBodyForJson(internalRequest, request.AccessControlList);
            }

            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<GetSessionTokenResponse>(httpWebResponse);
                }
            });
        }
    }
}