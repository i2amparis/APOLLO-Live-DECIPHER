using System.ComponentModel;
using Topsis.Domain.Common;

namespace Topsis.Domain.Contracts
{
    public enum Gender : short
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female = 2,
        [Description("I prefer not to say")]
        NotAnswered = 3,
        [Description("Other")]
        Other = EnumHelper.Other
    }
}
