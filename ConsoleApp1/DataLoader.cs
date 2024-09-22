using ConsoleApp1;
using OfficeOpenXml;

public static class DataLoader
{
    public static List<Company> LoadCompanies(string filePath)
    {
        var companies = new List<Company>();

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) // Починаємо з 2-го рядка, пропускаючи заголовки
            {
                string name = worksheet.Cells[row, 1].Text?.Trim() ?? string.Empty;
                string address = worksheet.Cells[row, 3].Text?.Trim() ?? string.Empty;
                string countryId = worksheet.Cells[row, 5].Text?.Trim() ?? "0";

                var company = new Company(name, address, countryId);
                companies.Add(company);
            }
        }
        return companies;
    }

    public static List<LegalForm> LoadLegalForms(string filePath)
    {
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
        return legalForms;
    }
}
