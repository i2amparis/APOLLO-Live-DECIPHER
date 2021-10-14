using Newtonsoft.Json;
using System.Collections.Generic;

namespace Topsis.Web.ChartJs
{
    public class ChartJsData
    {

        [JsonProperty("labels")]
        public List<string> Labels { get; set; }
        [JsonProperty("datasets")]
        public object[] Datasets { get; set; }
    }
}
