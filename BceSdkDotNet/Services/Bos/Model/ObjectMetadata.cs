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

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Represents the object metadata that is stored with Baidu Bos. This includes custom
    /// user-supplied metadata, as well as the standard HTTP headers that Baidu Bos
    /// sends and receives (Content-Length, ETag, Content-MD5, etc.).
    /// </summary>
    public class ObjectMetadata
    {
        /// <summary>
        /// Custom user metadata, represented in responses with the x-bce-meta- header prefix
        /// </summary>
        public IDictionary<String, String> UserMetadata { get; set; }
        /// <summary>
        /// The SHA-256 of the object content.
        /// </summary>
        public string BceContentSha256 { get; set; }
        /// <summary>
        /// The optional Content-Disposition HTTP header, which specifies
        /// presentation information for the object such as the recommended filename
        /// for the object to be saved as.
        /// </summary>
        public string ContentDisposition { get; set; }
        /// <summary>
        /// The optional Content-Encoding HTTP header specifying what
        /// content encodings have been applied to the object and what decoding
        /// mechanisms must be applied in order to obtain the media-type referenced
        /// by the Content-Type field.
        /// </summary>
        public string ContentEncoding { get; set; }
        /// <summary>
        /// The Content-Length HTTP header indicating the size of the
        /// associated object in bytes.
        /// </summary>
        public long ContentLength { get; set; }
        /// <summary>
        /// The base64 encoded 128-bit MD5 digest of the associated object
        /// (content - not including headers) according to RFC 1864. This data is
        /// used as a message integrity check to verify that the data received by
        /// Baidu Bos is the same data that the caller sent.
        /// </summary>
        public string ContentMd5 { get; set; }
        /// <summary>
        /// The Content-Type HTTP header, which indicates the type of content
        /// stored in the associated object. The value of this header is a standard
        /// MIME type.
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// The hex encoded 128-bit MD5 digest of the associated object
        /// according to RFC 1864. This data is used as an integrity check to verify
        /// that the data received by the caller is the same data that was sent by
        /// Baidu Bos.
        ///
        /// <para>
        /// This field represents the hex encoded 128-bit MD5 digest of an object's
        /// content as calculated by Baidu Bos. The ContentMD5 field represents the
        /// base64 encoded 128-bit MD5 digest as calculated on the caller's side.
        ///
        /// </para>
        /// </summary>
        public string ETag { get; set; }
        /// <summary>
        /// The physical length of the entire object stored in Bos.
        /// This is useful during, for example, a range get operation.
        /// </summary>
        public long InstanceLength { get; set; }
        /// <summary>
        /// The value of the Last-Modified header, indicating the date
        /// and time at which Baidu Bos last recorded a modification to the
        /// associated object.
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// The content range of object.
        /// </summary>
        public string ContentRange { get; set; }

        public int BceNextAppendOffset { get; set; }

        public string BceObjectType { get; set; }

        public string Expires { get; set; }

        public string CacheControl { get; set; }

        public string BceObjectAcl { get; set; }

        public string BceObjectGrantRead { get; set; }

        public ObjectMetadata()
        {
            UserMetadata = new Dictionary<String, String>();
        }
    }
}