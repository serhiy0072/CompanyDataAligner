
namespace ConsoleApp1
{
    public class GeoNames
    {
        public string PostalCode { get; set; }
        public string CityName { get; set; }
        public string RegionNameDE { get; set; }

        public GeoNames() { }

        public GeoNames(string postalCode, string cityName, string regionName)
        {
            PostalCode = postalCode;
            CityName = cityName;
            RegionNameDE = regionName;
        }
    }

    public static class GeoNamesData
    {
        public static List<GeoNames> GeoNamesList;

        static GeoNamesData()
        {
            try
            {
                // Ініціалізація даних при запуску
                GeoNamesList = LoadData(Path.Combine(Directory.GetCurrentDirectory(), "Files", "DataFiles\\GeoNamesDE.txt"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження даних: {ex.Message}");
            }
        }

        private static List<GeoNames> LoadData(string filePath)
        {
            var data = new List<GeoNames>();

            try
            {
                foreach (var line in File.ReadLines(filePath))
                {
                    var columns = line.Split('\t');
                    if (columns.Length >= 3) // 3 стовпці: поштовий код, місто, регіон
                    {
                        try
                        {
                            data.Add(new GeoNames
                            {
                                PostalCode = columns[1].Trim().ToUpper(),
                                CityName = columns[2].Trim().ToUpper(),
                                RegionNameDE = columns[3].Trim().ToUpper(),
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Помилка під час обробки рядка: {ex.Message}");
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Файл не знайдено: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час читання файлу: {ex.Message}");
            }

            return data;
        }
    }
}