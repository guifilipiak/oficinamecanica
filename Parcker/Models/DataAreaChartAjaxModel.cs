using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parcker.Models
{
    public class DataAreaChartAjaxModel
    {
        [JsonProperty(PropertyName = "labels")]
        public List<string> Labels { get; set; }
        [JsonProperty(PropertyName = "datasets")]
        public List<DatasetAreaChart> Datasets { get; set; }
    }

    public class DatasetAreaChart 
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "lineTension")]
        public double LineTension { get; set; }
        [JsonProperty(PropertyName = "backgroundColor")]
        public string BackgroundColor { get; set; }
        [JsonProperty(PropertyName = "borderColor")]
        public string BorderColor { get; set; }
        [JsonProperty(PropertyName = "pointRadius")]
        public int PointRadius { get; set; }
        [JsonProperty(PropertyName = "pointBackgroundColor")]
        public string PointBackgroundColor { get; set; }
        [JsonProperty(PropertyName = "pointBorderColor")]
        public string PointBorderColor { get; set; }
        [JsonProperty(PropertyName = "pointHoverRadius")]
        public int PointHoverRadius { get; set; }
        [JsonProperty(PropertyName = "pointHoverBackgroundColor")]
        public string PointHoverBackgroundColor { get; set; }
        [JsonProperty(PropertyName = "pointHoverBorderColor")]
        public string PointHoverBorderColor { get; set; }
        [JsonProperty(PropertyName = "pointHitRadius")]
        public int PointHitRadius { get; set; }
        [JsonProperty(PropertyName = "pointBorderWidth")]
        public int PointBorderWidth { get; set; }
        [JsonProperty(PropertyName = "data")]
        public List<decimal> Data { get; set; }
    }
}