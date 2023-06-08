using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class DataPieChartAjaxModel
    {
        [JsonProperty(PropertyName = "labels")]
        public List<string> Labels { get; set; }
        [JsonProperty(PropertyName = "datasets")]
        public List<DatasetPieChart> Datasets { get; set; }
    }

    public class DatasetPieChart
    {
        [JsonProperty(PropertyName = "data")]
        public List<int> Data { get; set; }
        [JsonProperty(PropertyName = "backgroundColor")]
        public List<string> BackgroundColor { get; set; }
        [JsonProperty(PropertyName = "hoverBackgroundColor")]
        public List<string> HoverBackgroundColor { get; set; }
        [JsonProperty(PropertyName = "hoverBorderColor")]
        public string HoverBorderColor { get; set; }
    }
}