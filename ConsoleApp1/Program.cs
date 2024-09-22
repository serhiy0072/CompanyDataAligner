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

        // Завантаження даних компаній та правових форм
        var companies = DataLoader.LoadCompanies(mainFilePath);
        var legalForms = DataLoader.LoadLegalForms(legalFormsFilePath);

        // Обробка компаній
        var processedCompanies = DataProcessor.ProcessMainData(companies, legalForms).OrderBy(c => c.UniqueName).ToList();

        // Встановлення ліцензії для використання EPPlus (потрібно з версії 5.x)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //ProcessExcelFiles(processedCompanies, resultFilePath);
        ProcessAnnotations(processedCompanies, annotationsFilePath);
    }

    // Метод для обробки Excel файлів (читання і запис)
    static void ProcessExcelFiles(List<Company> processedCompanies, string resultFilePath)
    {
        // Запис результатів у файл Excel
        ExcelWriter.WriteToExcelFile(resultFilePath, processedCompanies);
        Console.WriteLine($"Результати збережено у файл: {resultFilePath}");
    }

    // Метод для генерації та збереження анотацій
    static void ProcessAnnotations(List<Company> processedCompanies, string annotationsFilePath)
    {
        var groupedCompanies = DataProcessor.GroupUniqueCompanies(processedCompanies);
        var annotations = DataProcessor.GenerateAnnotations(groupedCompanies);

        // Запис анотацій у файл JSON
        JsonSerializer.SaveAnnotationsAsJson(annotations, annotationsFilePath);
        Console.WriteLine($"Анотації збережені у файл: {annotationsFilePath}");
    }
}
