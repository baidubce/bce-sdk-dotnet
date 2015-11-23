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
using System.Web.UI.WebControls;
using BaiduBce.Auth;
using log4net;
using BaiduBce.Internal;
using BaiduBce.Services.Bos.Model;
using BaiduBce.Model;
using BaiduBce.Util;
using Newtonsoft.Json;

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

        public User GetBosAccountOwner()
        {
            return ListBuckets().Owner;
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

        public void DeleteBucket(string bucketName)
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

        public bool DoesBucketExist(string bucketName)
        {
            return this.DoesBucketExist(new DoesBucketExistRequest() {BucketName = bucketName});
        }

        public bool DoesBucketExist(DoesBucketExistRequest request)
        {
            CheckNotNull(request, "request should not be null.");
            try
            {
                var internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Delete, request);
                return internalRequest.Config.RetryPolicy.Execute<bool>(attempt =>
                {
                    var httpWebResponse = this.httpClient.Execute(internalRequest);
                    using (httpWebResponse)
                    {
                        return true;
                    }
                });
            }
            catch (BceServiceException e)
            {
                // Forbidden means that the bucket exists.
                if (e.StatusCode == BceConstants.HttpStatusCode.Forbidden)
                {
                    return true;
                }
                if (e.StatusCode == BceConstants.HttpStatusCode.Forbidden)
                {
                    return false;
                }
                throw e;
            }
        }

        public GetBucketAclResponse GetBucketAcl(string bucketName)
        {
            return this.GetBucketAcl(new BucketRequestBase() {BucketName = bucketName});
        }

        public GetBucketAclResponse GetBucketAcl(BucketRequestBase request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            internalRequest.Parameters["acl"] = null;

            GetBucketAclResponse response = internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<GetBucketAclResponse>(httpWebResponse);
                }
            });
            if (response.Version > GetBucketAclResponse.MaxSupportedAclVersion)
            {
                throw new BceClientException("Unsupported acl version.");
            }
            return response;
        }

        public void SetBucketAcl(string bucketName, string acl)
        {
            this.SetBucketAcl(new SetBucketAclRequest() {BucketName = bucketName, CannedAcl = acl});
        }

        public void SetBucketAcl(SetBucketAclRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            var internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Put, request);
            internalRequest.Parameters["acl"] = null;

            if (!string.IsNullOrEmpty(request.CannedAcl))
            {
                internalRequest.Headers[BceConstants.HttpHeaders.BceAcl] = request.CannedAcl;
                internalRequest.Headers[BceConstants.HttpHeaders.ContentLength] = "0";
            }
            else if (request.AccessControlList != null)
            {
                AccessControlListRequest accessControlListRequest = new AccessControlListRequest();
                accessControlListRequest.AccessControlList = request.AccessControlList;
                string json = JsonUtils.SerializeObject(accessControlListRequest);
                FillRequestBodyForJson(internalRequest, json);
            }
            else
            {
                CheckNotNull(null, "request.acl should not be null.");
            }

            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<BosResponseBase>(httpWebResponse);
                }
            });
        }

        public Uri GeneratePresignedUrl(string bucketName, string key, int expirationInSeconds)
        {
            return this.GeneratePresignedUrl(bucketName, key, expirationInSeconds, BceConstants.HttpMethod.Get);
        }


        public Uri GeneratePresignedUrl(string bucketName, string key, int expirationInSeconds, string method)
        {
            GeneratePresignedUrlRequest request = new GeneratePresignedUrlRequest()
            {
                BucketName = bucketName,
                Key = key,
                Method = method,
                ExpirationInSeconds = expirationInSeconds
            };
            return this.GeneratePresignedUrl(request);
        }

        public Uri GeneratePresignedUrl(GeneratePresignedUrlRequest request)
        {
            CheckNotNull(request, "The request parameter must be specified when generating a pre-signed URL");

            string httpMethod = request.Method;

            // If the key starts with a slash character itself, the following method
            // will actually add another slash before the resource path to prevent
            // the HttpClient mistakenly treating the slash as a path delimiter.
            // For presigned request, we need to remember to remove this extra slash
            // before generating the URL.
            var internalRequest = new InternalRequest();
            var config = this.config.Merge(request.Config);
            config.SignOptions.ExpirationInSeconds = request.ExpirationInSeconds;
            internalRequest.Config = config;
            internalRequest.Uri = new Uri(
                HttpUtils.AppendUri(this.ComputeEndpoint(config), UrlPrefix, request.BucketName, request.Key));
            internalRequest.HttpMethod = httpMethod;
            internalRequest.Headers[BceConstants.HttpHeaders.Host] = HttpUtils.GenerateHostHeader(internalRequest.Uri);
            SignOptions options = new SignOptions();

            foreach (var entry in request.RequestHeaders)
            {
                internalRequest.Headers[entry.Key] = entry.Value ?? "";
            }
            foreach (var entry in request.RequestParameters)
            {
                internalRequest.Parameters[entry.Key] = entry.Value ?? "";
            }
            if (request.ContentType != null)
            {
                internalRequest.Headers[BceConstants.HttpHeaders.ContentType] = request.ContentType;
            }
            if (request.ContentMd5 != null)
            {
                internalRequest.Headers[BceConstants.HttpHeaders.ContentMd5] = request.ContentMd5;
            }

            internalRequest.Headers[BceConstants.HttpHeaders.Authorization] = config.Signer.Sign(internalRequest);

            // Remove the leading slash (if any) in the resource-path
            return ConvertRequestToUri(internalRequest);
        }

        public ListObjectsResponse ListObjects(string bucketName)
        {
            return this.ListObjects(new ListObjectsRequest() {BucketName = bucketName});
        }

        public ListObjectsResponse ListObjects(string bucketName, string prefix)
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

        public PutObjectResponse PutObject(string bucketName, string key, FileInfo fileInfo)
        {
            return this.PutObject(new PutObjectRequest() {BucketName = bucketName, Key = key, FileInfo = fileInfo});
        }

        public PutObjectResponse PutObject(string bucketName, string key, FileInfo fileInfo, ObjectMetadata metadata)
        {
            return this.PutObject(new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
                FileInfo = fileInfo,
                ObjectMetadata = metadata
            });
        }

        public PutObjectResponse PutObject(string bucketName, string key, string value)
        {
            return this.PutObject(bucketName, key, value, new ObjectMetadata());
        }

        public PutObjectResponse PutObject(string bucketName, string key, string value, ObjectMetadata metadata)
        {
            return this.PutObject(bucketName, key, Encoding.UTF8.GetBytes(value), metadata);
        }

        public PutObjectResponse PutObject(string bucketName, string key, byte[] value)
        {
            return this.PutObject(bucketName, key, value, new ObjectMetadata());
        }

        public PutObjectResponse PutObject(string bucketName, string key, byte[] value, ObjectMetadata metadata)
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

        public PutObjectResponse PutObject(string bucketName, string key, Stream input)
        {
            return this.PutObject(bucketName, key, input, new ObjectMetadata());
        }

        public PutObjectResponse PutObject(string bucketName, string key, Stream input, ObjectMetadata metadata)
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
            if (string.IsNullOrEmpty(request.Key))
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

            if (internalRequest.Content.CanSeek && internalRequest.Content.Position != 0)
            {
                throw new ArgumentException("input stream position should be 0");
            }

            internalRequest.Headers[BceConstants.HttpHeaders.ContentLength] = metadata.ContentLength.ToString();
            PopulateRequestMetadata(internalRequest, metadata);

            using (internalRequest.Content)
            {
                internalRequest.Config.RetryPolicy.CanRetry = internalRequest.Content.CanSeek;
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
        public BosObject GetObject(string bucketName, string key)
        {
            return this.GetObject(new GetObjectRequest() {BucketName = bucketName, Key = key});
        }

        public ObjectMetadata GetObject(string bucketName, string key, FileInfo destinationFile)
        {
            return this.GetObject(new GetObjectRequest() {BucketName = bucketName, Key = key}, destinationFile);
        }

        public byte[] GetObjectContent(string bucketName, string key)
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

        public ObjectMetadata GetObjectMetadata(string bucketName, string key)
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

        public CopyObjectResponse CopyObject(
            string sourceBucketName,
            string sourceKey,
            string destinationBucketName,
            string destinationKey)
        {
            return this.CopyObject(new CopyObjectRequest()
            {
                BucketName = destinationBucketName,
                SourceBucketName = sourceBucketName,
                Key = destinationKey,
                SourceKey = sourceKey
            });
        }

        public CopyObjectResponse CopyObject(CopyObjectRequest request)
        {
            CheckNotNull(request, "request should not be null.");
            if (string.IsNullOrEmpty(request.SourceKey))
            {
                throw new ArgumentException("object key should not be null or empty");
            }

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Put, request);
            string copySourceHeader = "/" + request.SourceBucketName + "/" + request.SourceKey;
            copySourceHeader = HttpUtils.NormalizePath(copySourceHeader);
            internalRequest.Headers[BceConstants.HttpHeaders.BceCopySource] = copySourceHeader;
            if (!string.IsNullOrEmpty(request.ETag))
            {
                internalRequest.Headers[BceConstants.HttpHeaders.BceCopySourceIfMatch] = "\"" + request.ETag + "\"";
            }
            ObjectMetadata newObjectMetadata = request.NewObjectMetadata;
            if (newObjectMetadata != null)
            {
                internalRequest.Headers[BceConstants.HttpHeaders.BceCopyMetadataDirective] = "replace";
                PopulateRequestMetadata(internalRequest, newObjectMetadata);
            }
            else
            {
                internalRequest.Headers[BceConstants.HttpHeaders.BceCopyMetadataDirective] = "copy";
            }
            internalRequest.Headers[BceConstants.HttpHeaders.ContentLength] = "0";

            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<CopyObjectResponse>(httpWebResponse);
                }
            });
        }

        public void DeleteObject(string bucketName, string key)
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

        public InitiateMultipartUploadResponse InitiateMultipartUpload(string bucketName, string key)
        {
            return this.InitiateMultipartUpload(new InitiateMultipartUploadRequest()
            {
                BucketName = bucketName,
                Key = key
            });
        }

        public InitiateMultipartUploadResponse InitiateMultipartUpload(InitiateMultipartUploadRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Post, request);
            internalRequest.Parameters["uploads"] = null;
            internalRequest.Headers[BceConstants.HttpHeaders.ContentLength] = "0";
            if (request.ObjectMetadata != null)
            {
                PopulateRequestMetadata(internalRequest, request.ObjectMetadata);
            }

            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<InitiateMultipartUploadResponse>(httpWebResponse);
                }
            });
        }

        public UploadPartResponse UploadPart(UploadPartRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            if (request.PartSize > 5 * 1024 * 1024 * 1024L)
            {
                throw new BceClientException("PartNumber " + request.PartNumber
                                             + " : Part Size should not be more than 5GB.");
            }

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Put, request);
            internalRequest.Parameters["uploadId"] = request.UploadId;
            internalRequest.Parameters["partNumber"] = request.PartNumber.ToString();
            internalRequest.Headers[BceConstants.HttpHeaders.ContentLength] = request.PartSize.ToString();

            Stream input = request.InputStream;
            if (input.CanSeek)
            {
                if (input.Position != 0)
                {
                    throw new ArgumentException("input stream position should be 0");
                }
                if (string.IsNullOrEmpty(request.Md5Digest))
                {
                    request.Md5Digest = HashUtils.ComputeMD5Hash(input);
                }
            }

            using (input)
            {
                internalRequest.Content = input;
                internalRequest.Config.RetryPolicy.CanRetry = internalRequest.Content.CanSeek;
                string eTag = internalRequest.Config.RetryPolicy.Execute(attempt =>
                {
                    var httpWebResponse = this.httpClient.Execute(internalRequest);
                    using (httpWebResponse)
                    {
                        return httpWebResponse.Headers[BceConstants.HttpHeaders.ETag].Replace("\"", "");
                    }
                });

                if (!eTag.Equals(request.Md5Digest, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new BceClientException("Unable to verify integrity of data upload.  "
                                                 + "Client calculated content hash didn't match hash calculated by "
                                                 + "Baidu BOS.  " +
                                                 "You may need to delete the data stored in Baiddu BOS.");
                }

                var result = new UploadPartResponse
                {
                    ETag = eTag,
                    PartNumber = request.PartNumber
                };
                return result;
            }
        }

        public ListPartsResponse ListParts(string bucketName, string key, string uploadId)
        {
            return ListParts(new ListPartsRequest()
            {
                BucketName = bucketName,
                Key = key,
                UploadId = uploadId
            });
        }


        public ListPartsResponse ListParts(ListPartsRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            internalRequest.Parameters["uploadId"] = request.UploadId;
            if (request.MaxParts != null)
            {
                internalRequest.Parameters["maxParts"] = request.MaxParts.ToString();
            }
            internalRequest.Parameters["partNumberMarker"] = request.PartNumberMarker.ToString();

            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    ListPartsResponse listPartsResponse = ToObject<ListPartsResponse>(httpWebResponse);
                    listPartsResponse.BucketName = request.BucketName;
                    return listPartsResponse;
                }
            });
        }

        public CompleteMultipartUploadResponse CompleteMultipartUpload(string bucketName, string key, string uploadId,
            List<PartETag> partETags)
        {
            return CompleteMultipartUpload(new CompleteMultipartUploadRequest()
            {
                BucketName = bucketName,
                Key = key,
                UploadId = uploadId,
                PartETags = partETags
            });
        }

        public CompleteMultipartUploadResponse CompleteMultipartUpload(string bucketName, string key, string uploadId,
            List<PartETag> partETags, ObjectMetadata metadata)
        {
            return CompleteMultipartUpload(new CompleteMultipartUploadRequest()
            {
                BucketName = bucketName,
                Key = key,
                UploadId = uploadId,
                PartETags = partETags,
                ObjectMetadata = metadata
            });
        }

        public CompleteMultipartUploadResponse CompleteMultipartUpload(CompleteMultipartUploadRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Post, request);
            internalRequest.Parameters["uploadId"] = request.UploadId;
            ObjectMetadata metadata = request.ObjectMetadata;
            if (metadata != null)
            {
                PopulateRequestMetadata(internalRequest, metadata);
            }
            PartETags partETags = new PartETags() {Parts = request.PartETags};
            string json = JsonUtils.SerializeObject(partETags);
            FillRequestBodyForJson(internalRequest, json);
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    var completeMultipartUploadResponse = ToObject<CompleteMultipartUploadResponse>(httpWebResponse);
                    completeMultipartUploadResponse.BucketName = request.BucketName;
                    return completeMultipartUploadResponse;
                }
            });
        }

        public void AbortMultipartUpload(string bucketName, string key, string uploadId)
        {
            AbortMultipartUpload(new AbortMultipartUploadRequest()
            {
                BucketName = bucketName,
                Key = key,
                UploadId = uploadId
            });
        }

        public void AbortMultipartUpload(AbortMultipartUploadRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Delete, request);
            internalRequest.Parameters["uploadId"] = request.UploadId;

            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<BosResponseBase>(httpWebResponse);
                }
            });
        }

        public ListMultipartUploadsResponse ListMultipartUploads(string bucketName)
        {
            return ListMultipartUploads(new ListMultipartUploadsRequest() {BucketName = bucketName});
        }

        public ListMultipartUploadsResponse ListMultipartUploads(ListMultipartUploadsRequest request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            internalRequest.Parameters["uploads"] = null;
            if (!string.IsNullOrEmpty(request.KeyMarker))
            {
                internalRequest.Parameters["keyMarker"] = request.KeyMarker;
            }
            if (!string.IsNullOrEmpty(request.Delimiter))
            {
                internalRequest.Parameters["delimiter"] = request.Delimiter;
            }
            if (!string.IsNullOrEmpty(request.Prefix))
            {
                internalRequest.Parameters["prefix"] = request.Prefix;
            }
            if (request.MaxUploads != null)
            {
                internalRequest.Parameters["maxUploads"] = request.MaxUploads.ToString();
            }

            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    var listMultipartUploadsResponse = ToObject<ListMultipartUploadsResponse>(httpWebResponse);
                    listMultipartUploadsResponse.BucketName = request.BucketName;
                    return listMultipartUploadsResponse;
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
            string eTag = httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ETag);
            if (eTag != null)
            {
                objectMetadata.ETag = eTag.Replace("\"", "");
            }
            string contentRange = httpWebResponse.GetResponseHeader(BceConstants.HttpHeaders.ContentRange);
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
            foreach (string header in httpWebResponse.Headers.AllKeys)
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

            IDictionary<string, string> userMetadata = metadata.UserMetadata;
            if (userMetadata != null)
            {
                foreach (var entry in userMetadata)
                {
                    string key = entry.Key;
                    if (key == null)
                    {
                        continue;
                    }
                    string value = entry.Value;
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
            var timestamp = config.SignOptions.Timestamp;
            if (timestamp == DateTime.MinValue)
            {
                timestamp = DateTime.Now;
            }
            internalRequest.Headers[BceConstants.HttpHeaders.BceDate] =
                DateUtils.FormatAlternateIso8601Date(timestamp);
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

        private Uri ConvertRequestToUri(InternalRequest request)
        {
            string resourcePath = HttpUtils.NormalizePath(request.Uri.AbsolutePath);

            // Removed the padding "/" that was already added into the request's resource path.
            if (resourcePath.StartsWith("/"))
            {
                resourcePath = resourcePath.Substring(1);
            }

            // Some http client libraries (e.g. Apache HttpClient) cannot handle
            // consecutive "/"s between URL authority and path components.
            // So we escape "////..." into "/%2F%2F%2F...", in the same way as how
            // we treat consecutive "/"s in AmazonS3Client#presignRequest(...)
            string urlPath = "/" + resourcePath;
            urlPath = urlPath.Replace("(?<=/)/", "%2F");
            string urlstring = this.ComputeEndpoint(request.Config) + urlPath;

            bool firstParam = true;
            foreach (var entry in request.Parameters)
            {
                if (firstParam)
                {
                    urlstring += "?";
                    firstParam = false;
                }
                else
                {
                    urlstring += "&";
                }
                urlstring += entry.Key + "=" + HttpUtils.Normalize(entry.Value);
            }

            string authorization = request.Headers[BceConstants.HttpHeaders.Authorization];
            if (authorization != null)
            {
                if (firstParam)
                {
                    urlstring += "?";
                }
                else
                {
                    urlstring += "&";
                }
                urlstring += "authorization" + "=" + HttpUtils.Normalize(authorization);
            }

            try
            {
                return new Uri(urlstring);
            }
            catch (UriFormatException e)
            {
                throw new BceClientException("Unable to convert request to well formed URL: " + e.Message, e);
            }
        }

        private void FillRequestBodyForJson(InternalRequest internalRequest, string json)
        {
            internalRequest.Headers[BceConstants.HttpHeaders.ContentLength] = json.Length.ToString();
            internalRequest.Headers[BceConstants.HttpHeaders.ContentType] = "application/json";
            internalRequest.Content = new MemoryStream(Encoding.Default.GetBytes(json));
        }


        #endregion
    }
}