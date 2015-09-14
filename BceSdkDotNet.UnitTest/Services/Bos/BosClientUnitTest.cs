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

            protected BceClientConfiguration config;

            protected BosClient client;

            [TestInitialize()]
            public void TestInitialize()
            {
                this.bucketName = (BucketPrefix + new Random().Next().ToString("X")).ToLower();
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
                BosObject bosObject = this.client.GetObject(this.bucketName, key);
                String content =
                    Encoding.Default.GetString(IOUtils.StreamToBytes(bosObject.ObjectContent,
                        bosObject.ObjectMetadata.ContentLength, 8192));
                Assert.AreEqual(content, "data");
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
    }
}