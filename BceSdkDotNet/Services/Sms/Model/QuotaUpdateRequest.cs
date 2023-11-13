using BaiduBce.Model;
using Newtonsoft.Json;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// The quota frequency control request parameter can be used to modify the quota frequency control of the current user.
    /// </summary>
    public class QuotaUpdateRequest : BceRequestBase
    {
        /// <summary>
        /// <para>Daily quota.</para>
        /// <para>QuotaPerDay indicates the upper limit of the response-success request counts in one natural day.</para>
        /// <para>Set quotaPerDay value to -1 if you want to ignore counting daily quota.</para>
        /// </summary>
        public int QuotaPerDay { get; set; }
        
        /// <summary>
        /// <para>Monthly quota.</para>
        /// <para>QuotaPerDay indicates the upper limit of the response-success request counts in one natural month.</para>
        /// <para>Set quotaPerDay value to -1 if you want to ignore counting monthly quota..</para>
        /// </summary>
        public int QuotaPerMonth { get; set; }
        
        /// <summary>
        /// <para>Daily rate limit with same mobile and signature.</para>
        /// <para>Set rateLimitPerDay value to -1 if you want to ignore counting this rate.</para>
        /// </summary>
        [JsonProperty(PropertyName= "rateLimitPerMobilePerSignByDay")]
        public int RateLimitPerDay { get; set; }
        
        /// <summary>
        /// <para>Hourly rate limit with same mobile and signature.</para>
        /// <para>Set rateLimitPerDay value to -1 if you want to ignore counting this rate.</para>
        /// </summary>
        [JsonProperty(PropertyName= "rateLimitPerMobilePerSignByHour")]
        public int RateLimitPerHour { get; set; }
        
        /// <summary>
        /// <para>Minutely rate limit with same mobile and signature.</para>
        /// <para>Set rateLimitPerDay value to -1 if you want to ignore counting this rate.</para>
        /// </summary>
        [JsonProperty(PropertyName= "rateLimitPerMobilePerSignByMinute")]
        public int RateLimitPerMinute { get; set; }
    }
}