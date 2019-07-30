﻿// Copyright 2014 Baidu, Inc.
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

namespace BaiduBce.Model
{
    /// <summary>
    /// Represents the response from an BCE service, including the result payload and any response metadata. BCE response
    /// metadata consists primarily of the BCE request ID, which can be used for debugging purposes when services aren't
    /// acting as expected.
    /// </summary>
    public class BceResponseBase
    {
        /// <summary>
        /// The request id returned by bce service.
        /// </summary>
        public String BceRequestId { get; set; }
    }
}