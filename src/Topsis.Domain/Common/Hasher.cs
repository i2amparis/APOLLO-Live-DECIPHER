using HashidsNet;

namespace Topsis.Domain.Common
{
    public static class Hasher
    {
        private const int MinimumHashLength = 7;
        private const string Salt = "you can put anything here but don't change it";
        private static Hashids Hashids = new Hashids(Salt, MinimumHashLength);
        
        public static string Hash(this int id)
        {
            return Hashids.Encode(id);
        }

        public static int[] DehashInts(this string hash)
        {
            return Hashids.Decode(hash);
        }
    }
}
