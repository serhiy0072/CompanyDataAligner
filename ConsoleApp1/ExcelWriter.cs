using OfficeOpenXml;

namespace ConsoleApp1
{
    public static class ExcelWriter
    {
        public static void WriteToExcelFile(string filePath, List<Company> companies)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Processed Data");

                    worksheet.Column(1).Width = 60;
                    worksheet.Column(2).Width = 60;
                    worksheet.Column(3).Width = 10;
                    worksheet.Column(4).Width = 10;
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
                    worksheet.Cells[1, 9].Value = "Notes ";

                    for (int i = 0, row = 2; i < companies.Count; i++)
                    {
                        var company = companies[i];

                        worksheet.Cells[row, 1].Value = company.FullName;
                        worksheet.Cells[row, 2].Value = company.UniqueName;
                        worksheet.Cells[row, 3].Value = company.Address?.OriginalAddress;
                        worksheet.Cells[row, 4].Value = company.Address?.ToString();
                        worksheet.Cells[row, 5].Value = company.CountryId;
                        worksheet.Cells[row, 6].Value = company.LegalForm?.ShortName;
                        worksheet.Cells[row, 7].Value = company.LegalForm?.NameUA;
                        worksheet.Cells[row, 8].Value = company.LegalForm?.NameEN;
                        worksheet.Cells[row, 9].Value = company.HasMultipleLegalForms? "HasMultipleLegalForm" : "";

                        row++;
                    }

                    // Зберігаємо файл
                    package.SaveAs(new FileInfo(filePath));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Помилка доступу до файлу: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Помилка вводу-виводу: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сталася помилка при збереженні файлу: {ex.Message}");
            }
        }
        public static string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // Якщо файл не існує, повертаємо початкове ім'я
                return filePath;
            }

            // Отримуємо директорію та ім'я файлу
            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            int fileNumber = 1;

            // Генеруємо нові імена, поки не знайдемо те, що не існує
            string newFilePath;
            do
            {
                newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension} ({fileNumber}){extension}");
                fileNumber++;
            } while (File.Exists(newFilePath));

            return newFilePath;
        }
    }
}