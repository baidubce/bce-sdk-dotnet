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
using BaiduBce.Auth;

namespace BaiduBce.Model
{
    /// <summary>
    /// Base class for all BCE web service request objects.
    /// </summary>
    public class BceRequestBase
    {
        /// <summary>
        /// The credentials to use for this request. If this property is set, the credentials provided in config will be
        /// ingored.
        /// </summary>
        public IBceCredentials Credentials { get; set; }

        /// <summary>
        /// The optional config to use for this request - overrides the default config set at the client level.
        /// </summary>
        public BceClientConfiguration Config { get; set; }
    }
}