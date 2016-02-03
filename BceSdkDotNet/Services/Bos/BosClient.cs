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

    /// <summary>
    /// Provides the client for accessing the Baidu Object Service.
    /// </summary>
    public class BosClient : BceClientBase
    {
        private const string UrlPrefix = "/v1";
        private const string serviceEndpointFormat = "%s://%s.bcebos.com";

        private ILog logger = LogManager.GetLogger(typeof (BosClient));

        /// <summary>
        /// Constructs a new client to invoke service methods on Bos.
        /// </summary>
        public BosClient()
            : this(new BceClientConfiguration())
        {
        }

        /// <summary>
        /// Constructs a new Bos client using the client configuration to access Bos.
        /// </summary>
        /// <param name="config"> The bos client configuration options controlling how this client
        ///                            connects to Bos (e.g. retry counts, etc). </param>
        public BosClient(BceClientConfiguration config)
            : base(config, serviceEndpointFormat)
        {
        }

        /// <summary>
        /// Gets the current owner of the Bos account that the authenticated sender of the request is using.
        /// 
        /// <para>
        /// The caller <i>must</i> authenticate with a valid BCE Access Key ID that is registered with Bos.
        /// 
        /// </para>
        /// </summary>
        /// <returns> The account of the authenticated sender </returns>
        public User GetBosAccountOwner()
        {
            return ListBuckets().Owner;
        }

        /// <summary>
        /// Creates a new Bos bucket with the specified name.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket to create.
        ///     All buckets in Bos share a single namespace; ensure the bucket is given a unique name. </param>
        /// <returns> The newly created bucket. </returns>
        public CreateBucketResponse CreateBucket(string bucketName)
        {
            return this.CreateBucket(new CreateBucketRequest() {BucketName = bucketName});
        }

        /// <summary>
        /// Creates a new Bos bucket with the specified name.
        /// </summary>
        /// <param name="request"> The request object containing all options for creating a Bos bucket. </param>
        /// <returns> The newly created bucket. </returns>
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

        /// <summary>
        /// Returns a list of all Bos buckets that the authenticated sender of the request owns.
        /// 
        /// <para>
        /// Users must authenticate with a valid BCE Access Key ID that is registered
        /// with Bos. Anonymous requests cannot list buckets, and users cannot
        /// list buckets that they did not create.
        /// 
        /// </para>
        /// </summary>
        /// <returns> All of the Bos buckets owned by the authenticated sender of the request. </returns>
        public ListBucketsResponse ListBuckets()
        {
            return ListBuckets(new ListBucketsRequest());
        }

        /// <summary>
        /// Returns a list of all Bos buckets that the authenticated sender of the request owns.
        /// 
        /// <para>
        /// Users must authenticate with a valid BCE Access Key ID that is registered
        /// with Bos. Anonymous requests cannot list buckets, and users cannot
        /// list buckets that they did not create.
        /// 
        /// </para>
        /// </summary>
        /// <param name="request"> The request containing all of the options related to the listing of buckets. </param>
        /// <returns> All of the Bos buckets owned by the authenticated sender of the request. </returns>
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

        /// <summary>
        /// Deletes the specified bucket. All objects in the bucket must be deleted before the bucket itself
        /// can be deleted.
        /// 
        /// <para>
        /// Only the owner of a bucket can delete it, regardless of the bucket's access control policy.
        /// 
        /// </para>
        /// </summary>
        /// <param name="bucketName"> The name of the bucket to delete. </param>
        public void DeleteBucket(string bucketName)
        {
            this.DeleteBucket(new DeleteBucketRequest() {BucketName = bucketName});
        }

        /// <summary>
        /// Deletes the specified bucket. All objects in the bucket must be deleted before the bucket itself
        /// can be deleted.
        /// 
        /// <para>
        /// Only the owner of a bucket can delete it, regardless of the bucket's access control policy.
        /// 
        /// </para>
        /// </summary>
        /// <param name="request"> The request object containing all options for deleting a Bos bucket. </param>
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

        /// <summary>
        /// Checks if the specified bucket exists. Bos buckets are named in a
        /// global namespace; use this method to determine if a specified bucket name
        /// already exists, and therefore can't be used to create a new bucket.
        /// 
        /// <para>
        /// If invalid security credentials are used to execute this method, the
        /// client is not able to distinguish between bucket permission errors and
        /// invalid credential errors, and this method could return an incorrect
        /// result.
        /// 
        /// </para>
        /// </summary>
        /// <param name="bucketName"> The name of the bucket to check. </param>
        /// <returns> The value <code>true</code> if the specified bucket exists in Bos;
        ///     the value <code>false</code> if there is no bucket in Bos with that name. </returns>
        public bool DoesBucketExist(string bucketName)
        {
            return this.DoesBucketExist(new DoesBucketExistRequest() {BucketName = bucketName});
        }

        /// <summary>
        /// Checks if the specified bucket exists. Bos buckets are named in a
        /// global namespace; use this method to determine if a specified bucket name
        /// already exists, and therefore can't be used to create a new bucket.
        /// 
        /// <para>
        /// If invalid security credentials are used to execute this method, the
        /// client is not able to distinguish between bucket permission errors and
        /// invalid credential errors, and this method could return an incorrect
        /// result.
        /// 
        /// </para>
        /// </summary>
        /// <param name="request"> The request object containing all options for checking a Bos bucket. </param>
        /// <returns> The value <code>true</code> if the specified bucket exists in Bos;
        ///     the value <code>false</code> if there is no bucket in Bos with that name. </returns>
        public bool DoesBucketExist(DoesBucketExistRequest request)
        {
            CheckNotNull(request, "request should not be null.");
            try
            {
                var internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Head, request);
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
                if (e.StatusCode == BceConstants.HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw e;
            }
        }

        /// <summary>
        /// Gets the ACL for the specified Bos bucket.
        /// 
        /// <para>
        /// Each bucket and object in Bos has an ACL that defines its access
        /// control policy. When a request is made, Bos authenticates the
        /// request using its standard authentication procedure and then checks the
        /// ACL to verify the sender was granted access to the bucket or object. If
        /// the sender is approved, the request proceeds. Otherwise, Bos
        /// returns an error.
        /// 
        /// </para>
        /// </summary>
        /// <param name="bucketName"> The name of the bucket whose ACL is being retrieved. </param>
        /// <returns> The <code>GetBuckeetAclResponse</code> for the specified Bos bucket. </returns>
        public GetBucketAclResponse GetBucketAcl(string bucketName)
        {
            return this.GetBucketAcl(new BucketRequestBase() {BucketName = bucketName});
        }

        /// <summary>
        /// Gets the ACL for the specified Bos bucket.
        /// 
        /// <para>
        /// Each bucket and object in Bos has an ACL that defines its access
        /// control policy. When a request is made, Bos authenticates the
        /// request using its standard authentication procedure and then checks the
        /// ACL to verify the sender was granted access to the bucket or object. If
        /// the sender is approved, the request proceeds. Otherwise, Bos
        /// returns an error.
        /// 
        /// </para>
        /// </summary>
        /// <param name="request"> The request containing the name of the bucket whose ACL is being retrieved. </param>
        /// <returns> The <code>GetBuckeetAclResponse</code> for the specified Bos bucket. </returns>
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

        /// <summary>
        /// Sets the CannedAccessControlList for the specified Bos bucket using one of
        /// the pre-configured <code>CannedAccessControlLists</code>.
        /// 
        /// <para>
        /// A <code>CannedAccessControlList</code>
        /// provides a quick way to configure an object or bucket with commonly used
        /// access control policies.
        /// 
        /// </para>
        /// </summary>
        /// <param name="bucketName"> The name of the bucket whose ACL is being set. </param>
        /// <param name="acl"> The pre-configured <code>CannedAccessControlLists</code> to set for the specified bucket. </param>
        public void SetBucketAcl(string bucketName, string acl)
        {
            this.SetBucketAcl(new SetBucketAclRequest() {BucketName = bucketName, CannedAcl = acl});
        }

        /// <summary>
        /// Sets the Acl for the specified Bos bucket.
        /// </summary>
        /// <param name="request"> The request object containing the bucket to modify and the ACL to set. </param>
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

        /// <summary>
        /// Gets the Location for the specified Bos bucket.
        /// 
        /// <para>
        /// Each bucket and object in Bos has an Location that defines its location
        /// 
        /// </para>
        /// </summary>
        /// <param name="bucketName"> The name of the bucket whose Location is being retrieved. </param>
        /// <returns> The <code>GetBuckeetLocationResponse</code> for the specified Bos bucket. </returns>
        public GetBucketLocationResponse GetBucketLocation(string bucketName)
        {
            return this.GetBucketLocation(new BucketRequestBase() {BucketName = bucketName});
        }

        /// <summary>
        /// Gets the Location for the specified Bos bucket.
        /// 
        /// <para>
        /// Each bucket and object in Bos has an Location that defines its location
        /// 
        /// </para>
        /// </summary>
        /// <param name="request"> The request containing the name of the bucket whose Location is being retrieved. </param>
        /// <returns> The <code>GetBuckeetLocationResponse</code> for the specified Bos bucket. </returns>
        public GetBucketLocationResponse GetBucketLocation(BucketRequestBase request)
        {
            CheckNotNull(request, "request should not be null.");

            InternalRequest internalRequest = this.CreateInternalRequest(BceConstants.HttpMethod.Get, request);
            internalRequest.Parameters["location"] = null;

            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return ToObject<GetBucketLocationResponse>(httpWebResponse);
                }
            });
        }

        /// <summary>
        /// Returns a pre-signed URL for accessing a Bos resource.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the desired object. </param>
        /// <param name="key"> The key in the specified bucket under which the desired object is stored. </param>
        /// <param name="expirationInSeconds"> The expiration after which the returned pre-signed URL will expire. </param>
        /// <returns> A pre-signed URL which expires at the specified time, and can be
        ///     used to allow anyone to download the specified object from Bos,
        ///     without exposing the owner's Bce secret access key. </returns>
        public Uri GeneratePresignedUrl(string bucketName, string key, int expirationInSeconds)
        {
            return this.GeneratePresignedUrl(bucketName, key, expirationInSeconds, BceConstants.HttpMethod.Get);
        }

        /// <summary>
        /// Returns a pre-signed URL for accessing a Bos resource.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the desired object. </param>
        /// <param name="key"> The key in the specified bucket under which the desired object is stored. </param>
        /// <param name="expirationInSeconds"> The expiration after which the returned pre-signed URL will expire. </param>
        /// <param name="method">     The HTTP method verb to use for this URL </param>
        /// <returns> A pre-signed URL which expires at the specified time, and can be
        ///     used to allow anyone to download the specified object from Bos,
        ///     without exposing the owner's Bce secret access key. </returns>
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

        /// <summary>
        /// Returns a pre-signed URL for accessing a Bos resource.
        /// </summary>
        /// <param name="request"> The request object containing all the options for generating a
        ///     pre-signed URL (bucket name, key, expiration date, etc). </param>
        /// <returns> A pre-signed URL which expires at the specified time, and can be
        ///     used to allow anyone to download the specified object from Bos,
        ///     without exposing the owner's Bce secret access key. </returns>
        public Uri GeneratePresignedUrl(GeneratePresignedUrlRequest request)
        {
            CheckNotNull(request, "The request parameter must be specified when generating a pre-signed URL");

            string httpMethod = request.Method;
            if (!BceConstants.HttpMethod.Get.Equals(httpMethod, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("only support http method get");
            }
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

        /// <summary>
        /// Returns ListObjectsResponse containing a list of summary information about the objects in the specified buckets.
        /// List results are <i>always</i> returned in lexicographic (alphabetical) order.
        /// </summary>
        /// <param name="bucketName"> The name of the Bos bucket to list. </param>
        /// <returns> ListObjectsResponse containing a listing of the objects in the specified bucket, along with any
        ///     other associated information, such as common prefixes (if a delimiter was specified), the original
        ///     request parameters, etc. </returns>
        public ListObjectsResponse ListObjects(string bucketName)
        {
            return this.ListObjects(new ListObjectsRequest() {BucketName = bucketName});
        }

        /// <summary>
        /// Returns ListObjectsResponse containing a list of summary information about the objects in the specified buckets.
        /// List results are <i>always</i> returned in lexicographic (alphabetical) order.
        /// </summary>
        /// <param name="bucketName"> The name of the Bos bucket to list. </param>
        /// <param name="prefix"> An optional parameter restricting the response to keys beginning with the specified prefix.
        ///     Use prefixes to separate a bucket into different sets of keys, similar to how a file system
        ///     organizes files into directories. </param>
        /// <returns> ListObjectsResponse containing a listing of the objects in the specified bucket, along with any
        ///     other associated information, such as common prefixes (if a delimiter was specified), the original
        ///     request parameters, etc. </returns>
        public ListObjectsResponse ListObjects(string bucketName, string prefix)
        {
            return this.ListObjects(new ListObjectsRequest() {BucketName = bucketName, Prefix = prefix});
        }

        /// <summary>
        /// Returns ListObjectsResponse containing a list of summary information about the objects in the specified buckets.
        /// List results are <i>always</i> returned in lexicographic (alphabetical) order.
        /// </summary>
        /// <param name="request"> The request object containing all options for listing the objects in a specified bucket. </param>
        /// <returns> ListObjectsResponse containing a listing of the objects in the specified bucket, along with any
        ///     other associated information, such as common prefixes (if a delimiter was specified), the original
        ///     request parameters, etc. </returns>
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

        /// <summary>
        /// Provides an easy way to continue a truncated object listing and retrieve the next page of results.
        /// </summary>
        /// <param name="previousResponse"> The previous truncated <code>ListObjectsResponse</code>. If a non-truncated
        ///     <code>ListObjectsResponse</code> is passed in, an empty <code>ListObjectsResponse</code>
        ///     is returned without ever contacting Bos. </param>
        /// <returns> The next set of <code>ListObjectsResponse</code> results, beginning immediately
        ///     after the last result in the specified previous <code>ListObjectsResponse</code>. </returns>
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

        /// <summary>
        /// Uploads the specified file to Bos under the specified bucket and key name.
        /// </summary>
        /// <param name="bucketName"> The name of an existing bucket, to which you have Write permission. </param>
        /// <param name="key"> The key under which to store the specified file. </param>
        /// <param name="fileInfo"> The file containing the data to be uploaded to Bos. </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
        public PutObjectResponse PutObject(string bucketName, string key, FileInfo fileInfo)
        {
            return this.PutObject(new PutObjectRequest() {BucketName = bucketName, Key = key, FileInfo = fileInfo});
        }

        /// <summary>
        /// Uploads the specified file and object metadata to Bos under the specified bucket and key name.
        /// </summary>
        /// <param name="bucketName"> The name of an existing bucket, to which you have Write permission. </param>
        /// <param name="key"> The key under which to store the specified file. </param>
        /// <param name="fileInfo"> The file containing the data to be uploaded to Bos. </param>
        /// <param name="metadata"> Additional metadata instructing Bos how to handle the uploaded data
        ///     (e.g. custom user metadata, hooks for specifying content type, etc.). </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
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

        /// <summary>
        /// Uploads the specified string to Bos under the specified bucket and key name.
        /// </summary>
        /// <param name="bucketName"> The name of an existing bucket, to which you have Write permission. </param>
        /// <param name="key"> The key under which to store the specified file. </param>
        /// <param name="value"> The string containing the value to be uploaded to Bos. </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
        public PutObjectResponse PutObject(string bucketName, string key, string value)
        {
            return this.PutObject(bucketName, key, value, new ObjectMetadata());
        }

        /// <summary>
        /// Uploads the specified string and object metadata to Bos under the specified bucket and key name.
        /// </summary>
        /// <param name="bucketName"> The name of an existing bucket, to which you have Write permission. </param>
        /// <param name="key"> The key under which to store the specified file. </param>
        /// <param name="value"> The string containing the value to be uploaded to Bos. </param>
        /// <param name="metadata"> Additional metadata instructing Bos how to handle the uploaded data
        ///     (e.g. custom user metadata, hooks for specifying content type, etc.). </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
        public PutObjectResponse PutObject(string bucketName, string key, string value, ObjectMetadata metadata)
        {
            return this.PutObject(bucketName, key, Encoding.UTF8.GetBytes(value), metadata);
        }

        /// <summary>
        /// Uploads the specified bytes to Bos under the specified bucket and key name.
        /// </summary>
        /// <param name="bucketName"> The name of an existing bucket, to which you have Write permission. </param>
        /// <param name="key"> The key under which to store the specified file. </param>
        /// <param name="value"> The bytes containing the value to be uploaded to Bos. </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
        public PutObjectResponse PutObject(string bucketName, string key, byte[] value)
        {
            return this.PutObject(bucketName, key, value, new ObjectMetadata());
        }

        /// <summary>
        /// Uploads the specified bytes and object metadata to Bos under the specified bucket and key name.
        /// </summary>
        /// <param name="bucketName"> The name of an existing bucket, to which you have Write permission. </param>
        /// <param name="key"> The key under which to store the specified file. </param>
        /// <param name="value"> The bytes containing the value to be uploaded to Bos. </param>
        /// <param name="metadata"> Additional metadata instructing Bos how to handle the uploaded data
        ///     (e.g. custom user metadata, hooks for specifying content type, etc.). </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
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

        /// <summary>
        /// Uploads the specified input stream to Bos under the specified bucket and key name.
        /// </summary>
        /// <param name="bucketName"> The name of an existing bucket, to which you have Write permission. </param>
        /// <param name="key"> The key under which to store the specified file. </param>
        /// <param name="input"> The input stream containing the value to be uploaded to Bos. </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
        public PutObjectResponse PutObject(string bucketName, string key, Stream input)
        {
            return this.PutObject(bucketName, key, input, new ObjectMetadata());
        }

        /// <summary>
        /// Uploads the specified input stream and object metadata to Bos under the specified bucket and key name.
        /// </summary>
        /// <param name="bucketName"> The name of an existing bucket, to which you have Write permission. </param>
        /// <param name="key"> The key under which to store the specified file. </param>
        /// <param name="input"> The input stream containing the value to be uploaded to Bos. </param>
        /// <param name="metadata"> Additional metadata instructing Bos how to handle the uploaded data
        ///     (e.g. custom user metadata, hooks for specifying content type, etc.). </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
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

        /// <summary>
        /// Uploads a new object to the specified Bos bucket. The <code>PutObjectRequest</code> contains all the
        /// details of the request, including the bucket to upload to, the key the object will be uploaded under,
        /// and the file or input stream containing the data to upload.
        /// </summary>
        /// <param name="request"> The request object containing all the parameters to upload a new object to Bos. </param>
        /// <returns> A PutObjectResponse object containing the information returned by Bos for the newly created object. </returns>
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
                if (metadata.ContentLength <= 0)
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
            
            using (internalRequest.Content)
            {
                PopulateRequestMetadata(internalRequest, metadata);
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
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the desired object. </param>
        /// <param name="key"> The key under which the desired object is stored. </param>
        /// <returns> The object stored in Bos in the specified bucket and key. </returns>
        public BosObject GetObject(string bucketName, string key)
        {
            return this.GetObject(new GetObjectRequest() {BucketName = bucketName, Key = key});
        }

        /// <summary>
        /// Gets the object metadata for the object stored in Bos under the specified bucket and key,
        /// and saves the object contents to the specified file.
        /// Returns <code>null</code> if the specified constraints weren't met.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the desired object. </param>
        /// <param name="key"> The key under which the desired object is stored. </param>
        /// <param name="destinationFile"> Indicates the file (which might already exist)
        ///     where to save the object content being downloading from Bos. </param>
        /// <returns> All Bos object metadata for the specified object.
        ///     Returns <code>null</code> if constraints were specified but not met. </returns>
        public ObjectMetadata GetObject(string bucketName, string key, FileInfo destinationFile)
        {
            return this.GetObject(new GetObjectRequest() {BucketName = bucketName, Key = key}, destinationFile);
        }

        /// <summary>
        /// Gets the object content stored in Bos under the specified bucket and key.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the desired object. </param>
        /// <param name="key"> The key under which the desired object is stored. </param>
        /// <returns> The object content stored in Bos in the specified bucket and key. </returns>
        public byte[] GetObjectContent(string bucketName, string key)
        {
            return this.GetObjectContent(new GetObjectRequest() {BucketName = bucketName, Key = key});
        }

        /// <summary>
        /// Gets the object content stored in Bos under the specified bucket and key.
        /// </summary>
        /// <param name="request"> The request object containing all the options on how to download the Bos object content. </param>
        /// <returns> The object content stored in Bos in the specified bucket and key. </returns>
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
        /// </summary>
        /// <param name="request"> The request object containing all the options on how to download the object. </param>
        /// <returns> The object stored in Bos in the specified bucket and key. </returns>
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

        /// <summary>
        /// Gets the object metadata for the object stored in Bos under the specified bucket and key,
        /// and saves the object contents to the specified file.
        /// Returns <code>null</code> if the specified constraints weren't met.
        /// </summary>
        /// <param name="request"> The request object containing all the options on how to download the Bos object content. </param>
        /// <param name="destinationFileInfo"> Indicates the file (which might already exist) where to save the object
        ///     content being downloading from Bos. </param>
        /// <returns> All Bos object metadata for the specified object.
        ///     Returns <code>null</code> if constraints were specified but not met. </returns>
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

        /// <summary>
        /// Gets the metadata for the specified Bos object without actually fetching the object itself.
        /// This is useful in obtaining only the object metadata, and avoids wasting bandwidth on fetching
        /// the object data.
        /// 
        /// <para>
        /// The object metadata contains information such as content type, content disposition, etc.,
        /// as well as custom user metadata that can be associated with an object in Bos.
        /// 
        /// </para>
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the object's whose metadata is being retrieved. </param>
        /// <param name="key"> The key of the object whose metadata is being retrieved. </param>
        /// <returns> All Bos object metadata for the specified object. </returns>
        public ObjectMetadata GetObjectMetadata(string bucketName, string key)
        {
            return this.GetObjectMetadata(new ObjectRequestBase() {BucketName = bucketName, Key = key});
        }

        /// <summary>
        /// Gets the metadata for the specified Bos object without actually fetching the object itself.
        /// This is useful in obtaining only the object metadata, and avoids wasting bandwidth on fetching
        /// the object data.
        /// 
        /// <para>
        /// The object metadata contains information such as content type, content disposition, etc.,
        /// as well as custom user metadata that can be associated with an object in Bos.
        /// 
        /// </para>
        /// </summary>
        /// <param name="request"> The request object specifying the bucket, key whose metadata is being retrieved. </param>
        /// <returns> All Bos object metadata for the specified object. </returns>
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

        /// <summary>
        /// Copies a source object to a new destination in Bos.
        /// </summary>
        /// <param name="sourceBucketName"> The name of the bucket containing the source object to copy. </param>
        /// <param name="sourceKey"> The key in the source bucket under which the source object is stored. </param>
        /// <param name="destinationBucketName"> The name of the bucket in which the new object will be created.
        ///     This can be the same name as the source bucket's. </param>
        /// <param name="destinationKey"> The key in the destination bucket under which the new object will be created. </param>
        /// <returns> A CopyObjectResponse object containing the information returned by Bos for the newly created object. </returns>
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

        /// <summary>
        /// Copies a source object to a new destination in Bos.
        /// </summary>
        /// <param name="request"> The request object containing all the options for copying an Bos object. </param>
        /// <returns> A CopyObjectResponse object containing the information returned by Bos for the newly created object. </returns>
        public CopyObjectResponse CopyObject(CopyObjectRequest request)
        {
            CheckNotNull(request, "request should not be null.");
            CheckNotNull(request.SourceBucketName, "source bucket should not be null or empty");
            CheckNotNull(request.SourceKey, "source object key should not be null or empty");
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

        /// <summary>
        /// Deletes the specified object in the specified bucket.
        /// </summary>
        /// <param name="bucketName"> The name of the Bos bucket containing the object to delete. </param>
        /// <param name="key"> The key of the object to delete. </param>
        public void DeleteObject(string bucketName, string key)
        {
            this.DeleteObject(new ObjectRequestBase() {BucketName = bucketName, Key = key});
        }

        /// <summary>
        /// Deletes the specified object in the specified bucket.
        /// </summary>
        /// <param name="request"> The request object containing all options for deleting a Bos object. </param>
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

        /// <summary>
        /// Initiates a multipart upload and returns an InitiateMultipartUploadResponse
        /// which contains an upload ID. This upload ID associates all the parts in
        /// the specific upload and is used in each of your subsequent uploadPart requests.
        /// You also include this upload ID in the final request to either complete, or abort the multipart
        /// upload request.
        /// </summary>
        /// <param name="bucketName"> The name of the Bos bucket containing the object to initiate. </param>
        /// <param name="key"> The key of the object to initiate. </param>
        /// <returns> An InitiateMultipartUploadResponse from Bos. </returns>
        public InitiateMultipartUploadResponse InitiateMultipartUpload(string bucketName, string key)
        {
            return this.InitiateMultipartUpload(new InitiateMultipartUploadRequest()
            {
                BucketName = bucketName,
                Key = key
            });
        }

        /// <summary>
        /// Initiates a multipart upload and returns an InitiateMultipartUploadResponse
        /// which contains an upload ID. This upload ID associates all the parts in
        /// the specific upload and is used in each of your subsequent uploadPart requests.
        /// You also include this upload ID in the final request to either complete, or abort the multipart
        /// upload request.
        /// </summary>
        /// <param name="request"> The InitiateMultipartUploadRequest object that specifies all the parameters of this operation. </param>
        /// <returns> An InitiateMultipartUploadResponse from Bos. </returns>
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

        /// <summary>
        /// Uploads a part in a multipart upload. You must initiate a multipart
        /// upload before you can upload any part.
        /// </summary>
        /// <param name="request"> The UploadPartRequest object that specifies all the parameters of this operation. </param>
        /// <returns> An UploadPartResponse from Bos containing the part number and ETag of the new part. </returns>
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
            if (input.CanSeek && string.IsNullOrEmpty(request.Md5Digest))
            {
                request.Md5Digest = HashUtils.ComputeMD5Hash(input, request.PartSize);
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

        /// <summary>
        /// Lists the parts that have been uploaded for a specific multipart upload.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the multipart upload whose parts are being listed. </param>
        /// <param name="key"> The key of the associated multipart upload whose parts are being listed. </param>
        /// <param name="uploadId"> The ID of the multipart upload whose parts are being listed. </param>
        /// <returns> Returns a ListPartsResponse from Bos. </returns>
        public ListPartsResponse ListParts(string bucketName, string key, string uploadId)
        {
            return ListParts(new ListPartsRequest()
            {
                BucketName = bucketName,
                Key = key,
                UploadId = uploadId
            });
        }

        /// <summary>
        /// Lists the parts that have been uploaded for a specific multipart upload.
        /// </summary>
        /// <param name="request"> The ListPartsRequest object that specifies all the parameters of this operation. </param>
        /// <returns> Returns a ListPartsResponse from Bos. </returns>
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

        /// <summary>
        /// Completes a multipart upload by assembling previously uploaded parts.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the multipart upload to complete. </param>
        /// <param name="key"> The key of the multipart upload to complete. </param>
        /// <param name="uploadId"> The ID of the multipart upload to complete. </param>
        /// <param name="partETags"> The list of part numbers and ETags to use when completing the multipart upload. </param>
        /// <returns> A CompleteMultipartUploadResponse from Bos containing the ETag for
        ///     the new object composed of the individual parts. </returns>
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

        /// <summary>
        /// Completes a multipart upload by assembling previously uploaded parts.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the multipart upload to complete. </param>
        /// <param name="key"> The key of the multipart upload to complete. </param>
        /// <param name="uploadId"> The ID of the multipart upload to complete. </param>
        /// <param name="partETags"> The list of part numbers and ETags to use when completing the multipart upload. </param>
        /// <param name="metadata"> Additional metadata instructing Bos how to handle the uploaded data
        ///     (e.g. custom user metadata, hooks for specifying content type, etc.). </param>
        /// <returns> A CompleteMultipartUploadResponse from Bos containing the ETag for
        ///     the new object composed of the individual parts. </returns>
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

        /// <summary>
        /// Completes a multipart upload by assembling previously uploaded parts.
        /// </summary>
        /// <param name="request"> The CompleteMultipartUploadRequest object that specifies all the parameters of this operation. </param>
        /// <returns> A CompleteMultipartUploadResponse from Bos containing the ETag for
        ///     the new object composed of the individual parts. </returns>
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

        /// <summary>
        /// Aborts a multipart upload. After a multipart upload is aborted, no
        /// additional parts can be uploaded using that upload ID. The storage
        /// consumed by any previously uploaded parts will be freed. However, if any
        /// part uploads are currently in progress, those part uploads may or may not
        /// succeed. As a result, it may be necessary to abort a given multipart
        /// upload multiple times in order to completely free all storage consumed by
        /// all parts.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the multipart upload to abort. </param>
        /// <param name="key"> The key of the multipart upload to abort. </param>
        /// <param name="uploadId"> The ID of the multipart upload to abort. </param>
        public void AbortMultipartUpload(string bucketName, string key, string uploadId)
        {
            AbortMultipartUpload(new AbortMultipartUploadRequest()
            {
                BucketName = bucketName,
                Key = key,
                UploadId = uploadId
            });
        }

        /// <summary>
        /// Aborts a multipart upload. After a multipart upload is aborted, no
        /// additional parts can be uploaded using that upload ID. The storage
        /// consumed by any previously uploaded parts will be freed. However, if any
        /// part uploads are currently in progress, those part uploads may or may not
        /// succeed. As a result, it may be necessary to abort a given multipart
        /// upload multiple times in order to completely free all storage consumed by
        /// all parts.
        /// </summary>
        /// <param name="request"> The AbortMultipartUploadRequest object that specifies all the parameters of this operation. </param>
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

        /// <summary>
        /// Lists in-progress multipart uploads. An in-progress multipart upload is a multipart upload that has
        /// been initiated, using the InitiateMultipartUpload request, but has not yet been completed or aborted.
        /// </summary>
        /// <param name="bucketName"> The name of the bucket containing the uploads to list. </param>
        /// <returns> A ListMultipartUploadsResponse from Bos. </returns>
        public ListMultipartUploadsResponse ListMultipartUploads(string bucketName)
        {
            return ListMultipartUploads(new ListMultipartUploadsRequest() {BucketName = bucketName});
        }

        /// <summary>
        /// Lists in-progress multipart uploads. An in-progress multipart upload is a multipart upload that has
        /// been initiated, using the InitiateMultipartUpload request, but has not yet been completed or aborted.
        /// </summary>
        /// <param name="request"> The ListMultipartUploadsRequest object that specifies all the parameters of this operation. </param>
        /// <returns> A ListMultipartUploadsResponse from Bos. </returns>
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

        /// <summary>
        /// Populates the specified request object with the appropriate headers from the ObjectMetadata object.
        /// </summary>
        /// <param name="request"> The request to populate with headers. </param>
        /// <param name="metadata"> The metadata containing the header information to include in the request. </param>
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
            string resourcePath = request.Uri.AbsolutePath;

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
                return new Uri(HttpUtility.UrlDecode(urlstring, Encoding.UTF8));
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