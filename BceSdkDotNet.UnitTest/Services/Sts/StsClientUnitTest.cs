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

using Microsoft.VisualStudio.TestTools.UnitTesting;

using BaiduBce.Auth;
using BaiduBce.Services.Sts;
using BaiduBce.Services.Sts.Model;

namespace BaiduBce.UnitTest.Services.Sts
{
    public class StsClientUnitTest
    {
        public class Base : BceClientUnitTestBase
        {
            public TestContext TestContext { get; set; }

            protected BceClientConfiguration config;

            protected StsClient client;

            [TestInitialize()]
            public void TestInitialize()
            {
                this.config = new BceClientConfiguration();
                this.config.Credentials =
                    new DefaultBceCredentials("d154df3e2ac44e53b566db53ec644a7e", "2e8445d62bc84011991b4306b91f19f8");
                this.config.Endpoint = "http://10.107.37.40:8586";
                this.client = new StsClient(this.config);
            }

            [TestCleanup()]
            public void TestCleanup()
            {
            }
        }

        [TestClass]
        public class GetSessionTokenTest : Base
        {
            [TestMethod]
            public void TestDefaultArguments()
            {
                var getSessionTokenResponse = this.client.GetSessionToken();
                Assert.IsNotNull(getSessionTokenResponse.AccessKeyId);
                Assert.IsNotNull(getSessionTokenResponse.SecretAccessKey);
                Assert.IsNotNull(getSessionTokenResponse.SessionToken);
                Assert.IsNotNull(getSessionTokenResponse.Expiration);
                Assert.IsTrue((getSessionTokenResponse.Expiration - DateTime.Now).TotalSeconds > 1500);
            }

            [TestMethod]
            public void TestDurationSeconds()
            {
                var getSessionTokenResponse =
                    this.client.GetSessionToken(new GetSessionTokenRequest() { DurationSeconds = 10 });
                Assert.IsNotNull(getSessionTokenResponse.AccessKeyId);
                Assert.IsNotNull(getSessionTokenResponse.SecretAccessKey);
                Assert.IsNotNull(getSessionTokenResponse.SessionToken);
                Assert.IsNotNull(getSessionTokenResponse.Expiration);
                Assert.IsTrue((getSessionTokenResponse.Expiration - DateTime.Now).TotalSeconds < 30);
            }

            [TestMethod]
            public void TestAcl()
            {
                var getSessionTokenResponse =
                    this.client.GetSessionToken(new GetSessionTokenRequest()
                    {
                        AccessControlList = @"
                        {
                            ""id"": ""test"",
                            ""accessControlList"": [
                                {
                                    ""eid"": ""e0"",
                                    ""service"": ""bce:bos"",
                                    ""region"": ""bj"",
                                    ""effect"": ""Allow"",
                                    ""resource"": [""test-bucket/*""],
                                    ""permission"": [""READ""]
                                }
                            ]
                        }"
                    });
                Assert.IsNotNull(getSessionTokenResponse.AccessKeyId);
                Assert.IsNotNull(getSessionTokenResponse.SecretAccessKey);
                Assert.IsNotNull(getSessionTokenResponse.SessionToken);
                Assert.IsNotNull(getSessionTokenResponse.Expiration);
            }

            [TestMethod]
            [ExpectBceServiceException(400, "InvalidRequest")]
            public void TestEmptyAcl()
            {
                var getSessionTokenResponse =
                    this.client.GetSessionToken(new GetSessionTokenRequest() { AccessControlList = "{}" });
            }

            [TestMethod]
            [ExpectBceServiceException(400, "InvalidRequestBody")]
            public void TestInvalidAcl()
            {
                var getSessionTokenResponse =
                    this.client.GetSessionToken(new GetSessionTokenRequest() { AccessControlList = "{" });
            }

            [TestMethod]
            [ExpectBceServiceException(400, "InvalidRequest", "not support temporal credential")]
            public void TestTemporaryCredentials()
            {
                var getSessionTokenResponse = this.client.GetSessionToken();
                var tempCredentials = new DefaultBceSessionCredentials(
                    getSessionTokenResponse.AccessKeyId,
                    getSessionTokenResponse.SecretAccessKey,
                    getSessionTokenResponse.SessionToken);
                this.client.GetSessionToken(new GetSessionTokenRequest() { Credentials = tempCredentials });
            }
        }
    }
}