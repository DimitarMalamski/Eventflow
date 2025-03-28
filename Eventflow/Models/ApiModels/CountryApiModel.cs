namespace Eventflow.Models.ApiModels
{
    public class CountryApiModel
    {
        public Name name { get; set; }
        public string region { get; set; }

        public class Name
        {
            public string common { get; set; }
        }
    }
}
