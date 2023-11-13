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

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// The CompleteMultipartUploadResponse contains all the information about the CompleteMultipartUpload method.
    /// </summary>
    public class CompleteMultipartUploadResponse : BosResponseBase
    {
        /// <summary>
        /// The name of the bucket containing the completed multipart upload.
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// The key by which the object is stored.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// The URL identifying the new multipart object.
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// The entity tag identifying the new object. An entity tag is an opaque
        /// string that changes if and only if an object's data changes.
        /// </summary>
        public string ETag { get; set; }
    }
}