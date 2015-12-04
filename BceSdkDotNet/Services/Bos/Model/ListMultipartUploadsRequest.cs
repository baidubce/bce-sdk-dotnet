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

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Container for the parameters of the ListMultipartUploads operation.
    /// 
    /// <para>
    /// Required Parameters: BucketName
    /// </para>
    /// </summary>
    public class ListMultipartUploadsRequest : BucketRequestBase
    {
        /// <summary>
        /// Optional parameter restricting the response to multipart uploads for keys
        /// which begin with the specified prefix. You can use prefixes to separate a
        /// bucket into different sets of keys in a way similar to how a file system
        /// uses folders.
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// The optional key marker indicating where in the results to begin listing.
        /// 
        /// <para>
        /// Together with the upload ID marker, specifies the multipart upload after
        /// which listing should begin.
        /// 
        /// </para>
        /// <para>
        /// If the upload ID marker is not specified, only the keys lexicographically
        /// greater than the specified key-marker will be included in the list.
        /// 
        /// </para>
        /// <para>
        /// If the upload ID marker is specified, any multipart uploads for a key
        /// equal to the key-marker may also be included, provided those multipart
        /// uploads have upload IDs lexicographically greater than the specified
        /// marker.
        /// </para>
        /// </summary>
        public string KeyMarker { get; set; }
        /// <summary>
        /// Optional parameter that causes multipart uploads for keys that contain
        /// the same string between the prefix and the first occurrence of the
        /// delimiter to be rolled up into a single result element. These rolled-up
        /// keys are not returned elsewhere in the response. The most commonly used
        /// delimiter is "/", which simulates a hierarchical organization similar to
        /// a file system directory structure.
        /// </summary>
        public string Delimiter { get; set; }
        /// <summary>
        /// The optional maximum number of uploads to return.
        /// </summary>
        public int? MaxUploads { get; set; }
    }
}