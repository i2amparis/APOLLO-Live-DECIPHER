using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Topsis.Domain.Common
{
    public static class EnumHelper
    {
        public const short Other = 100;

        public static string GetDescription<T>(this T source)
        {
            var fi = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }

        public static Dictionary<short, string> GetDictionary<TEnum>()
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            return values.ToDictionary(x => Convert.ToInt16(x), x => x.GetDescription());
        }
    }
}
