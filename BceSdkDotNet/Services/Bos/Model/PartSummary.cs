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

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Container for summary information about a part in a multipart upload, such as
    /// part number, size, etc.
    /// </summary>
    public class PartSummary
    {
        /// <summary>
        /// The part number describing this part's position relative to the other
        /// parts in the multipart upload. Part number must be between 1 and 10,000
        /// (inclusive).
        /// </summary>
        public int PartNumber { get; set; }
        /// <summary>
        /// The date at which this part was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// The entity tag generated from the part content.
        /// </summary>
        public string ETag { get; set; }
        /// <summary>
        /// The size, in bytes, of the part.
        /// </summary>
        public long Size { get; set; }
    }
}