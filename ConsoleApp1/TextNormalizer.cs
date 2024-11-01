using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class TextNormalizer
    {
        public static string NormalizeName(string input)
        {
            Dictionary<string, string> corrections = new Dictionary<string, string>
            {
                { "НІМЕЧЧИНА ", "" },
                { "+", "&" },
                { " & ", "&" },
                { " &", "&" },
                { "& ", "&" },
                { "\"", " " },
                { "\'", " " },
                { " .", " " },
                { ". ", " " },
                { "Ф-МА", " " },
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
                { "   ", " " },
                { "  ", " " },
                { "А/С", "A/S" },
                { "GMBH @", "GMBH &" },
                { "GESELLSCHAFT MIT BESCHRANKTER HAFTUNG", "GMBH" },
                { "АКЦІОНЕРНЕ ТОВАРИСТВО", "АТ" },
                { " Е V ", " E.V. " },
                { " Е К ", " E.K. " },
                { " E K ", " E.K. " },
                { " E V ", " E.V. " },
                { "ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ", "E.K." },
                { "ГРОМАДЯНИН НІМЕЧЧИНИ", "E.K." },
                { "ГР НІМЕЧЧИНИ", "E.K." },
                { "ЧАСТНЫЙ ПРЕДПРИНИМАТЕЛЬ", "E.K." },
                { "ПРИВАТНИЙ ПІДПРИЄМЕЦЬ", "E.K." },
                { "ФІЗИЧНА ОСОБА", "E.K." },
                { "ТОВАРИСТВО З ОБМЕЖЕНОЮ ВІДПОВІДАЛЬНІСТЮ", "ТОВ" },
                { "ГРОМАДСЬКА ОРГАНІЗАЦІЯ", "ГО" },
                { "КОМАНДИТНЕ ТОВАРИСТВО", "КТ" },
                { "ПІДПРИЄМЕЦЬ", "E.K." },
                { "ТОВ І КТ", "GMBH&CO.KG" },
                { "Г М Б Х", "GMBH" },
                { "Г.М.Б.Х.", "GMBH" },
                { "S P A ", "SPA" },
                { "S.A.S ", "SAS" },
                { "А Г", "AG" },
                { "A.G.", "AG" },
                { "U.G.", "UG" },
                { "CO. KG", "CO.KG" },
                { "CO KG", "CO.KG" },
                { "CO.K ", "CO.KG" },
                { "ДЖІ ЕМ БІ ЕЙЧ", "GMBH" },
                { "GMBH CO", "GMBH&CO" },
                { "AG&CO ", "AG&CO.KG" },
                { "GMBH&KG ", "GMBH&CO.KG" },
                { "KGG ", "KG" },
                // Додайте інші типові заміни тут
            };
            input = CorrectCommonNameMistakes(input);
            foreach (var correction in corrections)
            {
                //string pattern = $@"(?<=\s|^|&|,|\.){Regex.Escape(correction.Key)}(?=\s|$|&|,|\.)";
                //input = Regex.Replace(input, pattern, correction.Value, RegexOptions.IgnoreCase);
                //input = input.Trim('&').Trim();
                input = input.Replace(correction.Key, correction.Value).Trim('&').Trim();
            }

            input = Regex.Replace(input, @"(?<=[a-zA-Z])(?=\d)|(?<=\d)(?=[a-zA-Z])", " ");
            input = Regex.Replace(input, @"\s+", " ");
            return input;
        }
        private static string CorrectCommonNameMistakes(string input)
        {
            // Виправляємо типові помилки
            Dictionary<string, string> corrections = new Dictionary<string, string>
            {
                { "АКЦІОНЕОНЕРНЕ", "АКЦІОНЕРНЕ" },
                { "ТВАРИСТВО", "ТОВАРИСТВО" },
                { "ООО", "GMBH" },
                { "ТОВ", "GMBH" },
                { "ТЗОВ", "GMBH" },
                { "ГУСГМБХ", "ГУС GMBH" },
                { "CMBH", "GMBH" },
                { "GMBX", "GMBH" },
                { "GMBY", "GMBH" },
                { "GMH", "GMBH" },
                { "GBH", "GMBH" },
                { "GMHB", "GMBH" },
                { "GMBD", "GMBH" },
                { "GMDX", "GMBH" },
                { "GMDH", "GMBH" },
                { "ГМБХ", "GMBH" },
                { "ГМБГ", "GMBH" },
                { "ҐМБГ", "GMBH" },
                { "ҐМБХ", "GMBH" },
                { "ГХБХ", "GMBH" },
                { "КГАФ", "KGAA" },
                { "КГАА", "KGAA" },
                { "ГМБ", "GMBH" },
                { "АО", "АТ" },
                { "AT", "АТ" },
                { "МБХ", "GMBH" },
                { "МБГ", "GMBH" },
                { "MBH", "GMBH" },
                { "MBН", "GMBH" },
                { "МВ", "GMBH" },
                { "MB", "GMBH" },
                { "УГ", "UG" },
                { "АГ", "AG" },
                { "КО", "CO" },
                { "KO", "CO" },
                { "СО", "CO" },
                { "КГ", "KG" },
                { "CG", "KG" },
                { "СE", "SE" },
                { "СЕ", "SE" },
                { "CЕ", "SE" },
                { "СЄ", "SE" },
                { "AND", "&" },
                { "ЕНД", "&" },
                { "ЭНД", "&" },
                { "УНД", "&" },
                { "UND", "&" },
                { "ТА", "&" },
                { "INC", "" },
                { "ПП", "E.K." },
                { "ЧП", "E.K." },
                { "ФОП", "E.K." },
                { "І", "&" },
                { "И", "&" },
                { "COKG", "CO.KG" },
                { "КОМПАНІЯ", " " },
                { "КОМПАНИЯ", " " },
                { "ФІРМА", " " },
                { "ФИРМА", " " },
                { "СOMPANY", " " },
                //{ "АСCОЦІАЦІЯ", "E.V." },
                // Додайте інші типові заміни тут
            };

            string normalized = ApplyCorrections(input.Trim(), corrections);
            return normalized;
        }

        public static string NormalizeAddress(string input)
        {
            Dictionary<string, string> corrections = new Dictionary<string, string>
            {
                { "D-", " " },
                { "Д-", " " },
                { "DE-", " " },
                { "ДЕ-", " " },
                { "ВУЛ.", " " },
                { "М.", " " },
                { "&", " & " },
                { "LTD.", "LTD" },
                { "PLCK", "PLANK" },
                { "STR", "STRASSE"  },
                { "STR.", "STRASSE" },
                { "STRA?E", " STRASSE " },
                { "\"", " " },
                { "\'", " " },
                { "\\", " " },
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
                // Додайте інші типові заміни тут
            };

            foreach (var correction in corrections)
            {
                input = input.Replace(correction.Key, correction.Value);
            }
            input = Regex.Replace(input, @"(?<=[a-zA-Z])(?=\d)|(?<=\d)(?=[a-zA-Z])", " ");
            input = Regex.Replace(input, @"\s+", " ");
            input = ReplaceCityNames(input);
            input = CorrectCommonAddressMistakes(input);
            return input;
        }
        private static string CorrectCommonAddressMistakes(string input)
        {
            var corrections = new Dictionary<string, string>
            {

                { "ШТРАСЕ", "STRASSE " },
                { "WIESB", "WIESBADEN " },
                { "WIESBADENADEN", "WIESBADEN " },
                { "WIESBADEN ADEN", "WIESBADEN " },
                { "DELKENHEIM", " " },
                { "DELKENHEIMGERMANY", "DELKENHEIM GERMANY " },
                { "BUNDESLAND", " " },
                { "STATE", " " },
                { "ГАСТЕ", " " },

            };

            string normalized = ApplyCorrections(input.Trim(), corrections);
            return normalized;
        }
        private static string ReplaceCityNames(string address)
        {
            var CityNames = new Dictionary<string, string>
    {
        { "ААХЕН", "AACHEN" },
        { "АУГСБУРГ", "AUGSBURG" },
        { "БЕРЛІН", "BERLIN" },
        { "БОНН", "BONN" },
        { "БРАУНШВЕЙГ", "BRAUNSCHWEIG" },
        { "БРЕМЕН", "BREMEN" },
        { "БРЕМЕРХАВЕН", "BREMERHAVEN" },
        { "ВІТЕН", "WITTEN" },
        { "ГАННОВЕР", "HANNOVER" },
        { "ГАМБУРГ", "HAMBURG" },
        { "ГАСБЕРГЕН", "HASBERGEN" },
        { "ДУЙСБУРГ", "DUISBURG" },
        { "ДЮССЕЛЬДОРФ", "DUSSELDORF" },
        { "ДОРТМУНД", "DORTMUND" },
        { "ДРЕЗДЕН", "DRESDEN" },
        { "ЕННЕПЕТАЛЬ", "ENNEPETAL" },
        { "ЕРФУРТ", "ERFURT" },
        { "ЗАПОРІЖЖЯ", "ZAPOROZHYE" },
        { "КАССЕЛЬ", "KASSEL" },
        { "КЮЛЬН", "KÖLN" },
        { "КІЛЬ", "KIEL" },
        { "КЮНЦЕЛЬЗАУ", "KUNZELSAU" },
        { "ЛЕЙПЦИГ", "LEIPZIG" },
        { "ЛЮБЕК", "LUEBECK" },
        { "МАЙНЦ", "MAINZ" },
        { "МАНГЕЙМ", "MANNHEIM" },
        { "МООРВЕРДЕР", "MOORWERDER" },
        { "МЮЛЕНВІНКЕЛЬ", "MUEHLENWINKEL" },
        { "МЮНХЕН", "MUNICH" },
        { "НОРДЕРДАЙХ", "NORDERDEICH" },
        { "НЮРНБЕРГ", "NUREMBERG" },
        { "ОСНАБРЮК", "OSNABRUECK" },
        { "ПАДЕРБОРН", "PADERBORN" },
        { "ПОРЦГАЙМ", "PFORZHEIM" },
        { "РАЙНХОЛЬ", "REINHOL" },
        { "РОСТОК", "ROSTOCK" },
        { "ФРАНКФУРТ", "FRANKFURT" },
        { "ФРАНКФУРТ-НА-ОДЕРІ", "FRANKFURT-ODER" },
        { "ФРАНКФУРТ НА МАЙНІ", "FRANKFURT" },
        { "ФРАЙБУРГ", "FREIBURG" },
        { "ФУЛЬДА", "FULDA" },
        { "ХАРЗЕВІНКЕЛЬ", "HARSEWINKEL" },
        { "ХАСБЕРГЕН", "HASBERGEN" },
        { "ХЕССЕН", "HESSEN" },
        { "ШАРЛОТТЕНБУРГ", "CHARLOTTENBURG" },
        { "ШТУТГАРТ", "STUTTGART" },
        { "ШВЕРІН", "SCHWERIN" },
        { "ЦВІКАУ", "ZWICKAU" },
    };
            // Проходимо по всіх ключах словника CityNames
            foreach (var city in CityNames)
            {
                // Якщо адреса містить ключ (назву міста), замінюємо його на відповідне значення
                if (address.Contains(city.Key, StringComparison.OrdinalIgnoreCase))
                {
                    address = address.Replace(city.Key, city.Value, StringComparison.OrdinalIgnoreCase);
                }
            }

            // Повертаємо змінену адресу
            return address;
        }

        // Метод для загальних замін на основі словника
        private static string ApplyCorrections(string input, Dictionary<string, string> corrections)
        {
            foreach (var correction in corrections)
            {
                input = Regex.Replace(input, $@"\b{Regex.Escape(correction.Key)}\b", correction.Value, RegexOptions.IgnoreCase);
            }
            input = Regex.Replace(input, @"(?<=[a-zA-Z])(?=\d)|(?<=\d)(?=[a-zA-Z])", " ");
            input = Regex.Replace(input, @"\s+", " ");
            return input;
        }
    }
}
