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
using System.Linq;
using System.Text;

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Contains the summary of an object stored in a Baidu Bos bucket. This object doesn't contain contain the
    /// object's full metadata or any of its contents.
    /// </summary>
    public class BosObjectSummary
    {
        /// <summary>
        /// The name of the bucket in which this object is stored.
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// The key under which this object is stored.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Hex encoded MD5 hash of this object's contents, as computed by Baidu Bos.
        /// </summary>
        public string ETag { get; set; }
        /// <summary>
        /// The size of this object, in bytes.
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// The date, according to Baidu Bos, when this object was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// The user of this object - can be null if the requester doesn't have permission to view object ownership information.
        /// </summary>
        public User Owner { get; set; }
        /// <summary>
        /// storage class string of the object; can be STANDARD or STANDARD_IA.
        /// </summary>
        public string StorageClass { get; set; }
    }
}