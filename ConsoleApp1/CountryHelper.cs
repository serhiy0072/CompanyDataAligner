namespace ConsoleApp1
{
    public static class CountryHelper
    {
        public static readonly Dictionary<string, string> CountryTranslations = new Dictionary<string, string>
    {
        { "GERMANY", "GERMANY" },
        { "ФРН", "GERMANY" },
        { "ФЕДЕРАТИВНА РЕСПУБЛІКА НІМЕЧЧИНА", "GERMANY" },
        { "НІМЕЧЧИНА", "GERMANY" },
        { "DEUTSCHLAND", "GERMANY" },
        { "ГЕРМАНИЯ", "GERMANY" },
        { "UKRAINE", "UKRAINE" },
        { "УКРАЇНА", "UKRAINE" },
        { "FRANCE", "FRANCE" },
        { "ФРАНЦІЯ", "FRANCE" },
        { "FRANKREICH", "FRANCE" },
        { "USA", "USA" },
        { "UNITED STATES", "USA" },
        { "СПОЛУЧЕНІ ШТАТИ", "USA" },
        { "ITALY", "ITALY" },
        { "ІТАЛІЯ", "ITALY" },
        { "ITALIA", "ITALY" },
        { "SPAIN", "SPAIN" },
        { "ІСПАНІЯ", "SPAIN" },
        { "ESPAÑA", "SPAIN" }
        // Додайте інші країни за потребою
    };

        public static string GetCountry(string address, string countryId)
        {
            return CountryTranslations.FirstOrDefault(entry => address.Contains(entry.Key, StringComparison.OrdinalIgnoreCase)).Value
                ?? GetCountryById(countryId);
        }

        public static string GetCountryById(string countryId)
        {
            return countryId switch
            {
                "276" => "GERMANY",
                "840" => "USA",
                "250" => "FRANCE",
                _ => "UNKNOWN"
            };
        }
    }
}