using System;

namespace Topsis.Application.Contracts.Identity
{
    public class AdminSettings
    {
        public string Email { get; set; }
        public string InitialPassword { get; set; }

        public string GetPassword()
        {
            return string.IsNullOrEmpty(InitialPassword) 
                ? $"pW{new Random().Next(100_000, 999_999)}" 
                : InitialPassword;
        }
    }
}
