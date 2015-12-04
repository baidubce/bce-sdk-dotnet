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
    /// A multipart upload is an upload to Baidu Bos that is creating by uploading
    /// individual pieces of an object, then telling Baidu Bos to complete the
    /// multipart upload and concatenate all the individual pieces together into a
    /// single object.
    /// </summary>
    public class MultipartUploadSummary
    {
        /// <summary>
        /// The key by which this upload is stored.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// The unique ID of this multipart upload.
        /// </summary>
        public string UploadId { get; set; }
        /// <summary>
        /// The owner of this multipart upload.
        /// </summary>
        public User Owner { get; set; }
        /// <summary>
        /// The date at which this upload was initiated.
        /// </summary>
        public DateTime Initiated { get; set; }
    }
}