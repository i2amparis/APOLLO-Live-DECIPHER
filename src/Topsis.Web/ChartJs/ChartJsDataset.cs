using Newtonsoft.Json;
using System.Collections.Generic;

namespace Topsis.Web.ChartJs
{
    public class ChartJsDataset : ChartJsDataset<double>
    { 
    }

    public class ChartJsDatasetXY : ChartJsDataset<ChartJsXY>
    {
    }

    public struct ChartJsXY
    {
        public ChartJsXY(double x, double y)
        {
            X = x;
            Y = y;
        }

        [JsonProperty("x")]
        public double X { get; }
        [JsonProperty("y")]
        public double Y { get; }
    }

    public class ChartJsDataset<T>
    {
        public ChartJsDataset()
        {
            BackgroundColor = "rgb(255, 99, 132)";
            BorderColor = "rgb(255, 99, 132)";
            Data = new List<T>();
            Label = "My Chart";
            Type = null;
            Options = null;
        }

        [JsonProperty("options")]
        public ChartJsDatasetOptions Options { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public List<T> Data { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// "grey" or ["red", "green"]
        /// </summary>
        [JsonProperty("backgroundColor")]
        public object BackgroundColor { get; set; }
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

        [JsonProperty("scales")]
        public ChartJsScales Scales { get; set; }

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

        public class ChartJsScales
        {
            public ChartJsScales(string xLabel, string yLabel)
            {
                XAxes = new[] { new ChartJsAxes(xLabel) };
                YAxes = new[] { new ChartJsAxes(yLabel) };
            }

            [JsonProperty("x")]
            public ChartJsAxes[] XAxes { get; set; }

            [JsonProperty("y")]
            public ChartJsAxes[] YAxes { get; set; }
        }

        public class ChartJsAxes
        {
            public ChartJsAxes(string label)
            {
                Title = new ChartJsTitle(label);
            }

            [JsonProperty("title")]
            public ChartJsTitle Title { get; set; }
        }

        public class ChartJsTitle
        {
            public ChartJsTitle(string label)
            {
                Display = true;
                Text = label;
            }

            [JsonProperty("display")]
            public bool Display { get; set; }
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}
