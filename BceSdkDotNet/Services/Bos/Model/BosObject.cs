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
    /// Represents an object stored in Baidu Bos. This object contains the data content
    /// and the object metadata stored by Baidu Bos, such as content type, content length, etc.
    /// </summary>
    public class BosObject
    {
        /// <summary>
        /// The name of the bucket in which this object is contained.
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// The key under which this object is stored.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// The metadata stored by Baidu Bos for this object.
        /// </summary>
        public ObjectMetadata ObjectMetadata { get; set; }
        /// <summary>
        /// The stream containing the contents of this object from Bos.
        /// </summary>
        public Stream ObjectContent { get; set; }
    }
}