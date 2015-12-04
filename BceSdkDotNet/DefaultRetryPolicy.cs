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
    /// <summary>
    /// Retry policy that can be configured on a specific service client using <seealso cref="BceClientConfiguration"/>. This class is
    /// immutable, therefore safe to be shared by multiple clients.
    /// </summary>
    /// <seealso cref= "BceClientConfiguration"/>
    public class DefaultRetryPolicy : IRetryPolicy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (DefaultRetryPolicy));

        public const int DefaultMaxErrorRetry = 3;

        public const int DefaultMaxDelayInMillis = 20 * 1000;

        public const int DefaultScaleFactorInMillis = 300;

        /// <summary>
        /// Non-negative integer indicating the max retry count.
        /// </summary>
        public int MaxErrorRetry { get; set; }

        /// <summary>
        /// Max delay time in millis.
        /// </summary>
        public int MaxDelayInMillis { get; set; }

        /// <summary>
        /// Base sleep time (milliseconds) for general exceptions. *
        /// </summary>
        public int ScaleFactorInMillis { get; set; }

        public bool CanRetry { get; set; }

        /// <summary>
        /// Constructs a new DefaultRetryPolicy.
        /// </summary>
        public DefaultRetryPolicy()
        {
            this.MaxErrorRetry = DefaultMaxErrorRetry;
            this.MaxDelayInMillis = DefaultMaxDelayInMillis;
            this.ScaleFactorInMillis = DefaultScaleFactorInMillis;
            this.CanRetry = true;
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
                        this.GetDelayBeforeNextRetryInMillis(e, attempt);
                    if (delayForNextRetryInMillis < 0)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the delay (in milliseconds) before next retry attempt. A negative value indicates that no more retries
        /// should be made.
        /// </summary>
        /// <param name="exception">        the exception from the failed request, represented as an BceClientException object. </param>
        /// <param name="attempt"> the number of times the current request has been attempted
        ///         (not including the next attempt after the delay). </param>
        /// <returns> the delay (in milliseconds) before next retry attempt.A negative value indicates that no more retries
        ///         should be made. </returns>
        protected virtual int GetDelayBeforeNextRetryInMillis(Exception exception, int attempt)
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

        /// <summary>
        /// Returns whether a failed request should be retried according to the given request context. In the following
        /// circumstances, the request will fail directly without consulting this method:
        /// <ul>
        /// <li>if it has already reached the max retry limit,
        /// <li>if the request contains non-repeatable content,
        /// <li>if any RuntimeException or Error is thrown when executing the request.
        /// </ul>
        /// </summary>
        /// <param name="exception">        the exception from the failed request, represented as a BceClientException object. </param>
        /// <param name="retriesAttempted"> the number of times the current request has been attempted. </param>
        /// <returns> true if the failed request should be retried. </returns>
        protected virtual bool ShouldRetry(Exception exception, int retriesAttempted)
        {
            // Don't retry if stream can not seek.
            if (!CanRetry)
            {
                return false;
            }

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