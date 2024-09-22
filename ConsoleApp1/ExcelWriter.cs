using ConsoleApp1;
using OfficeOpenXml;

public static class ExcelWriter
{
    public static void WriteToExcelFile(string filePath, List<Company> companies)
    {
        try
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

                // Зберігаємо файл
                package.SaveAs(new FileInfo(filePath));
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Помилка доступу до файлу: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сталася помилка при збереженні файлу: {ex.Message}");
        }
    }
}
