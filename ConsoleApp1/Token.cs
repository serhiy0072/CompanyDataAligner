namespace ConsoleApp1
{
    public class Token
    {
        public string Word { get; set; }
        public string Tag { get; set; }

        public Token(string word, string tag)
        {
            Word = word;
            Tag = tag;
        }
    }
}