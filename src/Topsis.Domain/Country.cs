namespace Topsis.Domain
{
    public class Country : EntityWithTitle<string>
    {
        public Country()
        {
        }

        public Country(string title, string a2Code, string a3Code, string id) 
            : base(id, title)
        {
            A2Code = a2Code;
            A3Code = a3Code;
        }

        public string A3Code { get; set; }
        public string A2Code { get; set; }
    }
}
