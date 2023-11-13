using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// Some query template statistics request parameters
    /// </summary>
    public class StatisticsTemplateListRequest : BceRequestBase
    {
        /// <summary>
        /// startTime of query, the format is yyyyMMdd. (e.g. 20181225)
        /// </summary>
        public string StartTime { get; set; }
        
        /// <summary>
        /// endTime of query, the format is yyyyMMdd. (e.g. 20181226)
        /// </summary>
        public string EndTime { get; set; }
        
        /// <summary>
        /// The unique code identifying a template. (e.g. sms-tmpl-1123)
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// The current page number
        /// </summary>
        public int PageNo { get; set; } = 1;

        /// <summary>
        /// The current page number. range from 1 to 50
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}