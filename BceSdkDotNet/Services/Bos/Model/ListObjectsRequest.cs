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
    /// Contains options to return a list of summary information about the objects in the specified
    /// bucket. Depending on the request parameters, additional information is returned,
    /// such as common prefixes if a delimiter was specified. List
    /// results are <i>always</i> returned in lexicographic (alphabetical) order.
    /// </summary>
    public class ListObjectsRequest : BucketRequestBase
    {
        /// <summary>
        /// Optional parameter restricting the response to keys which begin with the
        /// specified prefix. You can use prefixes to separate a bucket into
        /// different sets of keys in a way similar to how a file system uses
        /// folders.
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// Optional parameter indicating where in the bucket to begin listing. The
        /// list will only include keys that occur lexicographically after the
        /// marker.
        /// </summary>
        public string Marker { get; set; }
        /// <summary>
        /// Optional parameter that causes keys that contain the same string between
        /// the prefix and the first occurrence of the delimiter to be rolled up into
        /// a single result element. These rolled-up keys
        /// are not returned elsewhere in the response. The most commonly used
        /// delimiter is "/", which simulates a hierarchical organization similar to
        /// a file system directory structure.
        /// </summary>
        public string Delimiter { get; set; }
        /// <summary>
        /// Optional parameter indicating the maximum number of keys to include in
        /// the response. Baidu Bos might return fewer than this, but will not return
        /// more. Even if maxKeys is not specified, Baidu Bos will limit the number
        /// of results in the response.
        /// </summary>
        public int? MaxKeys { get; set; }
    }
}