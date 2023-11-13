using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Some request parameters to create templates
    /// </summary>
    public class TemplateCreateRequest : BceRequestBase
    {
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
        /// The value of countryType could be DOMESTIC or INTERNATIONAL or GLOBAL.
        /// DOMESTIC means the template can only be used in Mainland China.
        /// INTERNATIONAL means the template can only be used out of Mainland China.
        /// GLOBAL means the template can only be used all over the world.
        /// </summary>
        public string CountryType { get; set; }

        /// <summary>
        /// Description of the template
        /// </summary>
        public string Description { get; set; } = "";
    }
}