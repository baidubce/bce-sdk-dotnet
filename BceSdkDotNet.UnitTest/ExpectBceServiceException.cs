using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaiduBce.UnitTest
{
    public sealed class ExpectBceServiceException : ExpectedExceptionBaseAttribute
    {
        public int? StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ExpectBceServiceException()
        {
        }

        public ExpectBceServiceException(int statusCode, String errorCode)
            : this(statusCode, errorCode, null)
        {
        }

        public ExpectBceServiceException(int statusCode, string errorCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        protected override void Verify(Exception exception)
        {
            if (!(exception is BceServiceException))
            {
                RethrowIfAssertException(exception);
                string message = string.Format(
                    "Test method {0}.{1} threw exception {2} but BceServiceException was expected.",
                    base.TestContext.FullyQualifiedTestClassName,
                    base.TestContext.TestName,
                    exception.GetType().FullName);
                throw new Exception(message);
            }
            BceServiceException bceServiceException = exception as BceServiceException;
            if (StatusCode.HasValue && StatusCode.Value != bceServiceException.StatusCode
                || ErrorCode != null && ErrorCode != bceServiceException.ErrorCode
                || ErrorMessage != null && ErrorMessage != bceServiceException.ErrorMessage)
            {
                string expected = "";
                string actual = "";
                if (StatusCode.HasValue)
                {
                    expected += ", StatusCode=" + StatusCode.Value;
                    actual += ", StatusCode=" + bceServiceException.StatusCode;
                }
                if (ErrorCode != null)
                {
                    expected += ", ErrorCode=" + ErrorCode;
                    actual += ", ErrorCode=" + bceServiceException.ErrorCode;
                }
                if (ErrorMessage != null)
                {
                    expected += ", ErrorMessage=" + ErrorMessage;
                    actual += ", ErrorMessage=" + bceServiceException.ErrorMessage;
                }
                throw new Exception(
                    string.Format("Expect [{0}], but was [{1}]", expected.Substring(2), actual.Substring(2)));
            }

        }
    }
}
