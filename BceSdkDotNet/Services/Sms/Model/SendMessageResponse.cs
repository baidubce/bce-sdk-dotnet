using System.Collections.Generic;
using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
	/// <summary>
	/// Response result data of SMS request
	/// </summary>
	public class SendMessageResponse : BceResponseBase
	{
        /// <summary>
        /// requestId(equals messageId)
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// return code, success: 1000
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// return message description
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Response result list data of SMS request.
        /// </summary>
        public List<SendMessageItem> Data { get; set; }
	}
	
	/// <summary>
	/// Response list data items
	/// </summary>
	public class SendMessageItem
	{
		/// <summary>
		/// The response code corresponding to the mobile
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// The target mobile
		/// </summary>
		public string Mobile { get; set; }

		/// <summary>
		/// The unique id identifying the message
		/// </summary>
		public string MessageId { get; set; }

		/// <summary>
		/// The explain message of the response
		/// </summary>
		public string Message { get; set; }
	}
}

