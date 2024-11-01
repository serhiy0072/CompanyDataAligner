namespace ConsoleApp1
{
    public class AnnotatedSentence
    {
        // Список токенів, кожен з яких має слово (Word) та тег (Tag)
        public List<Token> Tokens { get; set; }
        public AnnotatedSentence(List<Token> tokens)
        {
            Tokens = tokens;
        }
    }
}