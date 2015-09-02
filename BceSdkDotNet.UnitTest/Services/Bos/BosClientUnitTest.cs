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
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

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
                        List<BosObjectSummary> objects = this.client.ListObjects(bucketName).Contents;
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
            [ExpectedException(typeof(BceServiceException))]
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
    }
}
