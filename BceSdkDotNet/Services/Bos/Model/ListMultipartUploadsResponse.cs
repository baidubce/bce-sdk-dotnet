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

using System.Collections.Generic;

namespace BaiduBce.Services.Bos.Model
{
    public class ListMultipartUploadsResponse : BosResponseBase
    {
        public string BucketName { get; set; }
        public string KeyMarker { get; set; }
        public string Prefix { get; set; }
        public string Delimiter { get; set; }
        public int MaxUploads { get; set; }
        public bool IsTruncated { get; set; }
        public string NextKeyMarker { get; set; }
        public List<MultipartUploadSummary> Uploads { get; set; }
        public List<ObjectPrefix> CommonPrefixes { get; set; }
    }
}