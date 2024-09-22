using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Company
    {
        #region Properties
        public int Id { get; set; }
        public string? CountryId { get; set; }
        public string? FullName { get; set; }
        public string? UniqueName { get; set; }
        public Address Address { get; set; }
        public LegalForm? LegalForm { get; set; }
        #endregion
        #region Constructors 
        public Company(string name, string fullAddress, string countryId)
        {
            FullName = CorrectCommonMistakes(NormalizeName(name));
            Address = new Address(fullAddress, countryId);
            CountryId = countryId;
        }
        #endregion
        #region Methods
        public void FindLegalForm(List<List<string>> legalFormsData)
        {
            LegalForm? bestMatch = null;
            int longestMatchLength = 0;

            // Сортуємо форми за довжиною у порядку спадання, щоб довші форми перевірялися першими
            var sortedLegalFormsData = legalFormsData.OrderByDescending(row => row[0].Length).ToList();

            foreach (var row in sortedLegalFormsData)
            {
                string shortForm = row.Count > 0 ? row[0].ToUpper().Replace(" ", "") : string.Empty;//
                string ukrainianName = row.Count > 1 ? row[1].ToUpper() : string.Empty;
                string englishName = row.Count > 2 ? row[2].ToUpper() : string.Empty;

                // Перевіряємо, чи містить назва компанії цю форму
                if (!string.IsNullOrEmpty(shortForm) && FullName.Contains(shortForm))
                {
                    // Якщо знайдений збіг довший за поточний найдовший збіг, оновлюємо bestMatch
                    if (shortForm.Length > longestMatchLength)
                    {
                        longestMatchLength = shortForm.Length;
                        bestMatch = new LegalForm
                        {
                            ShortName = shortForm,
                            NameUA = ukrainianName,
                            NameEN = englishName
                        };
                    }
                }
            }
            LegalForm = bestMatch;
            UniqueName = ExtractUniqueName(FullName, bestMatch).Trim(',','&','.').Trim();
        }

        private string ExtractUniqueName(string name, LegalForm form)
        {
            if (form?.ShortName == null)
            {
                return name.Trim(); // Якщо форма або ShortName null, повертаємо оригінальне ім'я
            }

            return name.Replace(form.ShortName, "").Trim();
        }
        static string NormalizeName(string name)
        {
            // Видаляємо лапки, зайві пробіли та символи, приводимо до верхнього регістру
            string normalized = name.ToUpper().Replace("\"", "").Replace("\'", "").Replace("КОМПАНІЯ", "").Replace("ФІРМА", "").Replace("+", "&").Trim();
            normalized = Regex.Replace(normalized, @"\s+", " "); // Видаляємо зайві пробіли
            return normalized;
        }
        static string CorrectCommonMistakes(string input)
        {
            // Виправляємо типові помилки
            Dictionary<string, string> corrections = new Dictionary<string, string>
        {
            { "INC.", "INC" },
            { "LTD.", "LTD" },
            { "GMBX", "GMBH" },
            { "GMDX", "GMBH" },
            { "ГМБХ", "GMBH" },
            { "CO KG", "CO.KG" },
            { "CO.CG", "CO.KG" },
            { " .", "." },
            { ". ", "." },
            { "+", "&" },
            { "& ", "&" },
            { " &", "&" },
            { "-", " " },
            { "”", "" },
            { "“", "" },
            { "(", "" },
            { ")", "" },
            { "¬", "" },
            { "Ё", "Е" },
            { "`", "" },
            { ",", "" },
            { ",,", "" },
            { "...", "" },
            { "<", "" },
            { ">", "" },
            { "«", "" },
            { "»", "" },
            // Додайте інші типові заміни тут
        };

            foreach (var correction in corrections)
            {
                input = input.Replace(correction.Key, correction.Value);
            }

            return input;
        }

        public override string ToString()
        {
            return $"{UniqueName}, {LegalForm?.ShortName}, {Address.ToString()}";
        } // Перевизначаємо метод Equals для порівняння компаній за UniqueName, LegalForm.ShortName і Address
        public override bool Equals(object obj)
        {
            if (obj is Company other)
            {
                return string.Equals(this.UniqueName, other.UniqueName, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(this.LegalForm?.ShortName, other.LegalForm?.ShortName, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(this.Address?.ToString(), other.Address?.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        // Перевизначаємо GetHashCode для забезпечення коректної роботи з колекціями
        public override int GetHashCode()
        {
            return HashCode.Combine(
                UniqueName?.ToUpperInvariant(),
                LegalForm?.ShortName?.ToUpperInvariant(),
                Address?.ToString()?.ToUpperInvariant());
        }
        #endregion
    }
}
