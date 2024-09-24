    using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

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
}
