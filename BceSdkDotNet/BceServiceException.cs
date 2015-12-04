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
using System.IO;
using System.Net;
using BaiduBce.Util;

namespace BaiduBce
{

    /// <summary>
    /// Extension of BceClientException that represents an error response returned by a BCE service.
    /// Receiving an exception of this type indicates that the caller's request was correctly transmitted to the service,
    /// but for some reason, the service was not able to process it, and returned an error response instead.
    /// 
    /// <para>
    /// BceServiceException provides callers several pieces of information that can be used to obtain more information
    /// about the error and why it occurred. In particular, the errorType field can be used to determine if the caller's
    /// request was invalid, or the service encountered an error on the server side while processing it.
    /// </para>
    /// </summary>
    public class BceServiceException : BceBaseException
    {
        public override string Message
        {
            get
            {
                return this.ErrorMessage + " (Status Code: " + this.StatusCode + "; Error Code: "
                       + this.ErrorCode + "; Request ID: " + this.RequestId + ")";
            }
        }

        /// <summary>
        /// The unique BCE identifier for the service request the caller made. The BCE request ID can uniquely identify
        /// the BCE request, and is used for reporting an error to BCE support team.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// The BCE error code represented by this exception.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// The error message as returned by the service.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The HTTP status code that was returned with this error.
        /// </summary>
        public int StatusCode { get; set; }

        public BceServiceException()
        {
        }

        public BceServiceException(string message)
            : base(message)
        {
            this.ErrorMessage = message;
        }

        public BceServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorMessage = message;
        }

        /// <summary>
        /// convert HttpWebResponse to BceServiceException
        /// </summary>
        /// <param name="response">the input HttpWebResponse</param>
        /// <returns></returns>
        public static BceServiceException CreateFromHttpWebResponse(HttpWebResponse response)
        {
            BceServiceException bse = null;
            var content = response.GetResponseStream();
            if (content != null)
            {
                var errorResponse = JsonUtils.ToObject<BceErrorResponse>(new StreamReader(content));
                if (errorResponse != null && errorResponse.Message != null)
                {
                    bse = new BceServiceException(errorResponse.Message);
                    bse.ErrorCode = errorResponse.Code;
                    bse.RequestId = errorResponse.RequestId;
                }
            }
            if (bse == null)
            {
                bse = new BceServiceException(response.StatusDescription);
                bse.RequestId = response.Headers[BceConstants.HttpHeaders.BceRequestId];
            }
            bse.StatusCode = (int) response.StatusCode;
            return bse;
        }

        private class BceErrorResponse
        {
            public string RequestId { get; set; }
            public string Code { get; set; }
            public string Message { get; set; }
        }
    }
}