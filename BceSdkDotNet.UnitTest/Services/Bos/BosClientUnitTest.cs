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
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaiduBce.Services.Bos;
using BaiduBce.Services.Bos.Model;
using BaiduBce.Auth;
using BaiduBce.Util;

namespace BaiduBce.UnitTest.Services.Bos
{
    /// <summary>
    /// Summary description for BosClientUnitTest
    /// </summary>
    public class BosClientUnitTest
    {
        public class Base : BceClientUnitTestBase
        {
            public TestContext TestContext { get; set; }

            protected static readonly string BucketPrefix = "ut-net-" + new Random().Next().ToString("X") + "-";

            protected string bucketName;

            protected User owner;

            protected Grantee grantee;

            protected Grantee anonymous;

            protected BceClientConfiguration config;

            protected BosClient client;

            [TestInitialize()]
            public void TestInitialize()
            {
                this.bucketName = (BucketPrefix + new Random().Next().ToString("X")).ToLower();
                this.owner = new User() {Id = this.userId, DisplayName = "PASSPORT:105015426"};
                this.grantee = new Grantee() {Id = this.userId};
                this.anonymous = new Grantee() {Id = "*"};
                this.config = new BceClientConfiguration();
                this.config.Credentials = new DefaultBceCredentials(this.ak, this.sk);
                this.config.Endpoint = this.endpoint;
                this.client = new BosClient(this.config);
                this.client.CreateBucket(this.bucketName);
            }

            [TestCleanup()]
            public void TestCleanup()
            {
                this.client = new BosClient(this.config);
                List<BucketSummary> buckets = this.client.ListBuckets().Buckets;
                if (buckets == null || buckets.Count == 0)
                {
                    return;
                }
                foreach (BucketSummary bucket in buckets)
                {
                    string bucketName = bucket.Name;
                    if (bucketName.StartsWith("ut"))
                    {
                        ListObjectsResponse listObjectsResponse = this.client.ListObjects(bucketName);
                        List<BosObjectSummary> objects = listObjectsResponse.Contents;
                        if (objects != null && objects.Count > 0)
                        {
                            foreach (BosObjectSummary bosObject in objects)
                            {
                                String key = bosObject.Key;
                                this.client.DeleteObject(bucketName, key);
                            }
                        }
                        this.client.DeleteBucket(bucket.Name);
                    }
                }
            }
        }

        [TestClass]
        public class CommonTest : Base
        {
            [TestMethod]
            [ExpectedException(typeof (BceServiceException))]
            public void TestRequestWithInvalidCredential()
            {
                BceClientConfiguration bceClientConfiguration = new BceClientConfiguration();
                bceClientConfiguration.Credentials = new DefaultBceCredentials("test", "test");
                bceClientConfiguration.Endpoint = this.endpoint;
                this.client = new BosClient(bceClientConfiguration);
                this.client.ListBuckets();
            }

            [TestMethod]
            public void TestMimetypes()
            {
                Assert.AreEqual(MimeTypes.GetMimetype("png"), "image/png");
                Assert.AreEqual(MimeTypes.GetMimetype("gram"), "application/srgs");
                Assert.AreEqual(MimeTypes.GetMimetype(""), MimeTypes.MimeTypeOctetStream);
            }
        }

        [TestClass]
        public class BucketTest : Base
        {
            [TestMethod]
            public void TestListBuckets()
            {
                var listBucketsResponse = this.client.ListBuckets();
                Assert.IsTrue(listBucketsResponse.Buckets.Count > 0);
            }

            [TestMethod]
            public void TestDoesBucketExist()
            {
                Assert.IsTrue(this.client.DoesBucketExist(this.bucketName));
                Assert.IsFalse(this.client.DoesBucketExist("xxxaaa"));
            }

            [TestMethod]
            public void TestGetBucketLocation()
            {
                GetBucketLocationResponse getBucketLocationResponse = this.client.GetBucketLocation(this.bucketName);
                Assert.AreEqual(getBucketLocationResponse.LocationConstraint, "bj");
            }
        }

