using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using Topsis.Application.Contracts.Security;

namespace Topsis.Web.Pages
{
    public static class RecaptchaActions
    {
        public const string LOGIN = "LOGIN";
        public const string REGISTER = "REGISTER";
        public const string FORGOT_PASSWORD = "FORGOT_PASSWORD";
        public const string GUEST_REGISTER = "GUEST_REGISTER";

        /// <summary>
        /// Check if its a recaptcha valid request.
        /// </summary>
        /// <param name="recaptcha"></param>
        /// <param name="token"></param>
        /// <param name="modelState"></param>
        /// <returns>The error message if request is invalid, otherwise null.</returns>
        internal static async Task<string> ValidateAsync(this IRecaptchaService recaptcha, string token, string action)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return "Recaptcha validation failed (1).";
            }

            var success = await recaptcha.ValidateRecaptchaAsync(token, action);
            return success ? null : "Recaptcha validation failed (2).";
        }
    }
}
