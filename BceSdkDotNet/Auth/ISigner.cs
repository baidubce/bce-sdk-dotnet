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
using BaiduBce.Internal;

namespace BaiduBce.Auth
{
    /// <summary>
    /// A strategy for applying cryptographic signatures to a request, proving that the request was made by someone in
    /// possession of the given set of credentials without transmitting the secret key over the wire.
    /// </summary>
    public interface ISigner
    {
        /// <summary>
        /// Sign the given request. Modifies the passed-in request to apply the signature.
        /// </summary>
        /// <param name="request">     the request to sign. </param>
        string Sign(InternalRequest request);
    }
}