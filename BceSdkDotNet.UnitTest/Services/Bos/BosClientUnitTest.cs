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

using Microsoft.VisualStudio.TestTools.UnitTesting;

using BaiduBce.Services.Bos;
using BaiduBce.Auth;
using System.Diagnostics;

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
                this.bucketName = BucketPrefix + new Random().Next().ToString("X");
                this.config = new BceClientConfiguration();
                this.config.Credentials = new DefaultBceCredentials(this.ak, this.sk);
                this.config.Endpoint = this.endpoint;
                this.client = new BosClient(this.config);
                this.client.CreateBucket(this.bucketName);
            }
        }

        [TestClass]
        public class CommonTest : Base
        {
            [TestMethod]
            public void TestRequestWithInvalidCredential()
            {
            }
        }
    }
}
