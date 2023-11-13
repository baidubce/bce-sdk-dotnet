using System.Collections.Generic;
using BaiduBce.Model;
using Newtonsoft.Json;

namespace BaiduBce.Services.Sms.Model
{
	/// <summary>
	/// SMS request parameters
	/// </summary>
	public class SendMessageRequest : BceRequestBase
	{

        /// <summary>
        /// The target mobile phone number, use "," to split if you have multiple targets
		/// e.g. 138001380000,138001380001
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// The unique code identifying message content template, which can be obtained from cloud.baidu.com
        /// e.g. sms-tmpl-xIEkZG87760
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// The unique code identifying message content signature, which can be obtained from cloud.baidu.com
        /// <para>Signature is generally the text in "【xxx】" in front of the message</para>
        /// e.g. sms-sign-AfFLyb67322
        /// </summary>
        public string SignatureId { get; set; }

        /// <summary>
        /// ContentVar is a hash map which indicates the correspondence of template parameter name to parameter value.
        /// </summary>
        public Dictionary<string, string> ContentVar { get; set; }

        /// <summary>
        /// optional;
        /// The user self defined param
        /// </summary>
        public string Custom { get; set; }

        /// <summary>
        /// optional;
        /// The user self defined channel code
        /// </summary>
        public string UserExtId { get; set; }

        /// <summary>
        /// optional;
        /// The id of callback url specified by user
        /// </summary>
        [JsonProperty(PropertyName= "merchantUrlId")]
        public string CallbackUrlId { get; set; }

        /// <summary>
        /// optional;
        /// The parameter for idempotence of http post
        /// </summary>
        public string ClientToken { get; set; }

	}
}

