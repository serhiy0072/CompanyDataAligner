using System.Text.RegularExpressions;

public static class AddressHelper
{
    public static string NormalizeAddress(string address)
    {
        ReplaceCityNames( ref address);
        return CorrectCommonMistakes(address.ToUpperInvariant().Replace(",", " ").Trim());
    }

    public static string CorrectCommonMistakes(string input)
    {
        var corrections = new Dictionary<string, string>
        {

            { "LTD.", "LTD" },
            { "PLCK", "PLANK" },
            { "STR.", " STRASSE " },
            { "STRA?E", " STRASSE " },
            { "STRASSE", " STRASSE " },
            { "ШТРАСЕ", " STRASSE " },
            { "АМ АМАЗОНЕНВЕРК", " AMAZONENWERK " },
            { "WIESB", "WIESBADEN " },
            { "WIESBADENADEN", "WIESBADEN " },
            { "WIESBADEN ADEN", "WIESBADEN " },
            { "DELKENHEIM", " " },
            { "DELKENHEIMGERMANY", "DELKENHEIM GERMANY " },
            { "BUNDESLAND", " " },
            { "STATE", " " },
            { "ГАСТЕ", " " },

            { "ВУЛ.", " " },
            { "М.", " " },
            { "/", " " },
            { "\\", " " },
            { "\"", " " },
            { "\'", " " },
            { "¬", " " },
            { "„", " " },
            { ";", " " },
            { ":", " " },
            { "+", " " },
            { "CO KG", " " },
            { "(", " " },
            { ")", " " },
            { " BAVARIA ", " " },
            { "«", " " },
            { "»", " " },
            { "&", " & " },
            { "D-", " " },
            { "Д-", " " },
            { "DE-", " " },
            { "ДЕ-", " " },
            { "<", " " },
            { ">", " " },
            { "-", " " },
            { ".", " " },
            { "  ", " " },
        };

        foreach (var correction in corrections)
        {
            input = input.Replace(correction.Key, correction.Value);
            input = Regex.Replace(input, @"\s+", " ");
        }

        input = Regex.Replace(input, @"(?<=[a-zA-Z])(?=\d)|(?<=\d)(?=[a-zA-Z])", " ");
        return input;
    }
    
    public static string ReplaceCityNames(ref string address)
    {
        var CityNames = new Dictionary<string, string>
    {
        { "ААХЕН", "AACHEN" },
        { "АУГСБУРГ", "AUGSBURG" },
        { "БЕРЛІН", "BERLIN" },
        { "БОНН", "BONN" },
        { "БРАУНШВЕЙГ", "BRAUNSCHWEIG" },
        { "БРЕМЕН", "BREMEN" },
        { "БРЕМЕРХАВЕН", "BREMERHAVEN" },
        { "ВІТЕН", "WITTEN" },
        { "ГАННОВЕР", "HANNOVER" },
        { "ГАМБУРГ", "HAMBURG" },
        { "ГАСБЕРГЕН", "HASBERGEN" },
        { "ДУЙСБУРГ", "DUISBURG" },
        { "ДЮССЕЛЬДОРФ", "DUSSELDORF" },
        { "ДОРТМУНД", "DORTMUND" },
        { "ДРЕЗДЕН", "DRESDEN" },
        { "ЕННЕПЕТАЛЬ", "ENNEPETAL" },
        { "ЕРФУРТ", "ERFURT" },
        { "ЗАПОРІЖЖЯ", "ZAPOROZHYE" },
        { "КАССЕЛЬ", "KASSEL" },
        { "КЮЛЬН", "KÖLN" },
        { "КІЛЬ", "KIEL" },
        { "КЮНЦЕЛЬЗАУ", "KUNZELSAU" },
        { "ЛЕЙПЦИГ", "LEIPZIG" },
        { "ЛЮБЕК", "LUEBECK" },
        { "МАЙНЦ", "MAINZ" },
        { "МАНГЕЙМ", "MANNHEIM" },
        { "МООРВЕРДЕР", "MOORWERDER" },
        { "МЮЛЕНВІНКЕЛЬ", "MUEHLENWINKEL" },
        { "МЮНХЕН", "MUNICH" },
        { "НОРДЕРДАЙХ", "NORDERDEICH" },
        { "НЮРНБЕРГ", "NUREMBERG" },
        { "ОСНАБРЮК", "OSNABRUECK" },
        { "ПАДЕРБОРН", "PADERBORN" },
        { "ПОРЦГАЙМ", "PFORZHEIM" },
        { "РАЙНХОЛЬ", "REINHOL" },
        { "РОСТОК", "ROSTOCK" },
        { "ФРАНКФУРТ", "FRANKFURT" },
        { "ФРАНКФУРТ-НА-ОДЕРІ", "FRANKFURT-ODER" },
        { "ФРАНКФУРТ НА МАЙНІ", "FRANKFURT" },
        { "ФРАЙБУРГ", "FREIBURG" },
        { "ФУЛЬДА", "FULDA" },
        { "ХАРЗЕВІНКЕЛЬ", "HARSEWINKEL" },
        { "ХАСБЕРГЕН", "HASBERGEN" },
        { "ХЕССЕН", "HESSEN" },
        { "ШАРЛОТТЕНБУРГ", "CHARLOTTENBURG" },
        { "ШТУТГАРТ", "STUTTGART" },
        { "ШВЕРІН", "SCHWERIN" },
        { "ЦВІКАУ", "ZWICKAU" },
    };
        // Проходимо по всіх ключах словника CityNames
        foreach (var city in CityNames)
        {
            // Якщо адреса містить ключ (назву міста), замінюємо його на відповідне значення
            if (address.Contains(city.Key, StringComparison.OrdinalIgnoreCase))
            {
                address = address.Replace(city.Key, city.Value, StringComparison.OrdinalIgnoreCase);
            }
        }

        // Повертаємо змінену адресу
        return address;
    }

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