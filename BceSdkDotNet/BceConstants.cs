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

namespace BaiduBce
{
    public static class BceConstants
    {
        public static class Region
        {
            public const string Beijing = "bj";
        }

        public static class Protocol
        {
            public const string Http = "http";
            public const int HttpDefaultPort = 80;
            public const string Https = "https";
            public const int HttpsDefaultPort = 443;
        }

        public static class HttpHeaders
        {
            #region Common HTTP Headers

            public const string Authorization = "Authorization";

            public const string ContentDisposition = "Content-Disposition";

            public const string ContentEncoding = "Content-Encoding";

            public const string TransferEncoding = "Transfer-Encoding";

            public const string ContentLength = "Content-Length";

            public const string ContentMd5 = "Content-MD5";

            public const string ContentRange = "Content-Range";

            public const string ContentType = "Content-Type";

            public const string Date = "Date";

            public const string ETag = "ETag";

            public const string Expires = "Expires";

            public const string Host = "Host";

            public const string LastModified = "Last-Modified";

            public const string Range = "Range";

            public const string Server = "Server";

            public const string UserAgent = "User-Agent";

            #endregion

            #region BCE HTTP Headers

            public const string BcePrefix = "x-bce-";

            public const string BceAcl = "x-bce-acl";

            public const string BceContentSha256 = "x-bce-content-sha256";

            public const string BceCopyMetadataDirective = "x-bce-metadata-directive";

            public const string BceCopySource = "x-bce-copy-source";

            public const string BceCopySourceIfMatch = "x-bce-copy-source-if-match";

            public const string BceDate = "x-bce-date";

            public const string BceUserMetadataPrefix = "x-bce-meta-";

            public const string BceRequestId = "x-bce-request-id";

            #endregion

            #region BOS HTTP Headers

            public const string BosDebugId = "x-bce-bos-debug-id";

            #endregion
        }

        public static class HttpMethod
        {
            public const string Get = "GET";
            public const string Post = "POST";
            public const string Put = "PUT";
            public const string Delete = "DELETE";
            public const string Head = "HEAD";
        }

        public static class ContentType
        {
            public const string Json = "application/json; charset=utf-8";
        }

        public static class HttpStatusCode
        {
            // --- 1xx Informational ---

            /** <tt>100 Continue</tt> (HTTP/1.1 - RFC 2616) */
            public const int Continue = 100;
            /** <tt>101 Switching Protocols</tt> (HTTP/1.1 - RFC 2616)*/
            public const int SwitchingProtocol = 101;
            /** <tt>102 Processing</tt> (WebDAV - RFC 2518) */
            public const int Processing = 102;

            // --- 2xx Success ---

            /** <tt>200 OK</tt> (HTTP/1.0 - RFC 1945) */
            public const int Ok = 200;
            /** <tt>201 Created</tt> (HTTP/1.0 - RFC 1945) */
            public const int Created = 201;
            /** <tt>202 Accepted</tt> (HTTP/1.0 - RFC 1945) */
            public const int Accepted = 202;
            /** <tt>203 Non Authoritative Information</tt> (HTTP/1.1 - RFC 2616) */
            public const int NonAuthoritativeInformation = 203;
            /** <tt>204 No Content</tt> (HTTP/1.0 - RFC 1945) */
            public const int NoContent = 204;
            /** <tt>205 Reset Content</tt> (HTTP/1.1 - RFC 2616) */
            public const int ResetContent = 205;
            /** <tt>206 Partial Content</tt> (HTTP/1.1 - RFC 2616) */
            public const int PartialContent = 206;
            /**
             * <tt>207 Multi-Status</tt> (WebDAV - RFC 2518) or <tt>207 Partial Update
             * OK</tt> (HTTP/1.1 - draft-ietf-http-v11-spec-rev-01?)
             */
            public const int MultiStatus = 207;

            // --- 3xx Redirection ---

            /** <tt>300 Mutliple Choices</tt> (HTTP/1.1 - RFC 2616) */
            public const int MutlipleChoices = 300;
            /** <tt>301 Moved Permanently</tt> (HTTP/1.0 - RFC 1945) */
            public const int MovedPermanently = 301;
            /** <tt>302 Moved Temporarily</tt> (Sometimes <tt>Found</tt>) (HTTP/1.0 - RFC 1945) */
            public const int MovedTemporarily = 302;
            /** <tt>303 See Other</tt> (HTTP/1.1 - RFC 2616) */
            public const int SeeOther = 303;
            /** <tt>304 Not Modified</tt> (HTTP/1.0 - RFC 1945) */
            public const int NotModified = 304;
            /** <tt>305 Use Proxy</tt> (HTTP/1.1 - RFC 2616) */
            public const int UseProxy = 305;
            /** <tt>307 Temporary Redirect</tt> (HTTP/1.1 - RFC 2616) */
            public const int TemporaryRedirect = 307;

            // --- 4xx Client Error ---