        [TestClass]
        public class SetBucketAclTest : Base
        {
            [TestMethod]
            public void TestPublicReadWrite()
            {
                string objectKey = "objectPublicReadWrite";
                string data = "dataPublicReadWrite";

                this.client.SetBucketAcl(this.bucketName, BosConstants.CannedAcl.PublicReadWrite);
                GetBucketAclResponse response = this.client.GetBucketAcl(this.bucketName);
                Assert.AreEqual(response.Owner.Id, this.grantee.Id);

                List<Grant> grants = new List<Grant>();
                List<Grantee> granteeOwner = new List<Grantee>();
                granteeOwner.Add(this.grantee);
                List<string> permissionOwner = new List<string>();
                permissionOwner.Add(BosConstants.Permission.FullControl);
                grants.Add(new Grant() {Grantee = granteeOwner, Permission = permissionOwner});
                List<Grantee> granteeAnonymous = new List<Grantee>();
                granteeAnonymous.Add(this.anonymous);
                List<string> permissionAnonymous = new List<string>();
                permissionAnonymous.Add(BosConstants.Permission.Read);
                permissionAnonymous.Add(BosConstants.Permission.Write);
                grants.Add(new Grant() {Grantee = granteeAnonymous, Permission = permissionAnonymous});

                Assert.AreEqual(response.AccessControlList.Count, grants.Count);
                this.client.PutObject(this.bucketName, objectKey, data);
                BceClientConfiguration bceClientConfiguration = new BceClientConfiguration();
                bceClientConfiguration.Endpoint = this.endpoint;
                BosClient bosAnonymous = new BosClient(bceClientConfiguration);
                Assert.AreEqual(
                    Encoding.Default.GetString(bosAnonymous.GetObjectContent(this.bucketName, objectKey)), data);

                bosAnonymous.PutObject(this.bucketName, "anonymous", "dataAnonymous");
                Assert.AreEqual(
                    Encoding.Default.GetString(bosAnonymous.GetObjectContent(this.bucketName, "anonymous")),
                    "dataAnonymous");
            }
        }

        [TestClass]
        public class GeneratePresignedUrlTest : Base
        {
            [TestMethod]
            public void TestOrdinary()
            {
                string objectKey = "test";
                string value = "value1" + "\n" + "value2";
                this.client.PutObject(this.bucketName, objectKey, value);
                GeneratePresignedUrlRequest request = new GeneratePresignedUrlRequest()
                {
                    BucketName = this.bucketName,
                    Key = objectKey,
                    Method = BceConstants.HttpMethod.Get
                };
                request.ExpirationInSeconds = 1800;
                Uri url = this.client.GeneratePresignedUrl(request);
                using (WebClient webClient = new WebClient())
                {
                    using (Stream stream = webClient.OpenRead(url))
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        string response = streamReader.ReadToEnd();
                        Assert.AreEqual(response, value);
                    }

                }
            }
        }

        [TestClass]
        public class CopyObjectTest : Base
        {
            [TestMethod]
            public void TestOrdinary()
            {
                string objectName = "sample";
                client.PutObject(bucketName, objectName, "sampledata");

                // 2. 普通拷贝并打印结果
                string newObjectName = "copyobject";
                CopyObjectResponse copyObjectResponse = client.CopyObject(bucketName, objectName, bucketName,
                    newObjectName);
                // sampledata
                Assert.AreEqual(Encoding.Default.GetString(client.GetObjectContent(bucketName, newObjectName)),
                    "sampledata");

                // 3. 拷贝并设置新的meta
                newObjectName = "copyobject-newmeta";
                CopyObjectRequest copyObjectRequest = new CopyObjectRequest()
                {
                    SourceBucketName = bucketName,
                    SourceKey = objectName,
                    BucketName = bucketName,
                    Key = newObjectName
                };
                Dictionary<String, String> userMetadata = new Dictionary<String, String>();
                userMetadata["metakey"] = "metavalue";
                ObjectMetadata objectMetadata = new ObjectMetadata()
                {
                    UserMetadata = userMetadata
                };
                copyObjectRequest.NewObjectMetadata = objectMetadata;
                client.CopyObject(copyObjectRequest);
                Assert.AreEqual(client.GetObjectMetadata(bucketName, newObjectName).UserMetadata["metakey"],
                    "metavalue");
            }
        }

