using ConsoleApp1;
using System.Collections.Generic;

public class Tagger
{
    public static List<AnnotatedSentence> AnnotateCompaniesWithTags(List<Company> companies, List<LegalForm> legalForms)
    {
        var annotatedData = new List<AnnotatedSentence>();

        foreach (var company in companies)
        {
            var taggedTokens = new List<Token>();

            // Тегування назви компанії
            if (!string.IsNullOrEmpty(company.UniqueName))
            {
                taggedTokens.AddRange(TagCompanyName(company.UniqueName, legalForms));
            }

            // Тегування адреси
            taggedTokens.AddRange(TagAddress(company.Address));

            // Додаємо розмічене речення до списку
            annotatedData.Add(new AnnotatedSentence(taggedTokens));
        }

        return annotatedData;
    }

    // Метод для тегування назви компанії
    private static IEnumerable<Token> TagCompanyName(string companyName, List<LegalForm> legalForms)
    {
        var tokens = companyName?.Split(new[] { ' ', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        var taggedTokens = new List<Token>();

        bool foundLegalForm = false;
        bool isFirstOrgNameToken = true;

        foreach (var token in tokens ?? Array.Empty<string>())
        {
            if (IsInvalidToken(token)) continue;
            taggedTokens.Add(TagToken(token, legalForms, ref foundLegalForm, ref isFirstOrgNameToken));
        }

        return taggedTokens;
    }

    // Метод для тегування адреси
    private static IEnumerable<Token> TagAddress(Address address)
    {
        var taggedTokens = new List<Token>();
        if (address == null || string.IsNullOrEmpty(address.ToString())) return taggedTokens;

        var addressTokens = address.ToString().Split(new[] { ' ', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        bool isFirstAddressToken = true;

        foreach (var token in addressTokens)
        {
            if (IsInvalidToken(token)) continue;

            taggedTokens.Add(isFirstAddressToken
                ? new Token(token, "B-ADDRESS")
                : new Token(token, "I-ADDRESS"));

            isFirstAddressToken = false;
        }

        return taggedTokens;
    }

    // Загальний метод для перевірки токенів
    private static Token TagToken(string token, List<LegalForm> legalForms, ref bool foundLegalForm, ref bool isFirstOrgNameToken)
    {
        if (IsSymbol(token))
        {
            return new Token(token, "O");  // Якщо це символ, позначаємо як "O"
        }

        if (IsLegalForm(token, legalForms) && !foundLegalForm)
        {
            foundLegalForm = true;
            return new Token(token, "B-LEGAL_FORM");  // Якщо це правова форма і вона ще не знайдена
        }

        if (foundLegalForm)
        {
            return new Token(token, "I-LEGAL_FORM");  // Якщо правова форма знайдена, позначаємо всі інші токени як "I-LEGAL_FORM"
        }

        // Якщо це перше слово назви організації, позначаємо як "B-ORG_NAME", інші - "I-ORG_NAME"
        if (isFirstOrgNameToken)
        {
            isFirstOrgNameToken = false;
            return new Token(token, "B-ORG_NAME");
        }
        else
        {
            return new Token(token, "I-ORG_NAME");
        }
    }

    private static bool IsInvalidToken(string token)
    {
        return string.IsNullOrWhiteSpace(token) || token == "\"" || token == "\'";
    }

    private static bool IsLegalForm(string token, List<LegalForm> legalForms)
    {
        return legalForms.Exists(form => token.Equals(form.ShortName, System.StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsSymbol(string token)
    {
        char[] symbols = { ',', '.', '(', ')', ';', ':', '!', '?', '"', '\'', '-', '/', '\\', '[', ']', '{', '}', '<', '>' };
        return token.Length == 1 && symbols.Contains(token[0]);
    }
}
