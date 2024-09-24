using ConsoleApp1;
using OfficeOpenXml;
using System.Text.RegularExpressions;

public static class DataProcessor
{
    public static void FindLegalForm(Company company, List<LegalForm> legalForms)
    {
        LegalForm? bestMatch = null;
        int longestMatchLength = 0;
        var normalizedFullName = company.UniqueName;

        var sortedLegalForms = legalForms.OrderByDescending(form => form.ShortName?.Length).ToList();

        foreach (var form in sortedLegalForms)
        {
            string shortForm = form.ShortName.ToUpper().Replace(" ", "");
            if (!string.IsNullOrEmpty(shortForm) && IsWholeWordMatch(normalizedFullName, shortForm))
            {
                if (shortForm.Length > longestMatchLength)
                {
                    longestMatchLength = shortForm.Length;
                    bestMatch = form;
                }
            }
        }

        company.LegalForm = bestMatch;
        company.UniqueName = ExtractUniqueName(normalizedFullName, bestMatch).Trim();
    }

    private static bool IsWholeWordMatch(string fullName, string searchTerm)
    {
        return Regex.IsMatch(fullName, $@"\b{Regex.Escape(searchTerm)}\b");
    }

    private static string ExtractUniqueName(string fullName, LegalForm? form)
    {
        if (form != null)
        {
            return Regex.Replace(fullName, $@"\b{Regex.Escape(form.ShortName)}\b", "").Trim();
        }
        return fullName;
    }

    public static List<Company> ProcessMainData(List<Company> companies, List<LegalForm> legalForms)
    {
        Parallel.ForEach(companies, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, company =>
        {
            DataProcessor.FindLegalForm(company, legalForms);
        });

        return companies.ToList();
    }

    public static List<Company> GroupUniqueCompanies(List<Company> companies)
    {
        return companies
            .GroupBy(c => new { c.UniqueName, FirmAddress = c.Address?.ToString() ?? string.Empty })
            .Select(g => g.First())
            .OrderBy(c => c.UniqueName)
            .ToList();
    }

    public static List<AnnotatedSentence> GenerateAnnotations(List<Company> companies)
    {
        return Tagger.AnnotateCompaniesWithTags(companies, new List<LegalForm>());
    }
}
