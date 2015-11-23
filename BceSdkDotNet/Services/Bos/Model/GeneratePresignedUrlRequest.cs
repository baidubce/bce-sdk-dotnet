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
using System.Collections;
using System.Collections.Generic;

namespace BaiduBce.Services.Bos.Model
{
    public class GeneratePresignedUrlRequest : ObjectRequestBase
    {
        public string Method { get; set; }
        public string ContentType;
        public string ContentMd5;
        public int ExpirationInSeconds;
        public IDictionary<String, String> RequestParameters = new Dictionary<String, String>();
        public IDictionary<String, String> RequestHeaders = new Dictionary<String, String>();
    }
}