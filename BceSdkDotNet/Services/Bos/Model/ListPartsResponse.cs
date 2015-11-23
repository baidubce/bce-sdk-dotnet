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

namespace BaiduBce.Services.Bos.Model
{
    public class ListPartsResponse : BosResponseBase
    {
        public string BucketName { get; set; }
        public DateTime Initiated { get; set; }
        public bool IsTruncated { get; set; }
        public String Key { get; set; }
        public int? MaxParts { get; set; }
        public int NextPartNumberMarker { get; set; }
        public User Owner { get; set; }
        public int PartNumberMarker { get; set; }
        public List<PartSummary> Parts { get; set; }
        public string UploadId { get; set; }
    }
}