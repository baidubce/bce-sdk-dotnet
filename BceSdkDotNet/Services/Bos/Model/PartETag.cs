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
    /// Container for the part number and ETag of an uploaded part. After the part is
    /// uploaded to Baidu Bos, this data is used when completing the multipart upload.
    /// </summary>
    public class PartETag
    {
        /// <summary>
        /// The part number of the associated part.
        /// </summary>
        public int PartNumber { get; set; }
        /// <summary>
        /// The entity tag generated from the content of the associated part.
        /// </summary>
        public string ETag { get; set; }
    }
}