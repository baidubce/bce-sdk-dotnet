using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Create the response result data for the signed request
    /// </summary>
    public class SignatureCreateResponse : BceResponseBase
    {
        /// <summary>
        /// The unique code identifying a signature.
        /// </summary>
        public string SignatureId { get; set; }
        
        /// <summary>
        /// Approval status of the signature
        /// </summary>
        public string Status { get; set; }
    }
}