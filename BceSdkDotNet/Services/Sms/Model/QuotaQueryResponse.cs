using BaiduBce.Model;
using Newtonsoft.Json;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Query response results of quota frequency control.
    /// </summary>
    public class QuotaQueryResponse : BceResponseBase
    {
        /// <summary>
        /// Daily quota, QuotaPerDay indicates the upper limit of the response-success request counts in one natural day.
        /// </summary>
        public int QuotaPerDay { get; set; }
        
        /// <summary>
        /// Monthly quota, QuotaPerMonth indicates the upper limit of the response-success request counts in one natural month.
        /// </summary>
        public int QuotaPerMonth { get; set; }
        
        /// <summary>
        /// Daily quota remaining value
        /// </summary>
        public int QuotaRemainToday { get; set; }
        
        /// <summary>
        /// Monthly Quota Remaining Value
        /// </summary>
        public int QuotaRemainThisMonth { get; set; }
        
        /// <summary>
        /// Daily rate limit with same mobile and signature.
        /// </summary>
        [JsonProperty(PropertyName= "rateLimitPerMobilePerSignByDay")]
        public int RateLimitPerDay { get; set; }
        
        /// <summary>
        /// Hourly limit of requests with same mobile and signature.
        /// </summary>
        [JsonProperty(PropertyName= "rateLimitPerMobilePerSignByHour")]
        public int RateLimitPerHour { get; set; }
        
        /// <summary>
        /// The limit of requests with same mobile and signature in one minute.
        /// </summary>
        [JsonProperty(PropertyName= "rateLimitPerMobilePerSignByMinute")]
        public int RateLimitPerMinute { get; set; }
        
        /// <summary>
        /// <para>RateLimitWhitelist indicates a user is in rate-limit white list or not.</para>
        /// <para>If rateLimitWhitelist is true, SMS will skip counting rate when the user sends SMS request.</para>
        /// </summary>
        public bool RateLimitWhitelist { get; set; }
        
        /// <summary>
        /// Daily quota update apply.
        /// </summary>
        [JsonProperty(PropertyName= "applyQuotaPerDay")]
        public int QuotaPerDayApply { get; set; }
        
        /// <summary>
        /// Monthly quota update apply.
        /// </summary>
        [JsonProperty(PropertyName= "applyQuotaPerMonth")]
        public int QuotaPerMonthApply { get; set; }
        
        /// <summary>
        /// Approval status of Quota apply. (PENDING: checking, PASS: checked pass, FAILURE: checked fail)
        /// </summary>
        [JsonProperty(PropertyName= "applyCheckStatus")]
        public string QuotaApplyCheckStatus { get; set; }
        
        /// <summary>
        /// Quota apply update check reply. (Reason of checked fail)
        /// </summary>
        [JsonProperty(PropertyName= "checkReply")]
        public string QuotaApplyCheckReply { get; set; }
    }
}