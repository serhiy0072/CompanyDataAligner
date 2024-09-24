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
        public int Id { get; private set; }
        public string? CountryId { get; private set; }
        public string? FullName { get; private set; }
        public string? UniqueName { get; set; }
        public Address Address { get; private set; }
        public LegalForm? LegalForm { get; set; }
        #endregion
        #region Constructors 
        public Company(string name, string fullAddress, string countryId)
        {
            FullName = name;
            UniqueName = CorrectCommonMistakes(NormalizeName(name));
            Address = new Address(fullAddress, countryId);
            CountryId = countryId;
        }
        #endregion
        #region Methods

        static string NormalizeName(string input)
        {
            Dictionary<string, string> corrections = new Dictionary<string, string>
            {
                { "&", " & " },
                { "\"", " " },
                { "\'", " " },
                { ".", " " },
                { "+", " " },
                { "-", " " },
                { "”", " " },
                { "“", " " },
                { "(", " " },
                { ")", " " },
                { "¬", " " },
                { "`", " " },
                { ",", " " },
                { ",,", " " },
                { "...", " " },
                { "<", " " },
                { ">", " " },
                { "«", " " },
                { "»", " " },
                { "  ", " " },
                { "КОМПАНІЯ", " " },
                { "КОМПАНИЯ", " " },
                { "ФІРМА", " " },
                { "Г М Б Х", "GMBH" },
                { "ГМБХ&КО.КГ", "GMBH&CO.KG" },
                { "ГМБ&КГ", "GMBH&CO.KG" },
                { "ГМБ & КО КГ", "GMBH&CO.KG" },
                { "ГМБГ І КО", "GMBH&CO.KG" },
                { "ГМБГ ТА КО КГ", "GMBH&CO.KG" },
                { "ҐМБГ І КО КҐ", "GMBH&CO.KG" },
                { "ҐМБХ І КО КҐ", "GMBH&CO.KG" },
                { "GMBH AND CO KG", "GMBH&CO.KG" },
                { "GMBH UND CO KG", "GMBH&CO.KG" },
                { "GMBH & CO KG", "GMBH&CO.KG" },
                { "GMBH & КО КГ", "GMBH&CO.KG" },
                { "GMBH І СО КГ", "GMBH&CO.KG" },
                { "GMBH І КО КГ", "GMBH&CO.KG" },
                { "GMBH ЕНД КО КГ", "GMBH&CO.KG" },
                { "GMBH CO KG", "GMBH&CO.KG" },
                { "GMBH КО КГ", "GMBH&CO.KG" },
                { "CЕ ЕНД КО КГ", "SE&CO.KG" },
                { "АКЦІОНЕРНЕ ТОВАРИСТВО", "GMBH" },
                { "АКЦІОНЕОНЕРНЕ ТВАРИСТВО", "GMBH" },
                { "АСCОЦІАЦІЯ", "E.V." },
                // Додайте інші типові заміни тут
           };
            // Видаляємо лапки, зайві пробіли та символи

            string normalized = input.Trim();
            foreach (var correction in corrections)
            {
                normalized = normalized.Replace(correction.Key, correction.Value);
            }
            normalized = Regex.Replace(normalized, @"(?<=[a-zA-Z])(?=\d)|(?<=\d)(?=[a-zA-Z])", " ");
            return normalized;
        }
        static string CorrectCommonMistakes(string input)
        {
            // Виправляємо типові помилки
            Dictionary<string, string> corrections = new Dictionary<string, string>
            {
                
                { "CMBH", "GMBH" },
                { "GMBX", "GMBH" },
                { "GMBY", "GMBH" },
                { "GMHB", "GMBH" },
                { "GMBD", "GMBH" },
                { "GMDX", "GMBH" },
                { "GMDH", "GMBH" },
                { "ГМБХ", "GMBH" },
                { "ГМБГ", "GMBH" },
                { "ГХБХ", "GMBH" },
                { "ТОВ", "GMBH" },
                { "АТ", "GMBH" },
                { "LLC", "GMBH" },
                { "INC", "" },
                { "МБХ", "MBH" },
                { "МБГ", "MBH" },
                { "УГ", "UG" },
                { "АГ", "AG" },
                { "КГ", "KG" },
                // Додайте інші типові заміни тут
            };

            foreach (var correction in corrections)
            {
                //Замінюємо помилкові слова, якщо вони є окремими словами
                input = Regex.Replace(input, $@"\b{Regex.Escape(correction.Key)}\b", correction.Value);
            }

            input = Regex.Replace(input, @"\s+", " ").Trim(); // Видаляємо зайві пробіли
            return input;
        }

        public override string ToString()
        {
            return $"{UniqueName}, {LegalForm?.ShortName}, {Address.ToString()}";
        } 
        
        // Перевизначаємо метод Equals для порівняння компаній за UniqueName, LegalForm.ShortName і Address
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
