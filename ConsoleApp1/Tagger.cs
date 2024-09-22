using ConsoleApp1;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Collections.Generic;

public class Tagger
{
    public static List<AnnotatedSentence> AnnotateCompaniesWithTags(List<Company> companies, List<LegalForm> legalForms)
    {
        var annotatedData = new List<AnnotatedSentence>();

        foreach (var company in companies)
        {
            var tokens = company.FullName.Split(' ');
            var taggedTokens = new List<Token>();

            bool foundLegalForm = false;

            // Додаємо теги для правової форми
            foreach (var token in tokens)
            {
                if (IsLegalForm(token, legalForms) && !foundLegalForm)
                {
                    taggedTokens.Add(new Token(token, "B-LEGAL_FORM"));
                    foundLegalForm = true;
                }
                else if (foundLegalForm)
                {
                    taggedTokens.Add(new Token(token, "I-LEGAL_FORM"));
                }
                else
                {
                    taggedTokens.Add(new Token(token, "B-ORG_NAME"));
                }
            }

            // Розбиття адреси на слова і додавання тегів для адреси
            var addressTokens = company.Address.OriginalAddress.Split(' ');
            foreach (var token in addressTokens)
            {
                taggedTokens.Add(new Token(token, "B-ADDRESS"));
            }

            // Додаємо розмічені речення до списку
            annotatedData.Add(new AnnotatedSentence(taggedTokens));
        }
        return annotatedData;
    }

    public static bool IsLegalForm(string token, List<LegalForm> legalForms)
    {
        foreach (var form in legalForms)
        {
            if (token.Equals(form.ShortName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
