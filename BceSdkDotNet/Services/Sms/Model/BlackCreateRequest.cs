using System;
using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Provide parameters to create a mobile number blacklist.
    /// </summary>
    public class BlackCreateRequest : BceRequestBase
    {
        /// <summary>
        /// Type of black, The value of type could be MerchantBlack or SignatureBlack.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Mobile of black, Support multiple mobile phone numbers, up to 200 maximum, separated by comma.
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// The unique code identifying a signature.
        /// When the value of "type" is "SignatureBlack", this field is required.
        /// </summary>
        public string SignatureIdStr { get; set; }
        
        /// <summary>
        /// SmsType, When the value of "type" is "SignatureBlack", this field is required.
        /// </summary>
        public string SmsType { get; set; }
    }
}