using System.Collections.Generic;
using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Query the response results of the mobile phone number blacklist data list.
    /// </summary>
    public class BlackListResponse : BceResponseBase
    {
        /// <summary>
        /// Total results
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// current page
        /// </summary>
        public int PageNo { get; set; }
        
        /// <summary>
        /// perPage Size
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// result list
        /// </summary>
        public List<BlackDetail> Blacklists { get; set; }
    }
    
    /// <summary>
    /// The data item contains the detailed data of each item in the list.
    /// </summary>
    public class BlackDetail
    {
        /// <summary>
        /// black phone
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Black type, The value of type could be MerchantBlack or SignatureBlack
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// SmsType, the limited smsType when send sms
        /// </summary>
        public string SmsType { get; set; }
        
        /// <summary>
        /// The unique code identifying a signature.
        /// </summary>
        public string SignatureIdStr { get; set; }
        
        /// <summary>
        /// black updateDate
        /// </summary>
        public string UpdateDate { get; set; }
    }
}