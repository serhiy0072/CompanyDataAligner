using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class Address
{
    public string? OriginalAddress { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public Address(string fullAddress, string countryId)
    {
        OriginalAddress = fullAddress;
        ParseAddress(fullAddress, countryId);
    }

    private static readonly Dictionary<string, string> CountryTranslations = new Dictionary<string, string>
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

    private void ParseAddress(string fullAddress, string countryId)
    {
        // Нормалізуємо адресу, видаляючи зайві пробіли та коми
        string normalizedAddress = CorrectCommonMistakes(fullAddress.ToUpper().Replace(",", " ").Trim()).Replace("-", " ").Replace(".", " ");
        normalizedAddress = Regex.Replace(normalizedAddress, @"\s+", " ").Trim(); // Видаляємо зайві пробіли

        // Знайдемо країну
        Country = CountryTranslations
            .FirstOrDefault(entry => normalizedAddress.Contains(entry.Key, StringComparison.OrdinalIgnoreCase))
            .Value ?? GetCountryById(countryId);

        // Видаляємо всі входження назви країни з адреси
        if (!string.IsNullOrEmpty(Country))
        {
            foreach (var entry in CountryTranslations.Where(entry => entry.Value == Country))
            {
                // Поки є входження назви країни, замінюємо їх на порожній рядок
                while (normalizedAddress.IndexOf(entry.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    normalizedAddress = normalizedAddress.Replace(entry.Key, string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
                }
            }
        }

        // Шукаємо поштовий індекс у будь-якій частині адреси
        var postalCodeMatch = Regex.Match(normalizedAddress, @"\b\d{5}\b");
        if (postalCodeMatch.Success)
        {
            PostalCode = postalCodeMatch.Value;

            // Вулиця - це все, що передує поштовому індексу
            Street = normalizedAddress.Substring(0, postalCodeMatch.Index).Trim();
            // Якщо вулиця не знайдена, вважаємо, що вулиця - це все, що за поштовим індексом
            Street = string.IsNullOrEmpty(Street)
                ? normalizedAddress.Substring(postalCodeMatch.Index + PostalCode.Length).Trim()
                : Street;

            //Пошук міста по індексу поки лише для Німеччини
            var geoObject = GeoNamesData.GeoNamesList.Where(g => g.PostalCode == PostalCode).OrderBy(g => g.CityName.Length).FirstOrDefault();
            City = geoObject?.CityName ?? City;
            if (!string.IsNullOrEmpty(Street) && geoObject != null)
            {
                Street = Street.Replace(geoObject.RegionNameDE, "", StringComparison.OrdinalIgnoreCase);
            }
        }
        else
        {
            // Якщо поштовий індекс не знайдено, все після видалення країни вважається містом
            City = normalizedAddress.Trim();
            var matchingCity = GeoNamesData.GeoNamesList
                .Select(g => g.CityName)
                .FirstOrDefault(cityName => City.Contains(cityName, StringComparison.OrdinalIgnoreCase));

            if (matchingCity != null)
            {
                Street = City;
                City = matchingCity;
            }
        }

        if (!string.IsNullOrEmpty(Street) && !string.IsNullOrEmpty(City))
        {
            Street = Street.Replace(City, "", StringComparison.OrdinalIgnoreCase);
            Street = Regex.Replace(Street, @"\s+", " ").Trim(); // Видаляємо зайві пробіли
        }
    }
    private string GetCountryById(string countryId)
    {
        // Створення словника CountryId -> Country Name
        var countryMap = new Dictionary<string, string>
        {
            { "276", "GERMANY" },
            { "840", "USA" },
            { "250", "FRANCE" },
            { "380", "ITALY" },
            // Додайте інші коди країн за потребою
        };
        return countryMap.TryGetValue(countryId, out var countryName) ? countryName : "GERMANY";
    }

    static string CorrectCommonMistakes(string input)
    {
        // Виправляємо типові помилки
        Dictionary<string, string> corrections = new Dictionary<string, string>
        {
            { "LTD.", "LTD" },
            { "PLCK", "PLANK" },
            { "STR.", " STRASSE " },
            { "STRA?E", " STRASSE " },
            { "STRASSE", " STRASSE " },
            { "ШТРАСЕ", " STRASSE " },
            { "АМ АМАЗОНЕНВЕРК", " AMAZONENWERK " },
            { "ВІЛЬХЕЛЬМШТРАССЕ", " WILHELMSЕTRASSE " },

            { "WIESB", "WIESBADEN " },
            { "WIESBADENADEN", "WIESBADEN " },
            { "WIESBADEN ADEN", "WIESBADEN " },

            { "ГАМБУРГ", "HAMBURG " },
            { "ГАСБЕРГЕН", "HASBERGEN " },
            { "ХАСБЕРГЕН", "HASBERGEN " },
            { "ФРАНКФУРТ НА МАЙНІ", "ФРАНКФУРТ " },
            { "МООРВЕРДЕР", "MOORWERDER " },
            { "КЮНЦЕЛЬЗАУ", "KUNZELSAU " },
            { "НОРДЕРДАЙХ", "NORDERDEICH " },
            { "МЮЛЕНВІНКЕЛЬ", "MUEHLENWINKEL " },
            { "ХАРЗЕВІНКЕЛЬ", "HARSEWINKEL " },
            { "РАЙНХОЛЬ", "REINHOL " },
            { "ЕННЕПЕТАЛЬ", "ENNEPETAL " },

            { "ВЮРТ", "WURTH " },
            { "DELKENHEIM", " " },
            { "DELKENHEIMGERMANY", "DELKENHEIM GERMANY " },
            { "BUNDESLAND", " " },
            { "STATE", " " },
            { "ГАСТЕ", " " },

            { "М.", " " },
            { "ВУЛ.", " " },
            { "(", "" },
            { ")", "" },
            { " BAVARIA ", " " },
            { "«", "" },
            { "»", "" },
            { "& ", "&" },
            { " &", "&" },
            { "  &  ", "&" },
            { "D-", " " },
            { "Д-", " " },
            { "<", " " },
            { ">", " " },
            // Додайте інші типові заміни тут
        };

        foreach (var correction in corrections)
        {
            input = input.Replace(correction.Key, correction.Value);
            input = Regex.Replace(input, @"\s+", " ");
        }
        return input;
    }

    public override bool Equals(object obj)
    {
        if (obj is Address other)
        {
            // Порівняння всіх ключових властивостей. Якщо всі властивості збігаються, об'єкти рівні.
            return (Street == other.Street && City == other.City && Country == other.Country) ||
                   (Street == other.Street && PostalCode == other.PostalCode && Country == other.Country) ||
                   (Street == other.Street && PostalCode == other.PostalCode && City == other.City && Country == other.Country);
        }
        return false;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Street?.GetHashCode() ?? 0,
            PostalCode?.GetHashCode() ?? 0,
            City?.GetHashCode() ?? 0,
            Country?.GetHashCode() ?? 0
        );
    }

    public override string ToString()
    {
        var parts = new List<string> { $"str. {Street}", $"postCode: {PostalCode}", $"City: {City}", $"country: {Country}" };
        return string.Join(", ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }
}
