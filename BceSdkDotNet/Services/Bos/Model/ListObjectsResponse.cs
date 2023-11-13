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
    /// Contains the results of listing the objects in an Baidu Bos bucket.
    /// This includes a list of objects describing the objects stored in
    /// the bucket, a list of common prefixes if a delimiter was specified in the
    /// request, information describing if this is a complete or partial
    /// listing, and the original request parameters.
    /// </summary>
    public class ListObjectsResponse : BosResponseBase
    {
        /// <summary>
        /// The name of the Baidu Bos bucket containing the listed objects
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// The marker to use in order to request the next page of results - only
        /// populated if the isTruncated member indicates that this object listing is truncated
        /// </summary>
        public string NextMarker { get; set; }
        /// <summary>
        /// Indicates if this is a complete listing, or if the caller needs to make
        /// additional requests to Baidu Bos to see the full object listing for an Bos bucket
        /// </summary>
        public bool IsTruncated { get; set; }
        /// <summary>
        /// The prefix parameter originally specified by the caller when this object listing was returned
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// The marker parameter originally specified by the caller when this object listing was returned
        /// </summary>
        public string Marker { get; set; }
        /// <summary>
        /// The maxKeys parameter originally specified by the caller when this object listing was returned
        /// </summary>
        public int MaxKeys { get; set; }
        /// <summary>
        /// The delimiter parameter originally specified by the caller when this object listing was returned
        /// </summary>
        public string Delimiter { get; set; }
        /// <summary>
        /// A list of summary information describing the objects stored in the bucket.
        /// </summary>
        public List<BosObjectSummary> Contents { get; set; }
        /// <summary>
        /// A list of the common prefixes included in this object listing - common
        /// prefixes will only be populated for requests that specified a delimiter.
        /// </summary>
        public List<ObjectPrefix> CommonPrefixes { get; set; }
    }
}