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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaiduBce.Auth;
using BaiduBce.Services.Sms;
using BaiduBce.Services.Sms.Model;

namespace BaiduBce.UnitTest.Services.Sms
{
    public class SmsClientUnitTest
    {
        public class Base : BceClientUnitTestBase
        {
            public TestContext TestContext { get; set; }

            protected BceClientConfiguration config;

            protected SmsClient client;

            [TestInitialize()]
            public void TestInitialize()
            {
                this.config = new BceClientConfiguration();
                this.config.Credentials = new DefaultBceCredentials("", "");
                this.config.Endpoint = "http://localhost:8220";
                this.client = new SmsClient(this.config);
            }

            [TestCleanup()]
            public void TestCleanup()
            {
            }
        }

        [TestClass]
        public class SendMessageTest : Base
        {
            [TestMethod]
            public void TestSendMessage()
            {
                SendMessageRequest request = new SendMessageRequest();
                request.Mobile = "17617171892";
                request.Template = "tep-xxx";
                request.SignatureId = "sms-sign-xxx";
                request.ContentVar = new Dictionary<string, string>
                {
                    {"content", "测试"}
                };
                SendMessageResponse response = this.client.SendMessage(request);
                Assert.IsTrue(response.Data.Count > 0);
            }
        }

        [TestClass]
        public class TemplateTest : Base
        {
            [TestMethod]
            public void TestTemplate()
            {
                TemplateCreateRequest request = new TemplateCreateRequest();
                request.Name = "模板测试";
                request.Content = "${content}";
                request.CountryType = "DOMESTIC";
                request.SmsType = "CommonNotice";
                request.Description = "模板测试";
                TemplateCreateResponse response = this.client.CreateTemplate(request);
                Assert.IsTrue(response.TemplateId.Length > 0);

                TemplateUpdateRequest requestUpdate = new TemplateUpdateRequest();
                requestUpdate.Name = "模板测试-更新";
                requestUpdate.Content = "${content}";
                requestUpdate.CountryType = "DOMESTIC";
                requestUpdate.SmsType = "CommonNotice";
                requestUpdate.Description = "模板测试";
                this.client.UpdateTemplate(requestUpdate);

                TemplateResponse templateResponse = this.client.GetTemplate(response.TemplateId);
                Assert.IsTrue(templateResponse.Name.Equals(requestUpdate.Name));
                
                this.client.DeleteTemplate(response.TemplateId); 
                templateResponse = this.client.GetTemplate(response.TemplateId);
                Assert.IsTrue(string.IsNullOrEmpty(templateResponse.TemplateId));
            }
        }

        [TestClass]
        public class SignatureTest : Base
        {
            [TestMethod]
            public void TestSignature()
            {
                SignatureCreateRequest request = new SignatureCreateRequest();
                request.Content = "测试";
                request.CountryType = "DOMESTIC";
                request.ContentType = "MobileApp";
                request.Description = "测试";
                SignatureCreateResponse response = this.client.CreateSignature(request);
                Assert.IsTrue(!string.IsNullOrEmpty(response.SignatureId));

                SignatureUpdateRequest updateRequest = new SignatureUpdateRequest();
                request.Content = "测试更新";
                updateRequest.CountryType = "DOMESTIC";
                updateRequest.ContentType = "MobileApp";
                updateRequest.Description = "MobileApp";
                this.client.UpdateSignature(updateRequest);

                SignatureResponse signatureResponse = this.client.GetSignature(response.SignatureId);
                Assert.IsTrue(signatureResponse.Content.Equals(updateRequest.Content));
                
                this.client.DeleteSignature(response.SignatureId);
                signatureResponse = this.client.GetSignature(response.SignatureId);
                Assert.IsTrue(string.IsNullOrEmpty(signatureResponse.SignatureId));
            }
        }

        [TestClass]
        public class QuotaRateTest : Base
        {
            [TestMethod]
            public void TestQuotaRate()
            {
                QuotaUpdateRequest request = new QuotaUpdateRequest();
                request.QuotaPerDay = 100;
                request.QuotaPerMonth = 1000;
                request.QuotaPerMonth = 1000;
                request.RateLimitPerMinute = 1;
                request.RateLimitPerHour = 3;
                request.RateLimitPerDay = 10;
                this.client.UpdateQuotaRate(request);

                QuotaQueryResponse response = this.client.QueryQuotaRate();
                Assert.IsTrue(response.QuotaApplyCheckStatus.Equals("PASS"));
            }
        }

        [TestClass]
        public class BlackMobileTest : Base
        {
            [TestMethod]
            public void TestBlackMobile()
            {
                BlackCreateRequest request = new BlackCreateRequest();
                request.Phone = "18889999999";
                request.SmsType = "CommonNotice";
                request.Type = "MerchantBlack";
                this.client.CreateMobileBlack(request);

                BlackListRequest listRequest = new BlackListRequest();
                listRequest.Phone = request.Phone;
                BlackListResponse listResponse = this.client.GetMobileBlack(listRequest);
                Assert.IsTrue(listResponse.TotalCount > 0);
                
                this.client.DeleteMobileBlack(request.Phone);
                listResponse = this.client.GetMobileBlack(listRequest);
                Assert.IsTrue(listResponse.TotalCount == 0);
            }
        }

        [TestClass]
        public class StatisticsTemplateTest : Base
        {
            [TestMethod]
            public void TestStatisticsTemplate()
            {
                StatisticsTemplateListRequest request = new StatisticsTemplateListRequest();
                request.StartTime = "20230101";
                request.EndTime = "20230808";
                StatisticsTemplateListResponse response = this.client.GetTemplateStatistics(request);
                Assert.IsTrue(response.data.SubmitTotal == 0);
            }
        }
                
        [TestClass]
        
        public class ListStatisticsTest : Base
        {
            [TestMethod]
            public void TestListStatistics()
            {
                // 填写必要查询条件的普通情况
                ListStatisticsRequest request = new ListStatisticsRequest();
                request.StartTime = "2023-09-30";
                request.EndTime = "2023-09-30";
                ListStatisticsResponse response = this.client.GetListStatistics(request);
                Assert.AreEqual(2, response.StatisticsResults.Count);
                
                // 增加一条可选查询条件
                request.CountryType = "international";
                response = this.client.GetListStatistics(request);
                Assert.AreEqual(4, response.StatisticsResults.Count);
                
                // 缺少必要查询条件
                request.StartTime = null;
                try
                { 
                    this.client.GetListStatistics(request);
                }
                catch (ArgumentNullException e)
                {
                    Assert.IsTrue(e.Message.Contains(
                        "statistics request start time should not be null or empty"
                    ));
                }
                
                // 时间格式错误
                request.StartTime = "2023-10-01 00:00:00";
                try
                { 
                    this.client.GetListStatistics(request);
                }
                catch (BceServiceException e)
                {
                    Assert.IsTrue(e.Message.Contains("Internal Server Error"));
                }
                
                // 超出查询范围
                request.StartTime = "2022-10-01";
                try
                { 
                    this.client.GetListStatistics(request);
                }
                catch (BceServiceException e)
                {
                    Assert.IsTrue(e.Message.Contains("请求时间范围错误，请检查请求查询时间范围"));
                }
            }
        }
    }

    

}