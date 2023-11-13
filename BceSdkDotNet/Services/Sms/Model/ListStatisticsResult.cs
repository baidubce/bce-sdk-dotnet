namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// The detail of result list
    /// </summary>
    public class ListStatisticsResult
    {
        /// <summary>
        /// Statistics date, like "2020-04-21" or "合计".
        /// </summary>
        public string Datetime { get; set; }
        
        /// <summary>
        /// The Country ISO Code ALPHA-2 standard, like "CN", "US".
        /// </summary>
        public string CountryAlpha2Code { get; set; }
        
        /// <summary>
        /// The count number of submitted messages.
        /// </summary>
        public string SubmitCount { get; set; }
        
        /// <summary>
        /// The count number of submitted long messages.
        /// </summary>
        public string SubmitLongCount { get; set; }
        
        /// <summary>
        /// The count of success response of sending messages.
        /// </summary>
        public string ResponseSuccessCount { get; set; }
        
        /// <summary>
        /// The percentage of success response of sending messages.
        /// </summary>
        public string ResponseSuccessProportion { get; set; }
        
        /// <summary>
        /// The count number of delivered successfully messages.
        /// </summary>
        public string DeliverSuccessCount { get; set; }
        
        /// <summary>
        /// The count number of delivered successfully long messages.
        /// </summary>
        public string DeliverSuccessLongCount { get; set; }
        
        /// <summary>
        /// The percentage of successfully delivered messages. 
        /// </summary>
        public string DeliverSuccessProportion { get; set; }
        
        /// <summary>
        /// The count number of delivered unsuccessfully messages.
        /// </summary>
        public string DeliverFailureCount { get; set; }
        
        /// <summary>
        /// The percentage of unsuccessfully delivered messages. 
        /// </summary>
        public string DeliverFailureProportion { get; set; }
        
        /// <summary>
        /// The percentage of receipts received.
        /// </summary>
        public string ReceiptProportion { get; set; }
        
        /// <summary>
        /// The count number of unknown deliver result messages.
        /// </summary>
        public string UnknownCount { get; set; }
        
        /// <summary>
        /// The percentage of unknown deliver result messages
        /// </summary>
        public string UnknownProportion { get; set; }
        
        /// <summary>
        /// The count of timeout response of sending messages
        /// </summary>
        public string ResponseTimeoutCount { get; set; }
        
        /// <summary>
        /// The count of failure due to unknown errors
        /// </summary>
        public string UnknownErrorCount { get; set; }
        
        /// <summary>
        /// The count of failure due to cell phone numbers not existed
        /// </summary>
        public string NotExistCount { get; set; }
        
        /// <summary>
        /// The count of failure due to signature or template error
        /// </summary>
        public string SignatureOrTemplateCount { get; set; }
        
        /// <summary>
        /// The count of failure due to carriers network error
        /// </summary>
        public string AbnormalCount { get; set; }
        
        /// <summary>
        /// The count of failure due to gateway submitting overclocking
        /// </summary>
        public string OverclockingCount { get; set; }
        
        /// <summary>
        /// The count of failure due to other error
        /// </summary>
        public string OtherErrorCount { get; set; }
        
        /// <summary>
        /// The count of failure due to blacklisting cell phone numbers
        /// </summary>
        public string BlacklistCount { get; set; }
        
        /// <summary>
        /// The count of failure due to message route or channel
        /// </summary>
        public string RouteErrorCount { get; set; }
        
        /// <summary>
        /// The count of failure due to carriers submitting error
        /// </summary>
        public string IssueFailureCount { get; set; }
        
        /// <summary>
        /// The count of failure due to parameters error
        /// </summary>
        public string ParameterErrorCount { get; set; }
        
        /// <summary>
        /// The count of failure due to illegal words
        /// </summary>
        public string IllegalWordCount { get; set; }
        
        /// <summary>
        /// The count of failure due to cell phone device abnormal
        /// </summary>
        public string AnomalyCount { get; set; }
    }
}