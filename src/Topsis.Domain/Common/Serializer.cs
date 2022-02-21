using Newtonsoft.Json;
using System;

namespace Topsis.Domain.Common
{
    public static class Serializer
    {
        private static JsonSerializerSettings Default = new JsonSerializerSettings
        {
            FloatParseHandling = FloatParseHandling.Double
        };

        public static string SerializeToJson(object obj, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(obj, settings ?? Default);
        }

        public static T DeserializeFromJson<T>(string json, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(json, settings ?? Default);
        }
    }

    public static class Rounder
    {
        public static double Round(double d, int decimals = 6)
        {
            return Math.Round(d, decimals);
        }
    }
}
