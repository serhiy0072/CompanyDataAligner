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
        string deletedFilePath = Path.Combine(directoryPath, "resultDeleted.xlsx");

        // Встановлення ліцензії для використання EPPlus (потрібно з версії 5.x)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        // Зчитуємо дані з обох файлів і заповнюємо список об'єктів Company
        var companies = ProcessMainData(mainFilePath, legalFormsFilePath);

        // Зчитуємо дані з обох файлів і заповнюємо список об'єктів Company
        var groupedCompanies = GroupUniqueCompanies(companies);

        // Записуємо оброблені дані у новий файл Excel
        WriteToExcelFile(resultFilePath, groupedCompanies);
        //WriteToExcelFile(deletedFilePath, delitedCompanies);

        Console.WriteLine($"Результати збережено у файл: {resultFilePath}");
    }

    static List<Company> ProcessMainData(string mainFilePath, string legalFormsFilePath)
    {
        var mainData = LoadExcelFile(mainFilePath);
        var legalFormsData = LoadExcelFile(legalFormsFilePath);
        var companies = new List<Company>();

        foreach (var row in mainData)
        {
            string name = row.Count > 0 ? row[0] : string.Empty;
            string address = row.Count > 2 ? row[2] : string.Empty;
            string countryId = row.Count > 4 ? row[4] : "0";

            // Створюємо об'єкт компанії
            var company = new Company(name, address, countryId); 
            
            company.FindLegalForm(legalFormsData);
            company.Address = new Address(address, countryId); 

            companies.Add(company);
        }

        return companies;
    }

    static List<Company> GroupUniqueCompanies(List<Company> companies)
    {
        return companies
            .GroupBy(c => new { c.UniqueName, FirmAddress = c.Address?.ToString() ?? string.Empty })//, LegalFormShortName = c.LegalForm?.ShortName ?? string.Empty
            .Select(g => g.First())
            .ToList();
    }

    static void WriteToExcelFile(string filePath, List<Company> companies)
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Processed Data");

            worksheet.Column(1).Width = 30;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 30;
            worksheet.Column(4).Width = 30;
            worksheet.Column(5).Width = 15;
            worksheet.Column(6).Width = 15;
            worksheet.Column(7).Width = 25;
            worksheet.Column(8).Width = 25;

            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Unique Name";
            worksheet.Cells[1, 3].Value = "Original Address";
            worksheet.Cells[1, 4].Value = "Unique Address";
            worksheet.Cells[1, 5].Value = "Country Id";
            worksheet.Cells[1, 6].Value = "Short Form";
            worksheet.Cells[1, 7].Value = "Ukrainian OPF";
            worksheet.Cells[1, 8].Value = "English OPF";

            for (int i = 0; i < companies.Count; i++)
            {
                var company = companies[i];
                worksheet.Cells[i + 2, 1].Value = company.FullName;
                worksheet.Cells[i + 2, 2].Value = company.UniqueName;
                worksheet.Cells[i + 2, 3].Value = company.Address?.OriginalAddress;
                worksheet.Cells[i + 2, 4].Value = company.Address?.ToString();
                worksheet.Cells[i + 2, 5].Value = company.CountryId;
                worksheet.Cells[i + 2, 6].Value = company.LegalForm?.ShortName;
                worksheet.Cells[i + 2, 7].Value = company.LegalForm?.NameUA;
                worksheet.Cells[i + 2, 8].Value = company.LegalForm?.NameEN;
            }

            //SetColumnColorToYellow(worksheet, 2);
            //SetColumnColorToYellow(worksheet, 4);
            //SetColumnColorToYellow(worksheet, 6);
            //SetColumnColorToYellow(worksheet, 7);
            //SetColumnColorToYellow(worksheet, 8);

            // Зберігаємо пакет в файл
            package.SaveAs(new FileInfo(filePath));
        }
    }

    static List<List<string>> LoadExcelFile(string filePath)
    {
        var data = new List<List<string>>();

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;

            for (int row = 2; row <= rowCount; row++)
            {
                var rowData = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    string cellValue = worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
                    rowData.Add(cellValue);
                }
                data.Add(rowData);
            }
        }
        return data;
    }
    public static void SetColumnColorToYellow(ExcelWorksheet worksheet, int columnIndex)
    {
        // Встановлюємо жовтий колір для всієї колонки
        using (var range = worksheet.Cells[1, columnIndex, worksheet.Dimension.End.Row, columnIndex])
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
        }
    }
}
