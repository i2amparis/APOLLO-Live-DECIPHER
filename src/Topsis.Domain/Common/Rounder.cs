using System;

namespace Topsis.Domain.Common
{
    public static class Rounder
    {
        public static double Round(double d, int decimals = 6)
        {
            return Math.Round(d, decimals);
        }
    }
}
