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
        File.WriteAllText(filePath, json);
    }
}
