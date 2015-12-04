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
    /// <summary>
    /// The ListMultipartUploadsResponse contains all the information about the
    /// ListMultipartUploads method.
    /// </summary>
    public class ListMultipartUploadsResponse : BosResponseBase
    {
        /// <summary>
        /// The name of the bucket containing the listed multipart uploads, as
        /// specified in the original request.
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// The optional key marker specified in the original request to specify
        /// where in the results to begin listing multipart uploads.
        /// </summary>
        public string KeyMarker { get; set; }
        /// <summary>
        /// The optional prefix specified in the original request to limit the
        /// returned multipart uploads to those for keys that match this prefix.
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// The optional delimiter specified in the original request to control how
        /// multipart uploads for keys with common prefixes are condensed.
        /// </summary>
        public string Delimiter { get; set; }
        /// <summary>
        /// The optional maximum number of uploads to be listed, as specified in the
        /// original request.
        /// </summary>
        public int MaxUploads { get; set; }
        /// <summary>
        /// Indicates if the listing is truncated, and additional requests need to be
        /// made to get more results.
        /// </summary>
        public bool IsTruncated { get; set; }
        /// <summary>
        /// If this listing is truncated, this is the next key marker that should be
        /// used in the next request to get the next page of results.
        /// </summary>
        public string NextKeyMarker { get; set; }
        /// <summary>
        /// The list of multipart uploads. </summary>
        public List<MultipartUploadSummary> Uploads { get; set; }
        /// <summary>
        /// A list of the common prefixes included in this multipart upload listing - common
        /// prefixes will only be populated for requests that specified a delimiter, and indicate
        /// additional key prefixes that contain more multipart uploads that have not been included
        /// in this listing.
        /// </summary>
        public List<ObjectPrefix> CommonPrefixes { get; set; }
    }
}