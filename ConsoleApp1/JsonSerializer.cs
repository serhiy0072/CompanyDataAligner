using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class JsonSerializer
    {
        // Збереження розмічених даних у форматі JSON
        public static void SaveAnnotationsAsJson(List<AnnotatedSentence> annotatedData, string filePath)
        {
            var json = JsonConvert.SerializeObject(annotatedData, Newtonsoft.Json.Formatting.Indented);
            try
            {
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Файл JSON успішно збережено: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка запису у файл {filePath}: {ex.Message}");
            }
        }
        public static void SaveAnnotationsAsText(List<AnnotatedSentence> annotatedData, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var sentence in annotatedData)
                {
                    foreach (var token in sentence.Tokens)
                    {
                        // Кожен токен з його тегом записуємо у новий рядок
                        writer.WriteLine($"{token.Word} {token.Tag}");
                    }
                    // Порожній рядок між реченнями
                    writer.WriteLine();
                }
            }

            Console.WriteLine($"Файл успішно збережено у форматі для навчання: {filePath}");
        }
    }
}