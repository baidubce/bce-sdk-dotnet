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
    /// Request object containing all the options for setting a bucket's Access Control List (ACL).
    /// </summary>
    public class SetBucketAclRequest : BucketRequestBase
    {
        /// <summary>
        /// The canned ACL to apply to the specified bucket.
        /// </summary>
        public String CannedAcl { get; set; }
        /// <summary>
        /// The accessControlList of this specified bucket.
        /// </summary>
        public List<Grant> AccessControlList { get; set; }
    }
}