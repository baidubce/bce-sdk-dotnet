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

namespace BaiduBce.Auth
{
    /// <summary>
    /// Provides access to the BCE credentials used for accessing BCE services: BCE access key ID and secret access key.
    /// These credentials are used to securely sign requests to BCE services.
    /// 
    /// <para>
    /// A basic implementation of this interface is provided in <seealso cref="BaiduBce.Auth.DefaultBceCredentials"/>, but callers
    /// are free to provide their own implementation, for example, to load BCE credentials from an encrypted file.
    /// </para>
    /// </summary>
    public interface IBceCredentials
    {
        /// <summary>
        /// Returns the BCE access key ID for this credentials object.
        /// </summary>
        /// <returns> the BCE access key ID for this credentials object. </returns>
        string AccessKeyId { get; }

        /// <summary>
        /// Returns the BCE secret access key for this credentials object.
        /// </summary>
        /// <returns> the BCE secret access key for this credentials object. </returns>
        string SecretKey { get; }
    }
}