using Microsoft.Extensions.Localization;
using System.Reflection;
using Topsis.Web.Resources;

namespace Topsis.Web.Infrastructure
{
    public class CommonLocalizationService
    {
        private readonly IStringLocalizer localizer;

        public CommonLocalizationService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
            localizer = factory.Create(nameof(SharedResource), assemblyName.Name);
        }

        public string Get(string key)
        {
            return localizer[key];
        }
    }
}
