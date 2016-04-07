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
using System.Linq;
using System.Text;
using System.IO;

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Uploads a new object to the specified Baidu Bos bucket. The AppendObjectRequest optionally uploads object metadata
    /// and applies a canned access control policy to the new object.
    ///
    /// <para>
    /// Baidu Bos never stores partial objects; if during this call an exception wasn't thrown, the entire object was stored.
    /// </para>
    /// </summary>
    public class AppendObjectRequest : ObjectRequestBase
    {
        /// <summary>
        /// The file containing the data to be uploaded to Baidu Bos. You must either
        /// specify a file or an InputStream containing the data to be uploaded to
        /// Baidu Bos.
        /// </summary>
        public FileInfo FileInfo { get; set; }
        /// <summary>
        /// The InputStream containing the data to be uploaded to Baidu Bos. You must
        /// either specify a file or an InputStream containing the data to be
        /// uploaded to Baidu Bos.
        /// </summary>
        public Stream Stream { get; set; }
        /// <summary>
        /// Optional metadata instructing Baidu Bos how to handle the uploaded data
        /// (e.g. custom user metadata, hooks for specifying content type, etc.). If
        /// you are uploading from an InputStream, you <bold>should always</bold>
        /// specify metadata with the content size set, otherwise the contents of the
        /// InputStream will have to be buffered in memory before they can be sent to
        /// Baidu Bos, which can have very negative performance impacts.
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }

        public long Offset { get; set; }

        public long Size { get; set; }

        public AppendObjectRequest()
        {
            ObjectMetadata = new ObjectMetadata();
        }
    }
}