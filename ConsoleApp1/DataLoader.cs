using ConsoleApp1;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Concurrent;
namespace ConsoleApp1
{
    public static class DataLoader
    {
        private static List<Company> _cachedCompanies;
        private static List<LegalForm> _cachedLegalForms;
        public static List<Company> LoadCompanies(string filePath)
        {
            if (_cachedCompanies != null)
            {
                // Повертаємо кешовані дані, якщо вони вже були завантажені
                return _cachedCompanies;
            }

            string cacheFilePath = "cached_companies.json";

            // Перевіряємо, чи існує файл кешу
            if (File.Exists(cacheFilePath))
            {
                string json = File.ReadAllText(cacheFilePath);

                // Спочатку десеріалізуємо як список простих об'єктів для створення з конструкторами
                var simpleCompanies = JsonConvert.DeserializeObject<List<Company>>(json);

                if (simpleCompanies != null)
                {
                    _cachedCompanies = simpleCompanies
                        .Select(c => new Company(c.FullName, c.Address?.OriginalAddress, c.CountryId))
                        .ToList();

                    return _cachedCompanies;
                }
            }

            // Якщо файл кешу не існує або не вдалося завантажити дані, завантажуємо з Excel
            var companies = new List<Company>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Починаємо з 2-го рядка, пропускаючи заголовки
                {
                    string name = worksheet.Cells[row, 1].Text?.ToUpper().Trim() ?? string.Empty;
                    string address = worksheet.Cells[row, 3].Text?.ToUpper().Trim() ?? string.Empty;
                    string countryId = worksheet.Cells[row, 5].Text?.Trim() ?? "0";

                    var company = new Company(name, address, countryId);
                    companies.Add(company);
                }
            }

            // Зберігаємо завантажені дані в файл для наступних запусків
            _cachedCompanies = companies;
            string serializedCompanies = JsonConvert.SerializeObject(companies, Formatting.Indented);
            File.WriteAllText(cacheFilePath, serializedCompanies);

            return companies;
        }


        public static List<LegalForm> LoadLegalForms(string filePath)
        {
            if (_cachedLegalForms != null)
            {
                // Повертаємо кешовані дані, якщо вони вже були завантажені
                return _cachedLegalForms;
            }

            string cacheFilePath = "cached_legal_forms.json";

            // Перевіряємо, чи існує файл кешу
            if (File.Exists(cacheFilePath))
            {
                // Якщо файл існує, завантажуємо дані з нього
                string json = File.ReadAllText(cacheFilePath);
                _cachedLegalForms = JsonConvert.DeserializeObject<List<LegalForm>>(json);
                if (_cachedLegalForms != null)
                {
                    return _cachedLegalForms;
                }
            }

            // Якщо файл кешу не існує або не вдалося завантажити дані, завантажуємо з Excel
            var legalForms = new List<LegalForm>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Починаємо з 2-го рядка, пропускаючи заголовки
                {
                    string shortForm = worksheet.Cells[row, 1].Text?.Trim() ?? string.Empty;
                    string nameUA = worksheet.Cells[row, 2].Text?.Trim() ?? string.Empty;
                    string nameEN = worksheet.Cells[row, 3].Text?.Trim() ?? string.Empty;

                    var legalForm = new LegalForm
                    {
                        ShortName = shortForm,
                        NameUA = nameUA,
                        NameEN = nameEN
                    };
                    legalForms.Add(legalForm);
                }
            }

            // Сортуємо і зберігаємо завантажені дані в файл для наступних запусків
            _cachedLegalForms = legalForms
                .OrderByDescending(form => form.ShortName?.Length)
                .ToList();

            string serializedLegalForms = JsonConvert.SerializeObject(_cachedLegalForms, Formatting.Indented);
            File.WriteAllText(cacheFilePath, serializedLegalForms);

            return _cachedLegalForms;
        }
    }
}