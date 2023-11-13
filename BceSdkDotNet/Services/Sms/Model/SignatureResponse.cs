using System;
using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// The response data which query the signature details.
    /// </summary>
    public class SignatureResponse : BceResponseBase
    {
        /// <summary>
        /// The unique code identifying the signature.
        /// </summary>
        public string SignatureId { get; set; }
        
        /// <summary>
        /// Owner's Baidu Cloud account id of the signature.
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Text content of the signature.
        /// e.g. Baidu
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// The type of the signature.
        /// The value of contentType could be Enterprise or MobileApp or Web or WeChatPublic or Brand or Else.
        /// </summary>
        public string ContentType { get; set; }
        
        /// <summary>
        /// Description of the signature.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Approval status of the template.
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// The countryType indicates the countries or regions in which the signature can be used.
        /// The value of countryType could be DOMESTIC or INTERNATIONAL or GLOBAL.
        /// DOMESTIC means the signature can only be used in Mainland China;
        /// INTERNATIONAL means the signature can only be used out of Mainland China;
        /// GLOBAL means the signature can only be used all over the world.
        /// </summary>
        public string CountryType { get; set; }
        
        /// <summary>
        /// Review opinion of the signature.
        /// </summary>
        public string Review { get; set; }
        
        /// <summary>
        /// The create date of the signature.
        /// </summary>
        public DateTime CreateDate { get; set; }
        
        /// <summary>
        /// The update date of the signature.
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}