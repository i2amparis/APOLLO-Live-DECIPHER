using Newtonsoft.Json;
using System.Collections.Generic;

namespace Topsis.Web.ChartJs
{
    public class ChartJsDataset
    {
        public ChartJsDataset()
        {
            BackgroundColor = "rgb(255, 99, 132)";
            BorderColor = "rgb(255, 99, 132)";
            Data = new List<double>();
            Label = "My Chart";
        }

        [JsonProperty("data")]
        public List<double> Data { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }
        [JsonProperty("borderColor")]
        public string BorderColor { get; set; }
        [JsonProperty("fill")]
        public bool Fill { get; internal set; }
        [JsonProperty("pointBackgroundColor")]
        public string PointBackgroundColor { get; internal set; }
        [JsonProperty("pointBorderColor")]
        public string PointBorderColor { get; internal set; }
        [JsonProperty("pointHoverBackgroundColor")]
        public string PointHoverBackgroundColor { get; internal set; }
        [JsonProperty("pointHoverBorderColor")]
        public string PointHoverBorderColor { get; internal set; }
    }
}
