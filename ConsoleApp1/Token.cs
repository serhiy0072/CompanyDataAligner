public class Token
{
    public string Word { get; set; }  // Токен (слово або частина слова)
    public string Tag { get; set; }   // Тег (наприклад, B-LEGAL_FORM, I-ORG_NAME)

    public Token(string word, string tag)
    {
        Word = word;
        Tag = tag;
    }
}
