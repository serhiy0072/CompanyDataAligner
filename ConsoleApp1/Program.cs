using OfficeOpenXml;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // Визначаємо шлях до файлів
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            string mainFilePath = Path.Combine(directoryPath, "DataFiles\\data.xlsx");  // Файл з основною таблицею
            string legalFormsFilePath = Path.Combine(directoryPath, "DataFiles\\legalForms.xlsx"); // Файл з правовими формами
            string resultFilePath = Path.Combine(directoryPath, "result.xlsx");
            string annotationsFilePath = Path.Combine(directoryPath, "annotations.json");
            string textFilePath = Path.Combine(directoryPath, "result");
            string textRawFilePath = Path.Combine(directoryPath, "resultRaw");

            // Встановлення ліцензії для використання EPPlus (потрібно з версії 5.x)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var companies = DataLoader.LoadCompanies(mainFilePath);
            var legalForms = DataLoader.LoadLegalForms(legalFormsFilePath);

            // Обробка компаній
            var processedCompanieWithLegalForms = DataProcessor.ProcessCompanyDataWithLegalForms(companies, legalForms);
            var processedCompanieWithAddressFormat = DataProcessor.ProcessCompanyDataWithAddressFormat(processedCompanieWithLegalForms).OrderBy(c => c.UniqueName).ToList();

            // Запис результатів у файл Excel
            ProcessExcelFiles(processedCompanieWithAddressFormat, resultFilePath);


            // Запис результатів у файл txt
            //ProcessTextAnnotations(processedCompanies, textFilePath);

            // Генерація анотацій
            //ProcessJsonAnnotations(processedCompanies, annotationsFilePath);
        }

        // Метод для обробки Excel файлів (читання і запис)
        static void ProcessExcelFiles(List<Company> processedCompanies, string resultFilePath)
        {
            try
            {
                string finalFilePath = ExcelWriter.GetUniqueFilePath(resultFilePath);
                // Тимчасова умова для кращої релевантності даних Фільтруємо компанії, де LegalForm.ShortName не є порожнім рядком або null
                //var filteredCompanies = processedCompanies
                //    .Where(c => !string.IsNullOrEmpty(c.LegalForm?.ShortName))
                //    .ToList();
                var groupedCompanies = DataProcessor.GroupUniqueCompanies(processedCompanies);

                ExcelWriter.WriteToExcelFile(finalFilePath, groupedCompanies);
                //ExcelWriter.WriteToExcelFile(resultFilePath, processedCompanies);
                Console.WriteLine($"Результати збережено у файл: {finalFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка обробки Excel-файлів: {ex.Message}");
            }
        }

        // Метод для генерації та збереження анотацій
        static void ProcessJsonAnnotations(List<Company> processedCompanies, string annotationsFilePath)
        {
            try
            {
                // Тимчасова умова для кращої релевантності даних Фільтруємо компанії, де LegalForm.ShortName не є порожнім рядком або null
                var filteredCompanies = processedCompanies
                    .Where(c => !string.IsNullOrEmpty(c.LegalForm?.ShortName))
                    .ToList();
                var groupedCompanies = DataProcessor.GroupUniqueCompanies(filteredCompanies);

                var annotations = DataProcessor.GenerateAnnotations(groupedCompanies);
                // 1. Збереження всіх анотацій у один загальний JSON файл
                SaveFullJsonFile(annotations, annotationsFilePath);

                // 2. Збереження у декілька JSON файлів, якщо розмір перевищує 10 МБ
                //JsonSplitter.SaveJsonWithLimit(annotations, annotationsFilePath);
                //Console.WriteLine($"Анотації збережені у файл: {annotationsFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка генерації анотацій: {ex.Message}");
            }
        }

        // Метод для генерації та збереження анотацій в текстовому форматі
        static void ProcessTextAnnotations(List<Company> processedCompanies, string annotationsFilePath)
        {
            try
            {
                // Тимчасова умова для кращої релевантності даних Фільтруємо компанії, де LegalForm.ShortName не є порожнім рядком або null
                //var filteredCompanies = processedCompanies
                //    .Where(c => !string.IsNullOrEmpty(c.LegalForm?.ShortName))
                //    .ToList();
                var groupedCompanies = DataProcessor.GroupUniqueCompanies(processedCompanies);

                var annotations = DataProcessor.GenerateAnnotations(groupedCompanies);
                TextDataWriter.SaveAnnotationsAsText(annotations, annotationsFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка генерації анотацій: {ex.Message}");
            }
        }
        // Метод для збереження загального JSON-файлу
        static void SaveFullJsonFile<T>(T data, string filePath)
        {
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
            Console.WriteLine($"Повний JSON файл збережений у файл: {filePath}");
        }
    }
}