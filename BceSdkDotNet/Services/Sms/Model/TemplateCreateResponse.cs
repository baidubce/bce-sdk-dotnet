using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// The result of the response to the request to create the template
    /// </summary>
    public class TemplateCreateResponse : BceResponseBase
    {
        /// <summary>
        /// The unique code identifying a template
        /// </summary>
        public string TemplateId { get; set; }
        
        /// <summary>
        /// Approval status of a template
        /// </summary>
        public string Status { get; set; }
    }
}