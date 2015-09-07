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
using log4net;
using BaiduBce.Model;

namespace BaiduBce
{
    public class DefaultRetryPolicy : IRetryPolicy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (DefaultRetryPolicy));

        public const int DefaultMaxErrorRetry = 3;

        public const int DefaultMaxDelayInMillis = 20 * 1000;

        public const int DefaultScaleFactorInMillis = 300;

        public int MaxErrorRetry { get; set; }

        public int MaxDelayInMillis { get; set; }

        public int ScaleFactorInMillis { get; set; }

        public DefaultRetryPolicy()
        {
            this.MaxErrorRetry = DefaultMaxErrorRetry;
            this.MaxDelayInMillis = DefaultMaxDelayInMillis;
            this.ScaleFactorInMillis = DefaultScaleFactorInMillis;
        }

        public T Execute<T>(Func<int, T> func)
        {
            long delayForNextRetryInMillis = 0;
            for (int attempt = 1;; ++attempt)
            {
                try
                {
                    return func(attempt);
                }
                catch (Exception e)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Unable to execute request", e);
                    }
                    if (!(e is BceBaseException))
                    {
                        e = new BceClientException("Unable to execute request", e);
                    }
                    delayForNextRetryInMillis =
                        this.getDelayBeforeNextRetryInMillis(e, attempt);
                    if (delayForNextRetryInMillis < 0)
                    {
                        throw e;
                    }
                }
            }
        }

        protected virtual int getDelayBeforeNextRetryInMillis(Exception exception, int attempt)
        {
            int retriesAttempted = attempt - 1;

            int maxErrorRetry = this.MaxErrorRetry;

            // Immediately fails when it has exceeds the max retry count.
            if (retriesAttempted >= maxErrorRetry)
            {
                return -1;
            }

            if (!this.ShouldRetry(exception, retriesAttempted))
            {
                return -1;
            }

            return Math.Min(this.MaxDelayInMillis, (1 << (retriesAttempted + 1)) * this.ScaleFactorInMillis);
        }

        protected virtual bool ShouldRetry(Exception exception, int retriesAttempted)
        {
            // Always retry on client exceptions caused by WebException which has not been converted to
            // BceServiceException.
            if (exception.InnerException is WebException)
            {
                log.Debug("Retry for WebException.");
                return true;
            }

            // Always retry on client exceptions caused by IOException
            if (exception.InnerException is IOException)
            {
                log.Debug("Retry for IOException.");
                return true;
            }

            // Only retry on a subset of service exceptions
            if (exception is BceServiceException)
            {
                BceServiceException bse = exception as BceServiceException;

                /*
                 * For 500 internal server errors and 503 service unavailable errors, we want to retry, but we need to use
                 * an exponential back-off strategy so that we don't overload a server with a flood of retries.
                 */
                if (bse.StatusCode == BceConstants.HttpStatusCode.InternalServerError)
                {
                    log.Debug("Retry for internal server error.");
                    return true;
                }
                if (bse.StatusCode == BceConstants.HttpStatusCode.ServiceUnavailable)
                {
                    log.Debug("Retry for service unavailable.");
                    return true;
                }

                string errorCode = bse.ErrorCode;
                if (errorCode == BceConstants.BceErrorCode.RequestExpired)
                {
                    log.Debug("Retry for request expired.");
                    return true;
                }
            }

            return false;
        }
    }
}