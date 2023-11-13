using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Provide some parameters to query the mobile phone number blacklist data list
    /// </summary>
    public class BlackListRequest : BceRequestBase
    {
        /// <summary>
        /// Mobile of black, Support multiple mobile phone numbers, up to 200 maximum, separated by comma.
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// SmsType, When the value of "type" is "SignatureBlack", this field is required.
        /// </summary>
        public string SmsType { get; set; }
        
        /// <summary>
        /// The unique code identifying a signature.
        /// When the value of "type" is "SignatureBlack", this field is required.
        /// </summary>
        public string SignatureIdStr { get; set; }
        
        /// <summary>
        /// The start of time condition, format is yyyy-MM-dd
        /// </summary>
        public string StartTime { get; set; }
        
        /// <summary>
        /// The end of time condition, format is yyyy-MM-dd
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// The current page number
        /// </summary>
        public int PageNo { get; set; } = 1;

        /// <summary>
        /// The current page size, range from 1 to 999
        /// </summary>
        public int PageSize { get; set; } = 10;
        
    }
}