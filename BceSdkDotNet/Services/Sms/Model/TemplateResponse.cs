using System;
using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Query the response result of template details
    /// </summary>
    public class TemplateResponse : BceResponseBase
    {
        /// <summary>
        /// The unique code identifying the template
        /// </summary>
        public string TemplateId { get; set; }
        
        /// <summary>
        /// Owner's Baidu Cloud account id of the template
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Template`s name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The content of template
        /// e.g. Your verify code is ${code}, it will be expired in ${number} minutes.
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// The sms type of the template content
        /// All the sms types can be obtained from cloud.baidu.com
        /// </summary>
        public string SmsType { get; set; }
        
        /// <summary>
        /// The countryType indicates the countries or regions in which the template can be used.
        /// </summary>
        public string CountryType { get; set; }
        
        /// <summary>
        /// Approval status of the template.
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Description of the template.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Review opinion of the template.
        /// </summary>
        public string Review { get; set; }
        
        /// <summary>
        /// The create date of the template.
        /// </summary>
        public DateTime CreateDate { get; set; }
        
        /// <summary>
        /// The update date of the template.
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }
}