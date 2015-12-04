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
using System.Collections;
using System.Collections.Generic;

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Contains options to generate a pre-signed URL for an Baidu BOS resource.
    /// 
    /// <para>
    /// Pre-signed URLs allow clients to form a URL for an Baidu BOS resource and
    /// sign it with the current BCE security credentials.
    /// A pre-signed URL may be passed around for other users to access
    /// the resource without providing them
    /// access to an account's BCE security credentials.
    /// </para>
    /// </summary>
    public class GeneratePresignedUrlRequest : ObjectRequestBase
    {
        /// <summary>
        /// The HTTP method (GET, PUT, DELETE, HEAD) to be used in this request and when the pre-signed URL is used
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// The optional Content-Type header that will be sent when the presigned URL is accessed
        /// </summary>
        public string ContentType;
        /// <summary>
        /// The optional Content-MD5 header that will be sent when the presigned URL is accessed
        /// </summary>
        public string ContentMd5;
        /// <summary>
        /// An optional expiration after which point the generated pre-signed URL
        /// will no longer be accepted by BOS. If not specified, a default
        /// value will be supplied.
        /// </summary>
        public int ExpirationInSeconds;
        /// <summary>
        /// An optional map of additional parameters to include in the pre-signed
        /// URL. Adding additional request parameters enables more advanced
        /// pre-signed URLs, such as accessing BOS's torrent resource for an
        /// object, or for specifying a version ID when accessing an object.
        /// </summary>
        public IDictionary<String, String> RequestParameters = new Dictionary<String, String>();
        /// <summary>
        /// An optional map of additional headers to include in the pre-signed URL.
        /// </summary>
        public IDictionary<String, String> RequestHeaders = new Dictionary<String, String>();
    }
}