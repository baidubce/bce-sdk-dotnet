using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// The request to fetch message statistics data as list format.
    /// </summary>
    public class ListStatisticsRequest : BceRequestBase
    {
        /// <summary>
        /// Queried short message type, default "all".
        /// </summary>
        public string SmsType { get; set; }
        
        /// <summary>
        /// The ID of message signature, optional.
        /// </summary>
        public string SignatureId { get; set; }
        
        /// <summary>
        /// Template code, optional.
        /// Example: sms-tmpl-xxxxxxxx.
        /// </summary>
        public string TemplateCode { get; set; }
        
        /// <summary>
        /// Queried countries or regions in which the signature can be used.
        /// Available values: "domestic", "international".
        /// </summary>
        public string CountryType { get; set; }
        
        /// <summary>
        /// The start of queried time condition, required.
        /// Format: "yyyy-MM-dd"
        /// </summary>
        public string StartTime { get; set; }
        
        /// <summary>
        /// The end of queried time condition, required.
        /// Format: "yyyy-MM-dd"
        /// </summary>
        public string EndTime { get; set; }
    }
}