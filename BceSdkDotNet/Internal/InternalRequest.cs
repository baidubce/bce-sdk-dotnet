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
using BaiduBce.Auth;
using BaiduBce.Http;

namespace BaiduBce.Internal
{
    public class InternalRequest
    {
        private Stream stream;

        public IDictionary<string, string> Parameters { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public Uri Uri { get; set; }

        public string HttpMethod { get; set; }

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
        /// 记录Stream的初始位置，用于重试时重置Stream的Position
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