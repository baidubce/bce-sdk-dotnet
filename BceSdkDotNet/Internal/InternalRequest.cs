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
using System.Collections.Generic;
using System.IO;
using BaiduBce.Auth;
using BaiduBce.Http;

namespace BaiduBce.Internal
{

    /// <summary>
    /// Represents a request being sent to a BCE Service, including the
    /// parameters being sent as part of the request, the endpoint to which the
    /// request should be sent, etc.
    /// </summary>
    public class InternalRequest
    {
        private Stream stream;

        /// <summary>
        /// Map of the parameters being sent as part of this request.
        /// </summary>
        public IDictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Map of the headers included in this request
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// The service endpoint to which this request should be sent
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The HTTP method to use when sending this request.
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// An optional stream from which to read the request payload.
        /// </summary>
        public Stream Content
        {
            get
            {
                return stream;
            }
            set
            {
                stream = value;
                StartPosition = stream.Position;
            }
        }

        /// <summary>
        /// The start position of the Content Stream
        /// </summary>
        public long StartPosition { get; set; }

        public BceClientConfiguration Config { get; set; }

        public bool Expect100Continue { get; set; }

        public long[] Range { get; set; }

        public InternalRequest()
        {
            Parameters = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
        }
    }
}