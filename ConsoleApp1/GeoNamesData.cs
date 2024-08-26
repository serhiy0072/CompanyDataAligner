using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GeoNames
{
    public string PostalCode { get; set; }
    public string CityName { get; set; }
    public string RegionName { get; set; }

    public GeoNames() { }

    public GeoNames(string postalCode, string cityName, string regionName)
    {
        PostalCode = postalCode;
        CityName = cityName;
        RegionName = regionName;
    }
}

public static class GeoNamesData
{
    public static List<GeoNames> GeoNames;

    static GeoNamesData()
    {
        // Ініціалізація даних при запуску
        GeoNames = LoadData(Path.Combine(Directory.GetCurrentDirectory(), "Files", "GeoNamesDE.txt"));
    }

    private static List<GeoNames> LoadData(string filePath)
    {
        var data = new List<GeoNames>();

        foreach (var line in File.ReadLines(filePath))
        {
            var columns = line.Split('\t');
            if (columns.Length >= 3) // 3 стовпці: поштовий код, місто, регіон
            {
                data.Add(new GeoNames
                {
                    PostalCode = columns[0].Trim().ToUpper(),
                    CityName = columns[1].Trim().ToUpper(),
                    RegionName = columns[2].Trim().ToUpper(),
                });
            }
        }
        return data;
    }
}
