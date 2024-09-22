using ConsoleApp1;
using OfficeOpenXml;

public static class DataProcessor
{
    public static void FindLegalForm(Company company, List<LegalForm> legalForms)
    {
        LegalForm? bestMatch = null;
        int longestMatchLength = 0;

        // Сортуємо правові форми за довжиною ShortName у порядку спадання, щоб довші форми перевірялися першими
        var sortedLegalForms = legalForms.OrderByDescending(form => form.ShortName.Length).ToList();

        foreach (var form in sortedLegalForms)
        {
            string shortForm = form.ShortName.ToUpper().Replace(" ", "");
            if (!string.IsNullOrEmpty(shortForm) && company.FullName.ToUpper().Contains(shortForm))
            {
                // Якщо знайдений збіг довший за поточний найдовший збіг, оновлюємо bestMatch
                if (shortForm.Length > longestMatchLength)
                {
                    longestMatchLength = shortForm.Length;
                    bestMatch = form;
                }
            }
        }

        // Прив'язуємо знайдену правову форму до компанії
        company.LegalForm = bestMatch;

        // Оновлюємо унікальну назву компанії без правової форми
        company.UniqueName = ExtractUniqueName(company.FullName, bestMatch).Trim(',', '&', '.').Trim();
    }

    // Метод для витягнення унікальної назви компанії без правової форми (ОПФ)
    private static string ExtractUniqueName(string fullName, LegalForm? form)
    {
        if (form != null)
        {
            return fullName.Replace(form.ShortName, "").Trim();
        }
        return fullName;
    }

    public static List<Company> ProcessMainData(List<Company> companies, List<LegalForm> legalForms)
    {
        // Паралельна обробка компаній
        Parallel.ForEach(companies, company =>
        {
            // Використовуємо новий метод FindLegalForm з DataProcessor
            DataProcessor.FindLegalForm(company, legalForms);
        });

        return companies.ToList(); // Повертаємо результат
    }

    public static List<Company> GroupUniqueCompanies(List<Company> companies)
    {
        return companies
                .GroupBy(c => new { c.UniqueName, FirmAddress = c.Address?.ToString() ?? string.Empty })
                .Select(g => g.First())
                .OrderBy(c => c.UniqueName)  // Додаємо сортування по UniqueName за зростанням
                .ToList();
    }
    // Новий метод для генерації анотацій
    public static List<AnnotatedSentence> GenerateAnnotations(List<Company> companies)
    {
        var annotatedSentences = new List<AnnotatedSentence>();

        foreach (var company in companies)
        {
            var tokens = new List<Token>();

            // Розбиваємо назву компанії на слова і додаємо теги для організації
            var nameTokens = company.FullName.Split(' ');
            bool isFirstOrgNameToken = true;

            foreach (var token in nameTokens)
            {
                if (isFirstOrgNameToken)
                {
                    tokens.Add(new Token(token, "B-ORG_NAME"));  // Перший токен - B-ORG_NAME
                    isFirstOrgNameToken = false;
                }
                else
                {
                    tokens.Add(new Token(token, "I-ORG_NAME"));  // Наступні - I-ORG_NAME
                }
            }

            // Додаємо теги для правової форми, якщо вона знайдена
            if (company.LegalForm != null)
            {
                tokens.Add(new Token(company.LegalForm.ShortName, "B-LEGAL_FORM"));
            }

            // Розбиваємо адресу на слова і додаємо теги для адреси
            if (company.Address != null && !string.IsNullOrEmpty(company.Address.OriginalAddress))
            {
                var addressTokens = company.Address.OriginalAddress.Split(' ');
                bool isFirstAddressToken = true;

                foreach (var token in addressTokens)
                {
                    if (isFirstAddressToken)
                    {
                        tokens.Add(new Token(token, "B-ADDRESS"));  // Перший токен адреси - B-ADDRESS
                        isFirstAddressToken = false;
                    }
                    else
                    {
                        tokens.Add(new Token(token, "I-ADDRESS"));  // Наступні - I-ADDRESS
                    }
                }
            }

            // Додаємо анотоване речення до списку
            annotatedSentences.Add(new AnnotatedSentence(tokens));
        }

        return annotatedSentences;
    }
}
