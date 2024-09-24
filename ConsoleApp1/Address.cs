using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class Address
{
    private static readonly ConcurrentDictionary<string, Address> AddressCache = new ConcurrentDictionary<string, Address>();
    public string? OriginalAddress { get; private set; }
    public string? Street { get; private set; }
    public string? PostalCode { get; private set; }
    public string? City { get; private set; }
    public string? Country { get; private set; }

    public Address(string fullAddress, string countryId)
    {
        OriginalAddress = fullAddress;
        ParseAddress(fullAddress, countryId);
    }

    private void ParseAddress(string fullAddress, string countryId)
    {
        if (AddressCache.TryGetValue(fullAddress, out var cachedAddress))
        {
            CopyAddress(cachedAddress);
            return;
        }

        string normalizedAddress = AddressHelper.NormalizeAddress(fullAddress.ToUpperInvariant().Replace(",", " ").Trim());
        normalizedAddress = Regex.Replace(normalizedAddress, @"(?<=[a-zA-Z])(?=\d)|(?<=\d)(?=[a-zA-Z])", " ");

        // Визначення країни
        Country = CountryHelper.GetCountry(normalizedAddress, countryId);
        normalizedAddress = AddressHelper.RemoveCountryFromAddress(normalizedAddress, Country);

        // Пошук поштового індексу
        int postalCodeIndex = AddressHelper.FindPostalCode(normalizedAddress, out var postalCode);
        PostalCode = postalCode;

        if (postalCodeIndex >= 0)
        {
            Street = AddressHelper.ExtractStreet(normalizedAddress, postalCodeIndex, PostalCode);
            City = GeoNamesData.GeoNamesList.FirstOrDefault(g => g.PostalCode == PostalCode)?.CityName;

            // Очищення назви вулиці від назви міста
            if (!string.IsNullOrEmpty(City))
            {
                Street = Street.Replace(City, "", StringComparison.OrdinalIgnoreCase).Trim();
            }
        }
        else
        {
            // Якщо індексу не знайдено, усе після країни вважається містом
            City = AddressHelper.FindCityInAddress(normalizedAddress);
        }

        //if (string.IsNullOrEmpty(Street))
        //{
        //    string newNormalizedAddress = AddressHelper.NormalizeAddress(fullAddress.ToUpperInvariant().Replace(",", " ").Trim());
        //    Street = AddressHelper.FindStreet(newNormalizedAddress, City, Country, PostalCode);
        //}

        AddressCache[fullAddress] = this;
    }
    private void CopyAddress(Address source)
    {
        OriginalAddress = source.OriginalAddress;
        Street = source.Street;
        PostalCode = source.PostalCode;
        City = source.City;
        Country = source.Country;
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

    public override string ToString()
    {
        var parts = new List<string> { Street, PostalCode, City, Country };
        return string.Join(", ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }
}