        [TestClass]
        public class PutObjectTest : Base
        {
            [TestMethod]
            public void TestOrdinary()
            {
                string path = "put_object_ordinary.txt";
                File.WriteAllText(path, "data");
                FileInfo fileInfo = new FileInfo(path);
                string key = "te%%st  ";
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo
                };
                String eTag = this.client.PutObject(request).ETAG;
                Assert.AreEqual(eTag, HashUtils.ComputeMD5Hash(fileInfo));
                String content = System.Text.Encoding.Default.GetString(this.client.GetObjectContent
                    (this.bucketName, key));
                Assert.AreEqual(content, "data");
                FileInfo outFileInfo = new FileInfo("object_ordinary.txt");
                this.client.GetObject(this.bucketName, key, outFileInfo);
                Assert.AreEqual(eTag, HashUtils.ComputeMD5Hash(outFileInfo));
            }

            [TestMethod]
            public void TestContentLengthSmallThanStreamLength()
            {
                ObjectMetadata objectMetadata = new ObjectMetadata();
                objectMetadata.ContentLength = 2;
                var userMetaDic = new Dictionary<string, string>();
                objectMetadata.UserMetadata = userMetaDic;
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = "te%%st",
                    Stream = new MemoryStream(System.Text.Encoding.Default.GetBytes("data")),
                    ObjectMetadata = objectMetadata
                };
                this.client.PutObject(request);
                String content = System.Text.Encoding.Default.GetString(this.client.GetObjectContent
                    (this.bucketName, "te%%st"));
                Assert.AreEqual(content, "da");
            }
        }

        [TestClass]
        public class GetObjectTest : Base
        {
            [TestMethod]
            public void TestGetObjectOk()
            {
                string path = "put_object_ordinary.txt";
                File.WriteAllText(path, "data");
                FileInfo fileInfo = new FileInfo(path);
                string key = "te%%st  ";
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo
                };
                String eTag = this.client.PutObject(request).ETAG;
                Assert.AreEqual(eTag, HashUtils.ComputeMD5Hash(fileInfo));
                BosObject bosObject = this.client.GetObject(this.bucketName, key);
                String content =
                    Encoding.Default.GetString(IOUtils.StreamToBytes(bosObject.ObjectContent,
                        bosObject.ObjectMetadata.ContentLength, 8192));
                Assert.AreEqual(content, "data");
            }

            [TestMethod]
            public void TestGetIAOk()
            {
                string path = "put_object_ordinary.txt";
                File.WriteAllText(path, "data");
                FileInfo fileInfo = new FileInfo(path);
                string key = "te%%st  ";
                var meta = new ObjectMetadata();
                meta.StorageClass = BosConstants.StorageClass.StandardInfrequentAccess;
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo,
                    ObjectMetadata = meta
                };
                String eTag = this.client.PutObject(request).ETAG;
                Assert.AreEqual(eTag, HashUtils.ComputeMD5Hash(fileInfo));
                BosObject bosObject = this.client.GetObject(this.bucketName, key);
                String content =
                    Encoding.Default.GetString(IOUtils.StreamToBytes(bosObject.ObjectContent,
                        bosObject.ObjectMetadata.ContentLength, 8192));
                Assert.AreEqual(content, "data");
                Assert.AreEqual(BosConstants.StorageClass.StandardInfrequentAccess,
                    bosObject.ObjectMetadata.StorageClass);
            }

            [TestMethod]
            public void TestGetRange()
            {
                string path = "put_object_ordinary.txt";
                File.WriteAllText(path, "data");
                FileInfo fileInfo = new FileInfo(path);
                string key = "te%%st  ";
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo
                };
                this.client.PutObject(request);
                GetObjectRequest getObjectRequest = new GetObjectRequest() {BucketName = this.bucketName, Key = key};
                getObjectRequest.SetRange(0, 0);
                BosObject bosObject = this.client.GetObject(getObjectRequest);
                String content =
                    Encoding.Default.GetString(IOUtils.StreamToBytes(bosObject.ObjectContent,
                        bosObject.ObjectMetadata.ContentLength, 8192));
                Assert.AreEqual(content, "d");
            }
        }

        [TestClass]
        public class InitiateMultipartUploadTest : Base
        {
            [TestMethod]
            public void TestOrdinary()
            {
                InitiateMultipartUploadResponse response = this.client.InitiateMultipartUpload(this.bucketName, "test");
                Assert.AreEqual(response.Bucket, this.bucketName);
                Assert.AreEqual(response.Key, "test");
                String uploadId = response.UploadId;
                List<MultipartUploadSummary> uploads =
                    this.client.ListMultipartUploads(this.bucketName).Uploads;
                Assert.AreEqual(uploads.Count, 1);
                Assert.AreEqual(uploads[0].UploadId, uploadId);
            }

            [TestMethod]
            public void TestInitiateIAMultiUpload()
            {
                var request = new InitiateMultipartUploadRequest();
                request.BucketName = this.bucketName;
                request.Key = "test";
                request.ObjectMetadata = new ObjectMetadata();
                request.ObjectMetadata.StorageClass = BosConstants.StorageClass.StandardInfrequentAccess;
                InitiateMultipartUploadResponse response = this.client.InitiateMultipartUpload(request);
                Assert.AreEqual(response.Bucket, this.bucketName);
                Assert.AreEqual(response.Key, "test");
                String uploadId = response.UploadId;
                List<MultipartUploadSummary> uploads =
                    this.client.ListMultipartUploads(this.bucketName).Uploads;
                Assert.AreEqual(uploads.Count, 1);
                Assert.AreEqual(uploads[0].UploadId, uploadId);
                Assert.AreEqual(BosConstants.StorageClass.StandardInfrequentAccess, uploads[0].StorageClass);
            }
        }

        [TestClass]
        public class UploadPartTest : Base
        {
            [TestMethod]
            public void TestOrdinary()
            {
                String uploadId = this.client.InitiateMultipartUpload(this.bucketName, "test").UploadId;
                UploadPartResponse response = this.client.UploadPart(new UploadPartRequest()
                {
                    BucketName = this.bucketName,
                    Key = "test",
                    UploadId = uploadId,
                    PartNumber = 1,
                    PartSize = 4,
                    InputStream = new MemoryStream(Encoding.Default.GetBytes("data"))
                });
                Assert.AreEqual(response.PartNumber, 1);
                Assert.IsNotNull(response.ETag);
                List<PartSummary> parts = this.client.ListParts(this.bucketName, "test", uploadId).Parts;
                Assert.AreEqual(parts.Count, 1);
                PartSummary part = parts[0];
                Assert.IsNotNull(part);
                Assert.AreEqual(part.ETag, response.ETag);
                Assert.AreEqual(part.Size, 4L);
            }
        }

        [TestClass]
        public class ListPartsTest : Base
        {
            [TestMethod]
            public void TestListPartsOk()
            {
                string uploadId = this.client.InitiateMultipartUpload(this.bucketName, "test").UploadId;
                List<string> eTags = new List<string>();
                for (int i = 0; i < 10; ++i)
                {
                    eTags.Add(this.client.UploadPart(new UploadPartRequest()
                    {
                        BucketName = this.bucketName,
                        Key = "test",
                        UploadId = uploadId,
                        PartNumber = i + 1,
                        PartSize = 1,
                        InputStream = new MemoryStream(Encoding.Default.GetBytes(i.ToString()))
                    }).ETag);
                }
                ListPartsResponse response = this.client.ListParts(this.bucketName, "test", uploadId);
                Assert.AreEqual(response.BucketName, this.bucketName);
                Assert.AreEqual(response.IsTruncated, false);
                Assert.AreEqual(response.Key, "test");
                Assert.AreEqual(response.MaxParts, 1000);
                Assert.AreEqual(response.NextPartNumberMarker, 10);
                Assert.AreEqual(response.Owner.Id, this.owner.Id);
                Assert.AreEqual(response.PartNumberMarker, 0);
                Assert.AreEqual(response.UploadId, uploadId);
                Assert.AreEqual(BosConstants.StorageClass.Standard, response.StorageClass);
                List<PartSummary> parts = response.Parts;
                Assert.AreEqual(parts.Count, 10);
                for (int i = 0; i < 10; ++i)
                {
                    PartSummary part = parts[i];
                    Assert.AreEqual(part.ETag, eTags[i]);
                    Assert.AreEqual(part.PartNumber, i + 1);
                    Assert.AreEqual(part.Size, 1);
                    Assert.IsTrue(Math.Abs(part.LastModified.Subtract(DateTime.UtcNow).TotalSeconds) < 60);
                }
            }
        }

        [TestClass]
        public class CompleteMultipartUploadTest : Base
        {
            [TestMethod]
            public void TestOrdinary()
            {
                ObjectMetadata objectMetadata = new ObjectMetadata();
                objectMetadata.ContentType = "text/plain";
                InitiateMultipartUploadRequest initRequest = new InitiateMultipartUploadRequest()
                {
                    BucketName = this.bucketName,
                    Key = "test",
                    ObjectMetadata = objectMetadata
                };

                string uploadId = this.client.InitiateMultipartUpload(initRequest).UploadId;
                List<PartETag> partETags = new List<PartETag>();
                for (int i = 0; i < 1; ++i)
                {
                    string eTag = this.client.UploadPart(new UploadPartRequest()
                    {
                        BucketName = this.bucketName,
                        Key = "test",
                        UploadId = uploadId,
                        PartNumber = i + 1,
                        PartSize = 1,
                        InputStream = new MemoryStream(Encoding.Default.GetBytes(i.ToString()))
                    }).ETag;
                    partETags.Add(new PartETag() {PartNumber = i + 1, ETag = eTag});
                }
                objectMetadata = new ObjectMetadata();
                Dictionary<string, string> userMetadata = new Dictionary<string, string>();
                userMetadata["metakey"] = "metaValue";
                objectMetadata.UserMetadata = userMetadata;
                objectMetadata.ContentType = "text/json";
                CompleteMultipartUploadRequest request =
                    new CompleteMultipartUploadRequest()
                    {
                        BucketName = this.bucketName,
                        Key = "test",
                        UploadId = uploadId,
                        PartETags = partETags,
                        ObjectMetadata = objectMetadata
                    };
                CompleteMultipartUploadResponse response = this.client.CompleteMultipartUpload(request);
                Assert.AreEqual(response.BucketName, this.bucketName);
                Assert.AreEqual(response.Key, "test");
                Assert.IsNotNull(response.ETag);
                Assert.IsNotNull(response.Location);
                ObjectMetadata metadata = this.client.GetObjectMetadata(bucketName, "test");
                Assert.AreEqual(metadata.ContentType, "text/plain");
                string resultUserMeta = metadata.UserMetadata["metakey"];
                Assert.AreEqual(resultUserMeta, "metaValue");
            }
        }

        [TestClass]
        public class AbortMultipartUploadTest : Base
        {
            [TestMethod]
            public void TestOrdinary()
            {
                string uploadId = this.client.InitiateMultipartUpload(this.bucketName, "abortMultipartTest").UploadId;
                for (int i = 0; i < 10; ++i)
                {
                    this.client.UploadPart(new UploadPartRequest()
                    {
                        BucketName = this.bucketName,
                        Key = "abortMultipartTest",
                        UploadId = uploadId,
                        PartNumber = i + 1,
                        PartSize = 1,
                        InputStream = new MemoryStream(Encoding.Default.GetBytes(i.ToString()))
                    });
                }
                List<MultipartUploadSummary> uploads =
                    this.client.ListMultipartUploads(this.bucketName).Uploads;
                Assert.AreEqual(uploads.Count, 1);
                this.client.AbortMultipartUpload(this.bucketName, "abortMultipartTest", uploadId);
                uploads = this.client.ListMultipartUploads(this.bucketName).Uploads;
                Assert.AreEqual(uploads.Count, 0);
            }
        }
        
        // Before performing the following test
        // 1.you need to create a bucket by yourself
        // 2.Apply for a domain name and register it
        // 3.Bind domain name to bucket
        [TestClass]
        public class CnameDomainTest : Base
        {
            [TestMethod]
            public void TestCnameHost()
            {
                string path = "put_object_cname_enabled.txt";
                File.WriteAllText(path, "data");
                FileInfo fileInfo = new FileInfo(path);
                string key = "test_cname_host.txt";
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo
                };
                BceClientConfiguration cfg = new BceClientConfiguration();
                cfg.Endpoint = "http://22.y001122.online";
                cfg.CnameEnabled = false;
                request.Config = cfg;
                PutObjectResponse response = client.PutObject(request);
                Assert.AreEqual(response.ETAG, HashUtils.ComputeMD5Hash(fileInfo));
            }
            
            [TestMethod]
            public void TestCnameLikeHost()
            {
                string path = "put_object_cname_disabled.txt";
                File.WriteAllText(path, "data");
                FileInfo fileInfo = new FileInfo(path);
                string key = "test_cname_like_host.txt";
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo
                };
                BceClientConfiguration cfg = new BceClientConfiguration();
                cfg.Endpoint = "http://test-cname.cdn.bcebos.com";
                cfg.CnameEnabled = false;
                request.Config = cfg;
                PutObjectResponse response = client.PutObject(request);
                Assert.AreEqual(response.ETAG, HashUtils.ComputeMD5Hash(fileInfo));
            }
        
            [TestMethod]
            public void TestCustomHost()
            {
                string path = "put_object_cname_disabled.txt";
                File.WriteAllText(path, "data");
                FileInfo fileInfo = new FileInfo(path);
                string key = "test_custom_host.txt";
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo
                };
                BceClientConfiguration cfg = new BceClientConfiguration();
                cfg.Endpoint = "http://test-cname.bj.bcebos.com";
                cfg.CnameEnabled = false;
                request.Config = cfg;
                request.BucketName = "test-cname";
                PutObjectResponse response = client.PutObject(request);
                Assert.AreEqual(response.ETAG, HashUtils.ComputeMD5Hash(fileInfo));
            }
        }

        [TestClass]
        public class TrafficLimitTest : Base
        {
            static void GenerateFile(string filePath, long fileSizeInBytes)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    fs.SetLength(fileSizeInBytes);
                }
            }
            
            [TestMethod]
            public void TestPutObjectTrafficLimit()
            {
                long fileSizeInBytes = 10 * 1024 * 1024;
                string filePath = "test_file_10M.txt";
                GenerateFile(filePath, fileSizeInBytes);
                FileInfo fileInfo = new FileInfo(filePath);
                
                string key = "test_traffic_limit.txt";
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo
                };
                BceClientConfiguration cfg = new BceClientConfiguration();
                request.Config = cfg;
                request.TrafficLimit = 819200;
                
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                PutObjectResponse response = client.PutObject(request);
                stopwatch.Stop();
                
                Console.WriteLine("Time taken: " + stopwatch.Elapsed);
                // Assert.IsTrue(stopwatch.Elapsed.Seconds >= 10);
                // Assert.AreEqual(response.ETAG, HashUtils.ComputeMD5Hash(fileInfo));
            }

            [TestMethod]
            public void TestGetObjectTrafficLimit()
            {
                long fileSizeInBytes = 10 * 1024 * 1024;
                string filePath = "test_file_10M.txt";
                GenerateFile(filePath, fileSizeInBytes);
                FileInfo fileInfo = new FileInfo(filePath);
                
                string key = "test_traffic_limit.txt";
                PutObjectRequest putRequest = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = fileInfo
                };
                BceClientConfiguration putCfg = new BceClientConfiguration();
                putRequest.Config = putCfg;
                PutObjectResponse response = client.PutObject(putRequest);
                string etag = response.ETAG;
                
                GetObjectRequest getRequest = new GetObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                };
                BceClientConfiguration getCfg = new BceClientConfiguration();
                getRequest.Config = getCfg;
                getRequest.TrafficLimit = 819200;
                
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                BosObject bosObject = client.GetObject(getRequest);
                filePath = "test_file_10M_new.txt";
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    bosObject.ObjectContent.CopyTo(fileStream);
                }
                stopwatch.Stop();
                
                Console.WriteLine("Time taken: " + stopwatch.Elapsed);
                
                // Assert.IsTrue(stopwatch.Elapsed.Seconds >= 10);
                // Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
            }
        }

        [TestClass]
        public class VirtualHostedTest : Base
        {
            private string etag = "";
            
            [TestInitialize()]
            public void VirtualHostedTestInitialize()
            {
                string filePath = "test.txt";
                long fileSizeInBytes = 1024;
                string key = "test.txt";
                
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    fs.SetLength(fileSizeInBytes);
                }
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                    FileInfo = new FileInfo(filePath)
                };
                request.Config = new BceClientConfiguration();
                PutObjectResponse response = client.PutObject(request);
                etag = response.ETAG;
            }
            
            [TestMethod]
            public void TestPathStyleEnable()
            {
                string key = "test.txt";
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                };
                request.Config =  new BceClientConfiguration();
                request.Config.PathStyleEnabled = true;
                request.Config.Endpoint = "http://bj.bcebos.com";
                BosObject bosObject = client.GetObject(request);
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
                
                request.Config.Endpoint = "https://bj.bcebos.com";
                bosObject = client.GetObject(request);
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);

                request.Config.Endpoint = "bj.bcebos.com";
                bosObject = client.GetObject(request);
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
                
                request.Config.Endpoint = string.Format("http://{0}.bj.bcebos.com", bucketName);
                bosObject = client.GetObject(request);
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
                
                request.Config.Endpoint = string.Format("https://{0}.bj.bcebos.com", bucketName);
                bosObject = client.GetObject(request);     
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
                
                request.Config.Endpoint = string.Format("{0}.bj.bcebos.com", bucketName);
                bosObject = client.GetObject(request);     
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
            }
            
            [TestMethod]
            public void TestPathStylDisable()
            {
                string key = "test.txt";
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = this.bucketName,
                    Key = key,
                };
                request.Config =  new BceClientConfiguration();
                request.Config.PathStyleEnabled = false;
                request.Config.Endpoint = "http://bj.bcebos.com";
                BosObject bosObject = client.GetObject(request);
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
                
                request.Config.Endpoint = "https://bj.bcebos.com";
                bosObject = client.GetObject(request);
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
 
                request.Config.Endpoint = "bj.bcebos.com";
                bosObject = client.GetObject(request);
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
                
                request.Config.Endpoint = string.Format("http://{0}.bj.bcebos.com", bucketName);
                bosObject = client.GetObject(request);
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);
                
                request.Config.Endpoint = string.Format("https://{0}.bj.bcebos.com", bucketName);
                bosObject = client.GetObject(request);     
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag);                
                
                request.Config.Endpoint = string.Format("{0}.bj.bcebos.com", bucketName);
                bosObject = client.GetObject(request);     
                Assert.AreEqual(etag, bosObject.ObjectMetadata.ETag); 
            }
            
        }
    }
}