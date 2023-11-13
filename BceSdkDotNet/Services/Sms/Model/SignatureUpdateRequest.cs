using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Contains some parameters for signature update
    /// </summary>
    public class SignatureUpdateRequest : BceRequestBase
    {
        
        /// <summary>
        /// The unique code identifying the signature.
        /// </summary>
        public string SignatureId { get; set; }
        
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
        /// The countryType indicates the countries or regions in which the signature can be used.
        /// The value of countryType could be DOMESTIC or INTERNATIONAL or GLOBAL.
        /// DOMESTIC means the signature can only be used in Mainland China;
        /// INTERNATIONAL means the signature can only be used out of Mainland China;
        /// GLOBAL means the signature can only be used all over the world.
        /// </summary>
        public string CountryType { get; set; }
        
        /// <summary>
        /// The base64 encoding string of the signature certificate picture.
        /// </summary>
        public string SignatureFileBase64 { get; set; }
        
        /// <summary>
        /// The format of the signature certificate picture, which can only be one of JPG、PNG、JPEG.
        /// </summary>
        public string SignatureFileFormat { get; set; }
    }
}