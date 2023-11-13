using System.Collections.Generic;
using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Query template statistics response result data
    /// </summary>
    public class StatisticsTemplateListResponse : BceResponseBase
    {
        /// <summary>
        /// The status of response.
        /// </summary>
        public int status { get; set; }
        
        /// <summary>
        /// The status`s description  of response.
        /// </summary>
        public string msg { get; set; }
        
        /// <summary>
        /// The data  of response.
        /// </summary>
        public StatisticsTemplate data { get; set; }
    }
    
    /// <summary>
    /// The dataList of response.
    /// </summary>
    public class StatisticsTemplate
    {
        /// <summary>
        /// The total number of submit.
        /// </summary>
        public long SubmitTotal { get; set; }
        
        /// <summary>
        /// The total number of deliver.
        /// </summary>
        public long DeliverTotal { get; set; }
        
        /// <summary>
        /// The current page number
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// The current page number.
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Statistics detail of per template 
        /// </summary>
        public List<TemplateItem> Items { get; set; }
    }

    /// <summary>
    /// list item
    /// </summary>
    public class TemplateItem
    {
        /// <summary>
        /// The unique code identifying a template.
        /// </summary>
        public string TemplateId { get; set; }
        
        /// <summary>
        /// The count of submit.
        /// </summary>
        public long SubmitCount { get; set; }
        
        /// <summary>
        /// The count of deliver.
        /// </summary>
        public long DeliverCount { get; set; }
    }
}