            /** <tt>400 Bad Request</tt> (HTTP/1.1 - RFC 2616) */
            public const int BadRequest = 400;
            /** <tt>401 Unauthorized</tt> (HTTP/1.0 - RFC 1945) */
            public const int Unauthorized = 401;
            /** <tt>402 Payment Required</tt> (HTTP/1.1 - RFC 2616) */
            public const int PaymentRequired = 402;
            /** <tt>403 Forbidden</tt> (HTTP/1.0 - RFC 1945) */
            public const int Forbidden = 403;
            /** <tt>404 Not Found</tt> (HTTP/1.0 - RFC 1945) */
            public const int NotFound = 404;
            /** <tt>405 Method Not Allowed</tt> (HTTP/1.1 - RFC 2616) */
            public const int MethodNotAllowed = 405;
            /** <tt>406 Not Acceptable</tt> (HTTP/1.1 - RFC 2616) */
            public const int NotAcceptable = 406;
            /** <tt>407 Proxy Authentication Required</tt> (HTTP/1.1 - RFC 2616)*/
            public const int ProxyAuthenticationRequired = 407;
            /** <tt>408 Request Timeout</tt> (HTTP/1.1 - RFC 2616) */
            public const int RequestTimeout = 408;
            /** <tt>409 Conflict</tt> (HTTP/1.1 - RFC 2616) */
            public const int Conflict = 409;
            /** <tt>410 Gone</tt> (HTTP/1.1 - RFC 2616) */
            public const int Gone = 410;
            /** <tt>411 Length Required</tt> (HTTP/1.1 - RFC 2616) */
            public const int LengthRequired = 411;
            /** <tt>412 Precondition Failed</tt> (HTTP/1.1 - RFC 2616) */
            public const int PreconditionFailed = 412;
            /** <tt>413 Request Entity Too Large</tt> (HTTP/1.1 - RFC 2616) */
            public const int RequestEntityTooLarge = 413;
            /** <tt>414 Request-URI Too Long</tt> (HTTP/1.1 - RFC 2616) */
            public const int RequestUriTooLong = 414;
            /** <tt>415 Unsupported Media Type</tt> (HTTP/1.1 - RFC 2616) */
            public const int UnsupportedMediaType = 415;
            /** <tt>416 Requested Range Not Satisfiable</tt> (HTTP/1.1 - RFC 2616) */
            public const int RequestedRangeNotSatisfiable = 416;
            /** <tt>417 Expectation Failed</tt> (HTTP/1.1 - RFC 2616) */
            public const int ExpectationFailed = 417;

            /**
             * Static constant for a 418 error.
             * <tt>418 Unprocessable Entity</tt> (WebDAV drafts?)
             * or <tt>418 Reauthentication Required</tt> (HTTP/1.1 drafts?)
             */
            // not used
            // public const int SC_UNPROCESSABLE_ENTITY = 418;

            /**
             * Static constant for a 419 error.
             * <tt>419 Insufficient Space on Resource</tt>
             * (WebDAV - draft-ietf-webdav-protocol-05?)
             * or <tt>419 Proxy Reauthentication Required</tt>
             * (HTTP/1.1 drafts?)
             */
            public const int InsufficientSpaceOnResource = 419;
            /**
             * Static constant for a 420 error.
             * <tt>420 Method Failure</tt>
             * (WebDAV - draft-ietf-webdav-protocol-05?)
             */
            public const int MethodFailure = 420;
            /** <tt>422 Unprocessable Entity</tt> (WebDAV - RFC 2518) */
            public const int UnprocessableEntity = 422;
            /** <tt>423 Locked</tt> (WebDAV - RFC 2518) */
            public const int Locked = 423;
            /** <tt>424 Failed Dependency</tt> (WebDAV - RFC 2518) */
            public const int FailedDependency = 424;

            // --- 5xx Server Error ---

            /** <tt>500 Server Error</tt> (HTTP/1.0 - RFC 1945) */
            public const int InternalServerError = 500;
            /** <tt>501 Not Implemented</tt> (HTTP/1.0 - RFC 1945) */
            public const int NotImplemented = 501;
            /** <tt>502 Bad Gateway</tt> (HTTP/1.0 - RFC 1945) */
            public const int BadGateway = 502;
            /** <tt>503 Service Unavailable</tt> (HTTP/1.0 - RFC 1945) */
            public const int ServiceUnavailable = 503;
            /** <tt>504 Gateway Timeout</tt> (HTTP/1.1 - RFC 2616) */
            public const int GatewayTimeout = 504;
            /** <tt>505 HTTP Version Not Supported</tt> (HTTP/1.1 - RFC 2616) */
            public const int HttpVersionNotSupported = 505;

            /** <tt>507 Insufficient Storage</tt> (WebDAV - RFC 2518) */
            public const int InsufficientStorage = 507;
        }

        public static class BceErrorCode
        {
            public const string AccessDenied = "AccessDenied";
            public const string InappropriateJson = "InappropriateJSON";
            public const string InternalError = "InternalError";
            public const string InvalidAccessKeyId = "InvalidAccessKeyId";
            public const string InvalidHttpAuthHeader = "InvalidHTTPAuthHeader";
            public const string InvalidHttpRequest = "InvalidHTTPRequest";
            public const string InvalidUri = "InvalidURI";
            public const string MalformedJson = "MalformedJSON";
            public const string InvalidVersion = "InvalidVersion";
            public const string OptInRequired = "OptInRequired";
            public const string PreconditionFailed = "PreconditionFailed";
            public const string RequestExpired = "RequestExpired";
            public const string SignatureDoesNotMatch = "SignatureDoesNotMatch";
        }
    }
}