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

using System.IO;

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Contains the parameters used for the UploadPart operation on Baidu Bos.
    /// 
    /// <para>
    /// Required Parameters: BucketName, Key, UploadId, PartNumber
    /// </para>
    /// </summary>
    public class UploadPartRequest : UploadRequestBase
    {
        /// <summary>
        /// The part number describing this part's position relative to the other
        /// parts in the multipart upload. Part number must be between 1 and 10,000
        /// (inclusive).
        /// </summary>
        public int PartNumber;

        /// <summary>
        /// The size of this part, in bytes.
        /// </summary>
        public long PartSize;

        /// <summary>
        /// The optional, but recommended, MD5 hash of the content of this part. If
        /// specified, this value will be sent to Baidu Bos to verify the data
        /// integrity when the data reaches Baidu Bos.
        /// </summary>
        public string Md5Digest;

        /// <summary>
        /// The stream containing the data to upload for the new part. Exactly one
        /// File or InputStream must be specified as the input to this operation.
        /// </summary>
        public Stream InputStream;
    }
}