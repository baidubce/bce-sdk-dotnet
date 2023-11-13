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

using System.Collections.Generic;

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Contains the results of initiating a multipart upload, particularly the unique ID of the new multipart upload.
    /// </summary>
    public class InitiateMultipartUploadResponse : BosResponseBase
    {
        /// <summary>
        /// The object key for which the multipart upload was initiated.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The name of the bucket in which the new multipart upload was initiated.
        /// </summary>
        public string Bucket { get; set; }

        /// <summary>
        /// The unique ID of the new multipart upload.
        /// </summary>
        public string UploadId { get; set; }
    }
}