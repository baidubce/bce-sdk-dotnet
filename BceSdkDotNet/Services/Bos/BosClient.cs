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

using BaiduBce.Internal;
using BaiduBce.Services.Bos.Model;
using BaiduBce.Model;
using BaiduBce.Util;

namespace BaiduBce.Services.Bos
{
    public class BosClient : BceClientBase
    {
        private const string UrlPrefix = "/v1";
        private const string serviceEndpointFormat = "%s://%s.bcebos.com";

        public BosClient()
            : this(new BceClientConfiguration())
        {
        }

        public BosClient(BceClientConfiguration config)
            : base(config, serviceEndpointFormat)
        {
        }

        public CreateBucketResponse CreateBucket(string bucketName)
        {
            return this.CreateBucket(new CreateBucketRequest() { BucketName = bucketName });
        }

        public CreateBucketResponse CreateBucket(CreateBucketRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request should NOT be null.");
            }

            var internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Put, request);
            return internalRequest.Config.RetryPolicy.Execute<CreateBucketResponse>(attempt =>
            {
                return this.httpClient.Execute<CreateBucketResponse>(internalRequest);
            });
        }

        public ListBucketsResponse ListBuckets(ListBucketsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request should NOT be null.");
            }

            var internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            return internalRequest.Config.RetryPolicy.Execute<ListBucketsResponse>(attempt =>
            {
                return this.httpClient.Execute<ListBucketsResponse>(internalRequest);
            });
        }

        private InternalRequest CreateInternalRequest(string httpMethod, BceRequestBase request)
        {
            string bucketName = null;
            string key = null;
            if (request is BucketRequestBase)
            {
                bucketName = (request as BucketRequestBase).BucketName;
            }
            if (request is ObjectRequestBase)
            {
                key = (request as ObjectRequestBase).Key;
            }
            var internalRequest = new InternalRequest();
            var config = this.config.Merge(request.Config);
            internalRequest.Config = config;
            internalRequest.Uri = new Uri(
                HttpUtils.AppendUri(this.ComputeEndpoint(config), UrlPrefix, bucketName, key));
            internalRequest.HttpMethod = httpMethod;
            internalRequest.Headers[BceConstants.HttpHeaders.BceDate] = DateUtils.formatAlternateIso8601Date(DateTime.Now);
            internalRequest.Headers[BceConstants.HttpHeaders.Host] = HttpUtils.GenerateHostHeader(internalRequest.Uri);
            return internalRequest;
        }
    }
}
