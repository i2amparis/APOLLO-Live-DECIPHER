using System;
using System.Threading.Tasks;

namespace Topsis.Application.Contracts.Security
{
    public interface IRecaptchaService
    {
        Task<bool> ValidateRecaptchaAsync(string token, string action);
    }
}
