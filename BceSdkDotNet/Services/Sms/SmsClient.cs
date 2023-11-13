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
using System.Net;
using BaiduBce.Internal;
using BaiduBce.Model;
using BaiduBce.Services.Sms.Model;
using BaiduBce.Util;

namespace BaiduBce.Services.Sms
{
    /// <summary>
    /// Provides the client for accessing the Baidu Object Service.
    /// </summary>
	public class SmsClient : BceClientBase
	{
        private const string UrlPrefix = "/sms/v3";
        private const string serviceEndpointFormat = "{0}://smsv3.{1}.baidubce.com";

        /// <summary>
        /// Constructs a new client to invoke service methods on sms.
        /// </summary>
        public SmsClient()
			: this(new BceClientConfiguration())
		{
		}

        /// <summary>
        /// Constructs a new Sms client using the client configuration to access Sms.
        /// </summary>
        /// <param name="config"> The sms client configuration options controlling how this client
        ///     connects to sms (e.g. retry counts, etc).
        ///  </param>
        public SmsClient(BceClientConfiguration config)
            : base(config, serviceEndpointFormat)
        {
        }

        private InternalRequest CreateSmsRequest (BceRequestBase request, string httpMethod, string[] pathComponents)
        {
            InternalRequest internalRequest = this.CreateInternalRequest(request, httpMethod, pathComponents);
            
            // 自定义签名内容
            HashSet<string> headersToSign = new HashSet<string>
            {
                BceConstants.HttpHeaders.BceDate,
                BceConstants.HttpHeaders.Host
            };
            internalRequest.Config.SignOptions.HeadersToSign = headersToSign;
            
            return internalRequest;
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="request"> The request of send message </param>
        /// <returns> The result of send message. </returns>
        public SendMessageResponse SendMessage(SendMessageRequest request)
        {
            this.CheckNotNull(request, "request should NOT be null.");
            this.CheckNotNullOrEmpty(request.Mobile, "mobile is required.");
            this.CheckNotNullOrEmpty(request.SignatureId, "signatureId is required.");
            this.CheckNotNullOrEmpty(request.Template, "template is required.");

            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Post,
                new [] { "api/v3/sendsms" });
            if (!string.IsNullOrEmpty(request.ClientToken))
            {
                internalRequest.Parameters.Add("clientToken", request.ClientToken);
            }

            this.FillRequestBodyForJson(internalRequest, JsonUtils.SerializeObject(request));
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<SendMessageResponse>(httpWebResponse);
                }
            });
        }

        /// <summary>
        /// Create the template
        /// </summary>
        /// <param name="request">The TemplateCreateRequest object that specifies all the parameters of this operation.</param>
        /// <returns> Returns a TemplateCreateResponse from SMS.</returns>
        public TemplateCreateResponse CreateTemplate(TemplateCreateRequest request)
        {
            this.CheckNotNull(request, "object request should not be null");
            this.CheckNotNullOrEmpty(request.Content, "content should not be null or empty");
            this.CheckNotNullOrEmpty(request.CountryType, "countryType should not be null or empty");
            this.CheckNotNullOrEmpty(request.Name, "name should not be null or empty");
            this.CheckNotNullOrEmpty(request.SmsType, "smsType should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Post, 
                new [] {UrlPrefix, "/template"});
            this.FillRequestBodyForJson(internalRequest, JsonUtils.SerializeObject(request));
            internalRequest.Parameters.Add("clientToken", Guid.NewGuid().ToString());
            
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<TemplateCreateResponse>(httpWebResponse);
                }
            });
            
        }
        
        /// <summary>
        /// Update the template
        /// </summary>
        /// <param name="request">The TemplateUpdateRequest object that specifies all the parameters of this operation.</param>
        public void UpdateTemplate(TemplateUpdateRequest request)
        {
            this.CheckNotNull(request, "object request should not be null");
            this.CheckNotNullOrEmpty(request.TemplateId, "templateId should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Put, 
                new [] {UrlPrefix, "/template", request.TemplateId});
            this.FillRequestBodyForJson(internalRequest, JsonUtils.SerializeObject(request));
            
            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<BceResponseBase>(httpWebResponse);
                }
            });
            
        }
        
        /// <summary>
        /// Delete the template
        /// </summary>
        /// <param name="templateId">The unique code identifying the template.</param>
        public void DeleteTemplate(string templateId)
        {
            this.CheckNotNullOrEmpty( templateId, "templateId should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(new BceRequestBase(), BceConstants.HttpMethod.Delete, 
                new [] {UrlPrefix, "/template", templateId});
            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<BceResponseBase>(httpWebResponse);
                }
            });
            
        }
        
        /// <summary>
        /// Get the template
        /// </summary>
        /// <param name="templateId">The unique code identifying the template.</param>
        public TemplateResponse GetTemplate(string templateId)
        {
            this.CheckNotNullOrEmpty(templateId, "templateId should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(new BceRequestBase(), BceConstants.HttpMethod.Get, 
                new [] {UrlPrefix, "/template", templateId});
            
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<TemplateResponse>(httpWebResponse);
                }
            });
            
        }
        
        /// <summary>
        /// Create the signature
        /// </summary>
        /// <param name="request">The SignatureCreateRequest object that specifies all the parameters of this operation.</param>
        /// <returns>Returns a SignatureCreateResponse from SMS.</returns>
        public SignatureCreateResponse CreateSignature(SignatureCreateRequest request)
        {
            this.CheckNotNull(request, "object request should not be null");
            this.CheckNotNullOrEmpty(request.Content, "content should not be null or empty");
            this.CheckNotNullOrEmpty(request.ContentType, "contentType should not be null or empty");
            this.CheckNotNullOrEmpty(request.CountryType, "countryType should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Post, 
                new [] {UrlPrefix, "/signatureApply"});
            this.FillRequestBodyForJson(internalRequest, JsonUtils.SerializeObject(request));
            internalRequest.Parameters.Add("clientToken", Guid.NewGuid().ToString());
            
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<SignatureCreateResponse>(httpWebResponse);
                }
            });
            
        }
        
        /// <summary>
        /// Update the signature
        /// </summary>
        /// <param name="request">The SignatureUpdateRequest object that specifies all the parameters of this operation.</param>
        public void UpdateSignature(SignatureUpdateRequest request)
        {
            this.CheckNotNull(request, "object request should not be null");
            this.CheckNotNullOrEmpty(request.SignatureId, "signatureId should not be null or empty");
            this.CheckNotNullOrEmpty(request.Content, "content should not be null or empty");
            this.CheckNotNullOrEmpty(request.ContentType, "contentType should not be null or empty");
            this.CheckNotNullOrEmpty(request.CountryType, "countryType should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Put, 
                new [] {UrlPrefix, "/signatureApply", request.SignatureId});
            this.FillRequestBodyForJson(internalRequest, JsonUtils.SerializeObject(request));
            
            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<BceResponseBase>(httpWebResponse);
                }
            });
            
        }
        
        /// <summary>
        /// Delete the signature
        /// </summary>
        /// <param name="signatureId">The unique code identifying the signature.</param>
        public void DeleteSignature(string signatureId)
        {
            this.CheckNotNullOrEmpty( signatureId, "signatureId should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(new BceRequestBase(), BceConstants.HttpMethod.Delete, 
                new [] {UrlPrefix, "/signatureApply", signatureId});
            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<BceResponseBase>(httpWebResponse);
                }
            });
            
        }
        
        /// <summary>
        /// Get the signature
        /// </summary>
        /// <param name="signatureId">The unique code identifying the signature.</param>
        public SignatureResponse GetSignature(string signatureId)
        {
            this.CheckNotNullOrEmpty(signatureId, "signatureId should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(new BceRequestBase(), BceConstants.HttpMethod.Get, 
                new [] {UrlPrefix, "/signatureApply", signatureId});
            
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<SignatureResponse>(httpWebResponse);
                }
            });
            
        }

        /// <summary>
        /// Update quota and rate-limit
        /// </summary>
        /// <param name="request">The QuotaUpdateRequest object that specifies all the parameters of this operation.</param>
        public void UpdateQuotaRate(QuotaUpdateRequest request)
        {
            this.CheckNotNull(request, "object request should not be null");
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Put, 
                new [] {UrlPrefix, "/quota"});
            this.FillRequestBodyForJson(internalRequest, JsonUtils.SerializeObject(request));
            
            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<BceResponseBase>(httpWebResponse);
                }
            });
        }

        /// <summary>
        /// Query quota and rate-limit detail
        /// </summary>
        /// <returns>The QuotaQueryResponse object which includes the detail quota and rate-limit info.</returns>
        public QuotaQueryResponse QueryQuotaRate()
        {
            InternalRequest internalRequest = this.CreateSmsRequest(new BceRequestBase(), BceConstants.HttpMethod.Get, 
                new [] {UrlPrefix, "/quota"});
            internalRequest.Parameters.Add("userQuery", "");
            
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<QuotaQueryResponse>(httpWebResponse);
                }
            });
        }
        
        /// <summary>
        /// Create mobile black
        /// </summary>
        /// <param name="request">The BlackCreateRequest object that specifies all the parameters of this operation.</param>
        public void CreateMobileBlack(BlackCreateRequest request)
        {
            this.CheckNotNull(request, "object request should not be null");
            this.CheckNotNullOrEmpty(request.Phone, "phone should not be null or empty");
            this.CheckNotNullOrEmpty(request.Type, "type should not be null or empty");
            if ("SignatureBlack".Equals(request.Type)) {
                this.CheckNotNullOrEmpty(request.SignatureIdStr,
                    "signatureIdStr should not be null or empty, when 'type' is 'SignatureBlack'.");
                this.CheckNotNullOrEmpty(request.SmsType,
                    "smsType should not be null or empty, when 'type' is 'SignatureBlack'.");
            }
            
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Post, 
                new [] {UrlPrefix, "/blacklist"});
            this.FillRequestBodyForJson(internalRequest, JsonUtils.SerializeObject(request));
            
            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<BceResponseBase>(httpWebResponse);
                }
            });
        }
        
        /// <summary>
        /// Delete Mobile Black
        /// </summary>
        /// <param name="phones">Support multiple mobile phone numbers, up to 200 maximum, separated by comma.</param>
        public void DeleteMobileBlack(string phones)
        {
            this.CheckNotNullOrEmpty(phones, "phones should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(new BceRequestBase() ,BceConstants.HttpMethod.Delete, 
                new [] {UrlPrefix, "/blacklist/delete"});
            internalRequest.Parameters.Add("phones", phones);
            internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<BceResponseBase>(httpWebResponse);
                }
            });
        }
        
        /// <summary>
        /// Get Black List
        /// </summary>
        /// <param name="request">The BlackListRequest object that specifies all the parameters of this operation.</param>
        /// <returns>The BlackListResponse object which includes the detail of mobileBlack.</returns>
        public BlackListResponse GetMobileBlack(BlackListRequest request)
        {
            this.CheckNotNull(request, "object should not be null");
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Get, 
                new [] {UrlPrefix, "/blacklist"});
            internalRequest.Parameters.Add("pageNo", request.PageNo.ToString());
            internalRequest.Parameters.Add("pageSize", request.PageSize.ToString());
            internalRequest.Parameters.Add("blackSource", "Console");
            if (!string.IsNullOrEmpty(request.Phone))
            {
                internalRequest.Parameters.Add("phone", request.Phone);
            }
            if (!string.IsNullOrEmpty(request.SmsType))
            {
                internalRequest.Parameters.Add("smsType", request.SmsType);
            }
            if (!string.IsNullOrEmpty(request.SignatureIdStr))
            {
                internalRequest.Parameters.Add("signatureId", request.SignatureIdStr);
            }
            if (!string.IsNullOrEmpty(request.StartTime))
            {
                internalRequest.Parameters.Add("startTime", request.StartTime);
            }
            if (!string.IsNullOrEmpty(request.EndTime))
            {
                internalRequest.Parameters.Add("endTime", request.EndTime);
            }
            
            
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<BlackListResponse>(httpWebResponse);
                }
            });
        }

        /// <summary>
        /// Query Statistics By Template
        /// </summary>
        /// <param name="request">The StatisticsTemplateListRequest object that specifies all the parameters of this operation.</param>
        /// <returns>The StatisticsTemplateListResponse object which includes the detail of statistics by template.</returns>
        public StatisticsTemplateListResponse GetTemplateStatistics(StatisticsTemplateListRequest request)
        {
            this.CheckNotNull(request, "object should not be null");
            this.CheckNotNullOrEmpty(request.StartTime, "startTime should not be null or empty");
            this.CheckNotNullOrEmpty(request.EndTime, "endTime should not be null or empty");
            
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Get, 
                new [] {UrlPrefix, "/openapi/statisticTemplate"});
            internalRequest.Parameters.Add("startTime", request.StartTime);
            internalRequest.Parameters.Add("endTime", request.EndTime);
            internalRequest.Parameters.Add("pageNo", request.PageNo.ToString());
            internalRequest.Parameters.Add("pageSize", request.PageSize.ToString());
            if (!string.IsNullOrEmpty(request.TemplateId))
            {
                internalRequest.Parameters.Add("templateId", request.TemplateId);
            }
            
            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                HttpWebResponse httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<StatisticsTemplateListResponse>(httpWebResponse);
                }
            });
        }
        
        /// <summary>
        /// Get statistics data as a list
        /// </summary>
        /// <param name="request">The ListStatisticsRequest object that specifies
        /// all the parameters of this operation.</param>
        /// <returns>The ListStatisticsResponse object which includes a list of statistics data.</returns>
        public ListStatisticsResponse GetListStatistics(ListStatisticsRequest request)
        {
            this.CheckNotNull(request, "statistics request should not be null");
            this.CheckNotNullOrEmpty(request.StartTime, 
                "statistics request start time should not be null or empty");
            this.CheckNotNullOrEmpty(request.EndTime,
                "statistics request end time should not be null or empty");

            
            InternalRequest internalRequest = this.CreateSmsRequest(request, BceConstants.HttpMethod.Get, 
                new [] {UrlPrefix, "/summary"});
            internalRequest.Parameters.Add("startTime", request.StartTime + " 00:00:00");
            internalRequest.Parameters.Add("endTime", request.EndTime + " 23:59:59");
            internalRequest.Parameters.Add("smsType", "all");
            internalRequest.Parameters.Add("dimension", "day");
            internalRequest.Parameters.Add("countryType", "domestic");

            if (!string.IsNullOrEmpty(request.SignatureId))
            {
                internalRequest.Parameters.Add("signatureId", request.SignatureId);
            }
            
            if (!string.IsNullOrEmpty(request.TemplateCode))
            {
                internalRequest.Parameters.Add("templateCode", request.TemplateCode);
            }
            
            if (!string.IsNullOrEmpty(request.SmsType))
            {
                internalRequest.Parameters["smsType"] = request.SmsType;
            }
            
            if (!string.IsNullOrEmpty(request.CountryType))
            {
                internalRequest.Parameters["countryType"] = request.CountryType;
            }

            return internalRequest.Config.RetryPolicy.Execute(attempt =>
            {
                var httpWebResponse = this.httpClient.Execute(internalRequest);
                using (httpWebResponse)
                {
                    return this.ToObject<ListStatisticsResponse>(httpWebResponse);
                }
            });
        }
    }
}

