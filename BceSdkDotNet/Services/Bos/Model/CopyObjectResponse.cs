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
using BaiduBce.Model;

namespace BaiduBce.Services.Bos.Model
{
    /// <summary>
    /// Contains the data returned by Baidu Bos from the
    /// <seealso cref="BaiduBce.Services.Bos.BosClient#copyObject(CopyObjectRequest copyObjectRequest)"/> call.
    /// This result may be ignored if not needed; otherwise, use this result
    /// to access information about the new object created from the copyObject call.
    /// </summary>
    public class CopyObjectResponse : BosResponseBase
    {
        /// <summary>
        /// The ETag value of the new object.
        /// </summary>
        public String ETag { get; set; }
        /// <summary>
        /// The last modified date for the new object.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}