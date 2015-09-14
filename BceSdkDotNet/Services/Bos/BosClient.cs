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
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Web;
using System.Text;
using log4net;
using BaiduBce.Internal;
using BaiduBce.Services.Bos.Model;
using BaiduBce.Model;
using BaiduBce.Util;

namespace BaiduBce.Services.Bos
{
    public class BosClient : BceClientBase
    {
        private const string UrlPrefix = "/v1";
        private const string serviceEndpointFormat = "%s://%s.bcebos.com";

        private ILog logger = LogManager.GetLogger(typeof (BosClient));

        public BosClient()
            : this(new BceClientConfiguration())
        {
        }

        public BosClient(BceClientConfiguration config)
            : base(config, serviceEndpointFormat)
        {
        }

        public CreateBucketResponse CreateBucket(string bucketName)
        {
            return this.CreateBucket(new CreateBucketRequest() {BucketName = bucketName});
        }

        public CreateBucketResponse CreateBucket(CreateBucketRequest request)
        {
            CheckNotNull(request, "request should NOT be null.");

            var internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Put, request);
            return internalRequest.Config.RetryPolicy.Execute<CreateBucketResponse>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<CreateBucketResponse>(httpWebResponse);
                }
            });
        }

        public ListBucketsResponse ListBuckets()
        {
            return ListBuckets(new ListBucketsRequest());
        }

        public ListBucketsResponse ListBuckets(ListBucketsRequest request)
        {
            CheckNotNull(request, "request should NOT be null.");

            var internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            return internalRequest.Config.RetryPolicy.Execute<ListBucketsResponse>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<ListBucketsResponse>(httpWebResponse);
                }
            });
        }

        public void DeleteBucket(String bucketName)
        {
            this.DeleteBucket(new DeleteBucketRequest() {BucketName = bucketName});
        }

        public void DeleteBucket(DeleteBucketRequest request)
        {
            CheckNotNull(request, "request should NOT be null.");

            var internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Delete, request);
            internalRequest.Config.RetryPolicy.Execute<DeleteBucketResponse>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<DeleteBucketResponse>(httpWebResponse);
                }
            });
        }

        public ListObjectsResponse ListObjects(String bucketName)
        {
            return this.ListObjects(new ListObjectsRequest() {BucketName = bucketName});
        }

        public ListObjectsResponse ListObjects(String bucketName, String prefix)
        {
            return this.ListObjects(new ListObjectsRequest() {BucketName = bucketName, Prefix = prefix});
        }

        public ListObjectsResponse ListObjects(ListObjectsRequest request)
        {
            CheckNotNull(request, "request should NOT be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            if (request.Prefix != null)
            {
                internalRequest.Parameters["prefix"] = request.Prefix;
            }
            if (request.Marker != null)
            {
                internalRequest.Parameters["marker"] = request.Marker;
            }
            if (request.Delimiter != null)
            {
                internalRequest.Parameters["delimiter"] = request.Delimiter;
            }
            if (request.MaxKeys != null && request.MaxKeys >= 0)
            {
                internalRequest.Parameters["maxKeys"] = request.MaxKeys.ToString();
            }

            return internalRequest.Config.RetryPolicy.Execute<ListObjectsResponse>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    ListObjectsResponse listObjectsResponse = ToObject<ListObjectsResponse>(httpWebResponse);
                    if (listObjectsResponse != null)
                    {
                        listObjectsResponse.BucketName = request.BucketName;
                        List<BosObjectSummary> contents = listObjectsResponse.Contents;
                        if (contents != null && contents.Count > 0)
                        {
                            foreach (BosObjectSummary summary in contents)
                            {
                                summary.BucketName = request.BucketName;
                            }
                        }
                    }
                    return listObjectsResponse;
                }
            });
        }

        public ListObjectsResponse ListNextBatchOfObjects(ListObjectsResponse previousResponse)
        {
            CheckNotNull(previousResponse, "response should NOT be null.");
            if (!previousResponse.IsTruncated)
            {
                return new ListObjectsResponse()
                {
                    BucketName = previousResponse.BucketName,
                    Delimiter = previousResponse.Delimiter,
                    Marker = previousResponse.NextMarker,
                    MaxKeys = previousResponse.MaxKeys,
                    Prefix = previousResponse.Prefix,
                    IsTruncated = false
                };
            }

            return this.ListObjects(new ListObjectsRequest()
            {
                BucketName = previousResponse.BucketName,
                Prefix = previousResponse.Prefix,
                Marker = previousResponse.NextMarker,
                Delimiter = previousResponse.Delimiter,
                MaxKeys = previousResponse.MaxKeys
            });
        }

        public PutObjectResponse PutObject(String bucketName, String key, FileInfo fileInfo)
        {
            return this.PutObject(new PutObjectRequest() {BucketName = bucketName, Key = key, FileInfo = fileInfo});
        }

        public PutObjectResponse PutObject(String bucketName, String key, FileInfo fileInfo, ObjectMetadata metadata)
        {
            return this.PutObject(new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
                FileInfo = fileInfo,
                ObjectMetadata = metadata
            });
        }

        public PutObjectResponse PutObject(String bucketName, String key, String value)
        {
            return this.PutObject(bucketName, key, value, new ObjectMetadata());
        }

        public PutObjectResponse PutObject(String bucketName, String key, String value, ObjectMetadata metadata)
        {
            return this.PutObject(bucketName, key, Encoding.UTF8.GetBytes(value), metadata);
        }

        public PutObjectResponse PutObject(String bucketName, String key, byte[] value)
        {
            return this.PutObject(bucketName, key, value, new ObjectMetadata());
        }

        public PutObjectResponse PutObject(String bucketName, String key, byte[] value, ObjectMetadata metadata)
        {
            if (metadata.ContentLength == 0)
            {
                metadata.ContentLength = value.Length;
            }
            return this.PutObject(new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
                Stream = new MemoryStream(value),
                ObjectMetadata = metadata
            });
        }

        public PutObjectResponse PutObject(String bucketName, String key, Stream input)
        {
            return this.PutObject(bucketName, key, input, new ObjectMetadata());
        }

        public PutObjectResponse PutObject(String bucketName, String key, Stream input, ObjectMetadata metadata)
        {
            return this.PutObject(new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
                Stream = input,
                ObjectMetadata = metadata
            });
        }

        public PutObjectResponse PutObject(PutObjectRequest request)
        {
            CheckNotNull(request, "request should NOT be null.");
            if (String.IsNullOrEmpty(request.Key))
            {
                throw new ArgumentNullException("object key should not be null or empty");
            }

            ObjectMetadata metadata = request.ObjectMetadata;
            Stream input = request.Stream;
            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Put, request);

            // If a file is specified for upload, we need to pull some additional information from it to auto-configure a
            // few options
            if (request.FileInfo != null)
            {
                FileInfo fileInfo = request.FileInfo;
                if (fileInfo.Length > 5 * 1024 * 1024 * 1024L)
                {
                    throw new BceServiceException()
                    {
                        ErrorMessage = "Your proposed upload exceeds the maximum allowed object size.",
                        StatusCode = 400,
                        ErrorCode = "EntityTooLarge"
                    };
                }

                // Always set the content length, even if it's already set
                metadata.ContentLength = fileInfo.Length;

                if (metadata.ContentType == null)
                {
                    metadata.ContentType = MimeTypes.GetMimetype(fileInfo);
                }
                internalRequest.Content = fileInfo.OpenRead();
                metadata.ContentMd5 = HashUtils.ComputeMD5HashWithBase64(fileInfo);
            }
            else
            {
                CheckNotNull(input, "Either file or inputStream should be set for PutObjectRequest.");
                if (metadata.ContentLength < 0)
                {
                    logger.Warn("No content length specified for stream data.");
                    metadata.ContentLength = input.Length;
                }
                else if (metadata.ContentLength > input.Length)
                {
                    throw new ArgumentNullException("ContentLength should not be greater than stream length");
                }
                internalRequest.Content = input;
            }

            internalRequest.Headers[BceConstants.HttpHeaders.ContentLength] = metadata.ContentLength.ToString();
            PopulateRequestMetadata(internalRequest, metadata);

            using (internalRequest.Content)
            {
                return internalRequest.Config.RetryPolicy.Execute<PutObjectResponse>(attempt =>
                {
                    var httpWebResponse = this.httpClient.Execute(internalRequest);
                    using (httpWebResponse)
                    {
                        PutObjectResponse putObjectResponse = ToObject<PutObjectResponse>(httpWebResponse);
                        putObjectResponse.ETAG =
                            httpWebResponse.Headers[BceConstants.HttpHeaders.ETag].Replace("\"", "");
                        return putObjectResponse;
                    }
                });
            }
        }

        /// <summary>
        /// Gets the object stored in Bos under the specified bucket and key.
        /// You should close the stream after you call this method
        /// </summary>
        /// <param name="bucketName">The name of the bucket containing the desired object.</param>
        /// <param name="key">The key under which the desired object is stored.</param>
        /// <returns>The object stored in Bos in the specified bucket and key.</returns>
        public BosObject GetObject(String bucketName, String key)
        {
            return this.GetObject(new GetObjectRequest() {BucketName = bucketName, Key = key});
        }

        public ObjectMetadata GetObject(String bucketName, String key, FileInfo destinationFile)
        {
            return this.GetObject(new GetObjectRequest() {BucketName = bucketName, Key = key}, destinationFile);
        }

        public byte[] GetObjectContent(String bucketName, String key)
        {
            return this.GetObjectContent(new GetObjectRequest() {BucketName = bucketName, Key = key});
        }

        public byte[] GetObjectContent(GetObjectRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = CreateInternalRequestForGetObject(request);

            return internalRequest.Config.RetryPolicy.Execute<byte[]>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return IOUtils.StreamToBytes(
                        httpWebResponse.GetResponseStream(),
                        httpWebResponse.ContentLength,
                        (int) config.SocketBufferSizeInBytes);
                }
            });
        }

        /// <summary>
        /// Gets the object stored in Bos under the specified bucket and key.
        /// You should close the stream after you call this method
        /// </summary>
        /// <param name="request">The request object containing all the options on how to download the object.</param>
        /// <returns>The object stored in Bos in the specified bucket and key.</returns>
        public BosObject GetObject(GetObjectRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = CreateInternalRequestForGetObject(request);

            return internalRequest.Config.RetryPolicy.Execute<BosObject>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                BosObject bosObject = new BosObject();
                bosObject.BucketName = request.BucketName;
                bosObject.Key = request.Key;
                bosObject.ObjectContent = httpWebResponse.GetResponseStream();
                bosObject.ObjectMetadata = GetObjectMetadata(httpWebResponse);
                return bosObject;
            });
        }

        public ObjectMetadata GetObject(GetObjectRequest request, FileInfo destinationFileInfo)
        {
            CheckNotNull(request, "request should not be null.");
            CheckNotNull(destinationFileInfo, "destinationFile should not be null.");

            InternalRequest internalRequest = CreateInternalRequestForGetObject(request);

            return internalRequest.Config.RetryPolicy.Execute<ObjectMetadata>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    IOUtils.StreamToFile(
                        httpWebResponse.GetResponseStream(),
                        destinationFileInfo,
                        (int) config.SocketBufferSizeInBytes);
                    return GetObjectMetadata(httpWebResponse);
                }
            });
        }

        public ObjectMetadata GetObjectMetadata(String bucketName, String key)
        {
            return this.GetObjectMetadata(new ObjectRequestBase() {BucketName = bucketName, Key = key});
        }

        public ObjectMetadata GetObjectMetadata(ObjectRequestBase request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            return internalRequest.Config.RetryPolicy.Execute<ObjectMetadata>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return GetObjectMetadata(httpWebResponse);
                }
            });
        }

        public void DeleteObject(String bucketName, String key)
        {
            this.DeleteObject(new ObjectRequestBase() {BucketName = bucketName, Key = key});
        }

        public void DeleteObject(ObjectRequestBase request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = CreateInternalRequest(BceConstants.HttpMethod.Delete, request);
            internalRequest.Config.RetryPolicy.Execute<BosResponseBase>(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<BosResponseBase>(httpWebResponse);
                }
            });
        }

        protected override T ToObject<T>(HttpWebResponse httpWebResponse)
        {
            T response = base.ToObject<T>(httpWebResponse);
            if (response is BosResponseBase)
            {
                (response as BosResponseBase).BosDebugId =
                    httpWebResponse.Headers[BceConstants.HttpHeaders.BosDebugId];
            }
            return response;
        }

        #region private methods

        private ObjectMetadata GetObjectMetadata(HttpWebResponse httpWebResponse)
        {
            ObjectMetadata objectMetadata = new ObjectMetadata();
            long contentLength;
            if (long.TryParse(httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ContentLength),
                out contentLength))
            {
                objectMetadata.ContentLength = contentLength;
            }

            objectMetadata.ContentType = httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ContentType);
            objectMetadata.ContentEncoding =
                httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ContentEncoding);
            objectMetadata.ContentMd5 = httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ContentMd5);
            objectMetadata.ContentDisposition =
                httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ContentDisposition);
            String eTag = httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ETag);
            if (eTag != null)
            {
                objectMetadata.ETag = eTag.Replace("\"", "");
            }
            String contentRange = httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ContentRange);
            objectMetadata.ContentRange = contentRange;
            if (contentRange != null)
            {
                int pos = contentRange.LastIndexOf('/');
                if (pos >= 0)
                {
                    try
                    {
                        objectMetadata.InstanceLength = long.Parse(contentRange.Substring(pos + 1));
                    }
                    catch (FormatException e)
                    {
                        logger.Warn(
                            "Fail to parse length from " + BceConstants.HttpHeaders.ContentRange + ": " + contentRange,
                            e);
                    }
                }
            }
            objectMetadata.LastModified =
                DateUtils.ParseRfc822Date(httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.LastModified));
            objectMetadata.BceContentSha256 =
                httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.BceContentSha256);
            foreach (String header in httpWebResponse.Headers.AllKeys)
            {
                if (header.StartsWith(BceConstants.HttpHeaders.BceUserMetadataPrefix))
                {
                    string key = header.Substring(BceConstants.HttpHeaders.BceUserMetadataPrefix.Length);
                    objectMetadata.UserMetadata[HttpUtility.UrlDecode(key)] =
                        HttpUtility.UrlDecode(httpWebResponse.GetResponseHeader(header));
                }
            }
            return objectMetadata;
        }

        private static void PopulateRequestMetadata(InternalRequest request, ObjectMetadata metadata)
        {
            if (metadata.ContentType != null)
            {
                request.Headers[BceConstants.HttpHeaders.ContentType] = metadata.ContentType;
            }
            if (metadata.ContentMd5 != null)
            {
                request.Headers[BceConstants.HttpHeaders.ContentMd5] = metadata.ContentMd5;
            }
            if (metadata.ContentEncoding != null)
            {
                request.Headers[BceConstants.HttpHeaders.ContentEncoding] = metadata.ContentEncoding;
            }
            if (metadata.ContentDisposition != null)
            {
                request.Headers[BceConstants.HttpHeaders.ContentDisposition] = metadata.ContentDisposition;
            }
            if (metadata.BceContentSha256 != null)
            {
                request.Headers[BceConstants.HttpHeaders.BceContentSha256] = metadata.BceContentSha256;
            }
            if (metadata.ETag != null)
            {
                request.Headers[BceConstants.HttpHeaders.ETag] = metadata.ETag;
            }

            IDictionary<String, String> userMetadata = metadata.UserMetadata;
            if (userMetadata != null)
            {
                foreach (var entry in userMetadata)
                {
                    String key = entry.Key;
                    if (key == null)
                    {
                        continue;
                    }
                    String value = entry.Value;
                    if (value == null)
                    {
                        value = "";
                    }
                    if (key.Length + value.Length > 1024 * 32)
                    {
                        throw new BceClientException("MetadataTooLarge");
                    }
                    string userMetaKey = BceConstants.HttpHeaders.BceUserMetadataPrefix +
                                         HttpUtils.Normalize(key.Trim());
                    request.Headers[userMetaKey] = HttpUtils.Normalize(value);
                }
            }
        }

        private InternalRequest CreateInternalRequest(string httpMethod, BceRequestBase request)
        {
            string bucketName = null;
            string key = null;
            if (request is BucketRequestBase)
            {
                bucketName = (request as BucketRequestBase).BucketName;
            }
            if (request is ObjectRequestBase)
            {
                key = (request as ObjectRequestBase).Key;
            }
            var internalRequest = new InternalRequest();
            var config = this.config.Merge(request.Config);
            internalRequest.Config = config;
            internalRequest.Uri = new Uri(
                HttpUtils.AppendUri(this.ComputeEndpoint(config), UrlPrefix, bucketName, key));
            internalRequest.HttpMethod = httpMethod;
            internalRequest.Headers[BceConstants.HttpHeaders.BceDate] =
                DateUtils.FormatAlternateIso8601Date(DateTime.Now);
            internalRequest.Headers[BceConstants.HttpHeaders.Host] = HttpUtils.GenerateHostHeader(internalRequest.Uri);
            return internalRequest;
        }

        private InternalRequest CreateInternalRequestForGetObject(GetObjectRequest request)
        {
            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            long[] range = request.Range;
            if (range != null && range.Length == 2)
            {
                internalRequest.Range = request.Range;
                //internalRequest.Headers[BceConstants.HttpHeaders.Range] = "bytes=" + range[0] + "-" + range[1];
            }
            return internalRequest;
        }

        #endregion
    }
}