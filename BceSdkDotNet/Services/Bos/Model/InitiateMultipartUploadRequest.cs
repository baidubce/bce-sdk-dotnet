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

using BaiduBce.Model;

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// The InitiateMultipartUploadRequest contains the parameters used for the InitiateMultipartUpload method.
    /// 
    /// <para>
    /// Required Parameters: BucketName, Key
    /// </para>
    /// </summary>
    public class InitiateMultipartUploadRequest : ObjectRequestBase
    {
        /// <summary>
        /// Additional information about the new object being created, such as
        /// content type, content encoding, user metadata, etc.
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }

        public InitiateMultipartUploadRequest()
        {
            ObjectMetadata = new ObjectMetadata();
        }
    }
}