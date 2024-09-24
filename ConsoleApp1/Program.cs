using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ConsoleApp1;
using System.Text;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Concurrent;
using Newtonsoft.Json;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Визначаємо шлях до файлів
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        string mainFilePath = Path.Combine(directoryPath, "data.xlsx");  // Файл з основною таблицею
        string legalFormsFilePath = Path.Combine(directoryPath, "legalForms.xlsx"); // Файл з правовими формами
        string resultFilePath = Path.Combine(directoryPath, "result.xlsx");
        string annotationsFilePath = Path.Combine(directoryPath, "annotations.json");

        // Встановлення ліцензії для використання EPPlus (потрібно з версії 5.x)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        List<Company> companies;
        List<LegalForm> legalForms;

        // Завантаження даних компаній та правових форм
        try
        {
            companies = DataLoader.LoadCompanies(mainFilePath);
            legalForms = DataLoader.LoadLegalForms(legalFormsFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка завантаження даних: {ex.Message}");
            return;
        }
        // Обробка компаній
        List<Company> processedCompanies;
        try
        {
            processedCompanies = DataProcessor.ProcessMainData(companies, legalForms).OrderBy(c => c.UniqueName).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка обробки даних компаній: {ex.Message}");
            return;
        }

        // Встановлення ліцензії для використання EPPlus (потрібно з версії 5.x)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        // Запис результатів у файл Excel
        ProcessExcelFiles(processedCompanies, resultFilePath);

        // Генерація анотацій
        ProcessAnnotations(processedCompanies, annotationsFilePath);
    }

    // Метод для обробки Excel файлів (читання і запис)
    static void ProcessExcelFiles(List<Company> processedCompanies, string resultFilePath)
    {
        try
        {
            // Тимчасова умова для кращої релевантності даних Фільтруємо компанії, де LegalForm.ShortName не є порожнім рядком або null
            var filteredCompanies = processedCompanies
                .Where(c => !string.IsNullOrEmpty(c.LegalForm?.ShortName))
                .ToList();
            var groupedCompanies = DataProcessor.GroupUniqueCompanies(filteredCompanies);

            ExcelWriter.WriteToExcelFile(resultFilePath, groupedCompanies);
            //ExcelWriter.WriteToExcelFile(resultFilePath, processedCompanies);
            Console.WriteLine($"Результати збережено у файл: {resultFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка обробки Excel-файлів: {ex.Message}");
        }
    }

    // Метод для генерації та збереження анотацій
    static void ProcessAnnotations(List<Company> processedCompanies, string annotationsFilePath)
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
    // Метод для збереження загального JSON-файлу
    static void SaveFullJsonFile<T>(T data, string filePath)
    {
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, jsonData);
        Console.WriteLine($"Повний JSON файл збережений у файл: {filePath}");
    }
}
