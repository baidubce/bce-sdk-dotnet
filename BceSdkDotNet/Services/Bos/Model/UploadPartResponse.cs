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

using System;
using System.Collections.Generic;
using BaiduBce.Model; 

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Contains the details returned from Baidu Bos after calling the UploadPart
    /// operation.
    /// </summary>
    public class UploadPartResponse : BosResponseBase
    {
        /// <summary>
        /// The part number of the newly uploaded part
        /// </summary>
        public int PartNumber { get; set; }
        /// <summary>
        /// The entity tag generated from the content of the upload part
        /// </summary>
        public string ETag { get; set; }
    }
}