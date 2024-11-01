using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    public static class AddressHelper
    {
        public static string RemoveCountryFromAddress(string address, string? country)
        {
            if (string.IsNullOrEmpty(country)) return address;
            return CountryHelper.CountryTranslations.Where(entry => entry.Value == country)
                .Aggregate(address, (current, entry) => current.Replace(entry.Key, string.Empty, StringComparison.OrdinalIgnoreCase).Trim());
        }

        public static int FindPostalCode(string address, out string postalCode)
        {
            var match = Regex.Match(address, @"\b\d{5}\b");
            postalCode = match.Success ? match.Value : string.Empty;
            return match.Success ? match.Index : -1;
        }

        public static string ExtractStreet(string address, int postalCodeIndex, string postalCode)
        {
            var street = address.Substring(0, postalCodeIndex).Trim();
            return string.IsNullOrEmpty(street)
                ? address.Substring(postalCodeIndex + postalCode.Length).Trim()
                : street;
        }

        public static string FindCityInAddress(string address)
        {
            return GeoNamesData.GeoNamesList
                .Select(g => g.CityName)
                .FirstOrDefault(cityName => address.Contains(cityName, StringComparison.OrdinalIgnoreCase)) ?? address;
        }

        public static string FindStreet(string originalAddress, string? city, string? country, string? postalCode)
        {
            var knownElements = new List<string> { city, country, postalCode };
            foreach (var element in knownElements.Where(e => !string.IsNullOrEmpty(e)))
            {
                originalAddress = originalAddress.Replace(element!, "", StringComparison.OrdinalIgnoreCase);
            }

            return originalAddress.Trim();
        }
    }
}