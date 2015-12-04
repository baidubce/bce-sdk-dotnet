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
    /// <summary>
    /// The ListPartsResponse contains all the information about the ListParts method.
    /// </summary>
    public class ListPartsResponse : BosResponseBase
    {
        /// <summary>
        /// The name of the bucket containing the listed parts, as specified in the
        /// original request.
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// The initiated time of the associated multipart upload.
        /// </summary>
        public DateTime Initiated { get; set; }
        /// <summary>
        /// Indicates if the listing is truncated, and additional requests need to be
        /// made to get more results.
        /// </summary>
        public bool IsTruncated { get; set; }
        /// <summary>
        /// The key value specified in the original request used to identify which
        /// multipart upload contains the parts to list.
        /// </summary>
        public String Key { get; set; }
        /// <summary>
        /// The optional max parts value specified in the original request to limit
        /// how many parts are listed.
        /// </summary>
        public int? MaxParts { get; set; }
        /// <summary>
        /// If this listing is truncated, this is the part number marker that should
        /// be used in the next request to get the next page of results.
        /// </summary>
        public int NextPartNumberMarker { get; set; }
        /// <summary>
        /// The user who owns the associated multipart upload.
        /// </summary>
        public User Owner { get; set; }
        /// <summary>
        /// The optional part number marker specified in the original request to
        /// specify where in the results to begin listing parts.
        /// </summary>
        public int PartNumberMarker { get; set; }
        /// <summary>
        /// The list of parts described in this part listing.
        /// </summary>
        public List<PartSummary> Parts { get; set; }
        /// <summary>
        /// The upload ID value specified in the original request used to identify
        /// which multipart upload contains the parts to list.
        /// </summary>
        public string UploadId { get; set; }
    }
}