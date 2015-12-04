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

using BaiduBce.Model;

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Provides options for copying an Baidu Bos object from a source location to a new destination.
    /// 
    /// <para>
    /// All <code>CopyObjectRequests</code> must specify a source bucket and key, along with a destination bucket and key.
    /// </para>
    /// </summary>
    public class CopyObjectRequest : ObjectRequestBase
    {
        /// <summary>
        /// The name of the bucket containing the object to be copied
        /// </summary>
        public string SourceBucketName { get; set; }

        /// <summary>
        /// The key in the source bucket under which the object to be copied is stored
        /// </summary>
        public string SourceKey { get; set; }

        /// <summary>
        /// Optional field specifying the object metadata for the new object
        /// </summary>
        public ObjectMetadata NewObjectMetadata { get; set; }

        /// <summary>
        /// Optional ETag value that constrain the copy request to only be executed
        /// if the source object's ETag matches the specified ETag values.
        /// </summary>
        public string ETag { get; set; }

        public CopyObjectRequest()
        {
            NewObjectMetadata = new ObjectMetadata();
        }
    }
}