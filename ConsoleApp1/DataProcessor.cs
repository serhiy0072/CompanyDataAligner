using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public static class DataProcessor
    {
        public static void FindLegalFormAndUniqueName(Company company, List<LegalForm> legalForms)
        {
            company.UniqueName = company?.FullName != null ? TextNormalizer.NormalizeName(company.FullName).Trim('.').Trim() : null;
            if (company?.UniqueName == null) return;

            var normalizedFullName = company.UniqueName;

            var foundLegalForms = new List<LegalForm>();

            // Пошук всіх можливих правових форм у назві компанії
            foreach (var form in legalForms)
            {
                string shortForm = form.ShortName.ToUpper();

                if (!string.IsNullOrEmpty(shortForm) && (IsWholeWordMatch(normalizedFullName, shortForm) || IsWholeWordMatchExt(normalizedFullName, shortForm)))
                {
                    foundLegalForms.Add(form);
                }
            }

            // Якщо знайдено кілька форм
            if (foundLegalForms.Count > 1)
            {
                // Сортуємо знайдені форми від найдовшої до найкоротшої
                foundLegalForms = foundLegalForms.OrderByDescending(f => f.ShortName.Length).ToList();

                // Видаляємо менші форми, які є частиною більшої
                var filteredLegalForms = new List<LegalForm>(foundLegalForms);

                for (int i = 0; i < foundLegalForms.Count; i++)
                {
                    for (int j = i + 1; j < foundLegalForms.Count; j++)
                    {
                        if (foundLegalForms[i].ShortName.Contains(foundLegalForms[j].ShortName))
                        {
                            filteredLegalForms.Remove(foundLegalForms[j]);
                        }
                    }
                }

                foundLegalForms = filteredLegalForms;

                // Якщо після видалення залишилось більше однієї форми, встановлюємо відмітку
                if (foundLegalForms.Count > 1)
                {
                    company.HasMultipleLegalForms = true;
                }
                else
                {
                    company.HasMultipleLegalForms = false;
                }
            }
            else
            {
                company.HasMultipleLegalForms = false;
            }

            // Якщо знайдена хоча б одна форма, призначаємо її
            if (foundLegalForms.Count > 0)
            {
                var bestMatch = foundLegalForms.FirstOrDefault();
                company.LegalForm = bestMatch;

                // Видаляємо знайдені правові форми з назви компанії
                normalizedFullName = ExtractUniqueName(normalizedFullName, bestMatch);
                company.UniqueName = normalizedFullName.Trim('.').Trim();
            }
        }

        private static bool IsWholeWordMatch(string fullName, string searchTerm)
        {
            string pattern = $@"(?i)\b{Regex.Escape(searchTerm).Replace(@"\ ", @"\s*[&\.\s]*")}\b";

            return Regex.IsMatch(fullName, pattern);
        }
        private static bool IsWholeWordMatchExt(string fullName, string searchTerm)
        {
            // Робимо шаблон нечутливим до регістру і враховуємо можливі пробіли та крапки
            string pattern = $@"(?i)\b{string.Join(@"\s*\.?\s*", searchTerm.Split('.'))}\b";

            return Regex.IsMatch(fullName, pattern);
        }


        private static string ExtractUniqueName(string fullName, LegalForm? form)
        {
            string pattern = string.Empty;
            if (form != null && form.ShortName != null)
            {
                pattern = $@"(?i)\b{string.Join(@"\s*&?\s*\.?\s*", form.ShortName.Split(' '))}\b";

            }
            return Regex.Replace(fullName, pattern, "", RegexOptions.IgnoreCase).Trim();
        }

        public static List<Company> ProcessCompanyDataWithLegalForms(List<Company> companies, List<LegalForm> legalForms)
        {
            Parallel.ForEach(companies, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, company =>
            {
                FindLegalFormAndUniqueName(company, legalForms);
            });

            return companies.ToList();
        }
        public static List<Company> ProcessCompanyDataWithAddressFormat(List<Company> companies)
        {
            Parallel.ForEach(companies, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, company =>
            {
                if (company?.Address?.OriginalAddress != null && company.CountryId != null)
                {
                    company.Address.ParseAddress(company.Address.OriginalAddress, company.CountryId);
                }
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
}