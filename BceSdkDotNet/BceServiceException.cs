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

using LitJson;
using System;
using System.IO;
using System.Net;

namespace BaiduBce
{
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

        public string RequestId { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

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

        public static BceServiceException CreateFromHttpWebResponse(HttpWebResponse response)
        {
            BceServiceException bse = null;
            var content = response.GetResponseStream();
            if (content != null)
            {
                var body = new StreamReader(content).ReadToEnd();
                var errorResponse = JsonMapper.ToObject<BceErrorResponse>(body);
                if (errorResponse.Message != null)
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
            bse.StatusCode = (int)response.StatusCode;
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
