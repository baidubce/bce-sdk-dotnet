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

namespace BaiduBce
{
    /// <summary>
    /// Base exception class for any errors that occur on the client side when attempting to access a BCE service API.
    /// 
    /// <para>
    /// For example, there is no network connection available or the network request is timeout, or the server returns an
    /// invalid response that the client is unable to parse, etc
    /// 
    /// </para>
    /// <para>
    /// Error responses from services will be handled as BceServiceExceptions.
    /// 
    /// </para>
    /// </summary>
    public class BceClientException : BceBaseException
    {
        public BceClientException()
        {
        }

        public BceClientException(string message)
            : base(message)
        {
        }

        public BceClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}