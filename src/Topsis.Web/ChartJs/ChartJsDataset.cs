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
            Type = null;
            Options = null;
        }

        [JsonProperty("options")]
        public ChartJsDatasetOptions Options { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

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

    public class ChartJsDatasetOptions
    {
        [JsonProperty("elements")]
        public ChartJsElement Elements { get; set; }

        public class ChartJsElement
        { 
            [JsonProperty("bar")]
            public ChartJsBarElement Bar { get; set; }
        }

        public class ChartJsBarElement
        {
            public ChartJsBarElement(string jsFunctionBackgroundColor)
            {
                BackgroundColor = jsFunctionBackgroundColor;
            }

            [JsonProperty("backgroundColor")]
            public string BackgroundColor { get; set; }
        }
    }
}
