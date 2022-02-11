namespace Topsis.Application.Contracts.Identity
{
    public class GoogleSettings 
    {
        public GoogleSettings()
        {
            DataProtection = new GoogleDataProtection();
        }

        public GoogleDataProtection DataProtection { get; set; }

        public bool IsValid()
        {
            return DataProtection != null
                && string.IsNullOrWhiteSpace(DataProtection.Bucket) == false
                && string.IsNullOrWhiteSpace(DataProtection.Object) == false
                && string.IsNullOrWhiteSpace(DataProtection.KmsKeyName) == false;
        }

        public class GoogleDataProtection
        {
            public string Bucket { get; set; }
            public string Object { get; set; }
            public string KmsKeyName { get; set; }
        }
    }
}
