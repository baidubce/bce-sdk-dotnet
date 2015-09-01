﻿// Copyright (c) 2014 Baidu.com, Inc. All Rights Reserved
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
using System.Linq;
using System.Text;

namespace BaiduBce.Services.Bos.Model
{
    public class ListObjectsResponse : BosResponseBase
    {
        public string BucketName { get; set; }
        public string NextMarker { get; set; }
        public bool IsTruncated { get; set; }
        public string Prefix { get; set; }
        public string Marker { get; set; }
        public int MaxKeys { get; set; }
        public string Delimiter { get; set; }
        public List<BosObjectSummary> Contents { get; set; }
        public List<ObjectPrefix> CommonPrefixes { get; set; }
    }
}
