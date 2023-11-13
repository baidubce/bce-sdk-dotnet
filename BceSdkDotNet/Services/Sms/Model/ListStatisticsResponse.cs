using System.Collections.Generic;
using BaiduBce.Model;

namespace BaiduBce.Services.Sms.Model
{
    /// <summary>
    /// The response of fetching statistics data
    /// </summary>
    public class ListStatisticsResponse : BceResponseBase
    {
        
        /// <summary>
        /// The statistics result list
        /// </summary>
        public List<ListStatisticsResult> StatisticsResults { get; set; }
    }
}