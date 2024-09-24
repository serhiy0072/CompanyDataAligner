using Newtonsoft.Json;
using System.IO;
using System.Text;

public static class JsonSplitter
{
    // Метод для серіалізації даних і збереження їх у кілька файлів, якщо розмір перевищує 10 МБ
    public static void SaveJsonWithLimit<T>(T data, string baseFilePath, int sizeLimitMB = 5)
    {
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Конвертуємо розмір у байти
        int sizeLimitBytes = sizeLimitMB * 1024 * 1024;

        // Перевіряємо, якщо дані більше 10 МБ, розбиваємо їх на частини
        if (Encoding.UTF8.GetByteCount(jsonData) > sizeLimitBytes)
        {
            SplitAndSaveJson(jsonData, baseFilePath, sizeLimitBytes);
        }
        else
        {
            // Якщо дані менші за 10 МБ, просто зберігаємо їх у файл
            try
            {
                File.WriteAllText($"{baseFilePath}_part1.json", jsonData);
                //Console.WriteLine($"Файл збережено: {baseFilePath}_part1.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка запису у файл: {baseFilePath}_part1.json. Деталі: {ex.Message}");
            }
        }
    }

    // Метод для розбиття і збереження JSON у декілька файлів
    private static void SplitAndSaveJson(string jsonData, string baseFilePath, int sizeLimitBytes)
    {
        // Поділяємо JSON на масив токенів
        var tokens = JsonConvert.DeserializeObject<List<object>>(jsonData);

        if (tokens == null || tokens.Count == 0)
        {
            return;
        }

        int currentPart = 1;
        var currentChunk = new List<object>();
        int currentSize = 0;

        for (int i = 0; i < tokens.Count; i++)
        {
            string chunkJson = JsonConvert.SerializeObject(tokens[i], Formatting.None);
            int chunkSize = Encoding.UTF8.GetByteCount(chunkJson);

            // Додаємо елемент до поточної частини, якщо його розмір не перевищує ліміту
            if (currentSize + chunkSize > sizeLimitBytes)
            {
                // Зберігаємо поточну частину
                SaveChunkToFile(currentChunk, baseFilePath, currentPart);
                currentPart++;
                currentChunk.Clear();
                currentSize = 0;
            }

            currentChunk.Add(tokens[i]);
            currentSize += chunkSize;
        }

        // Зберігаємо останню частину
        if (currentChunk.Count > 0)
        {
            SaveChunkToFile(currentChunk, baseFilePath, currentPart);
        }
    }

    // Метод для збереження частини JSON у файл
    private static void SaveChunkToFile(List<object> chunk, string baseFilePath, int part)
    {
        var chunkJson = JsonConvert.SerializeObject(chunk, Formatting.Indented);
        string filePath = $"{baseFilePath}_part{part}.json";
        try
        {
            File.WriteAllText(filePath, chunkJson);
            //Console.WriteLine($"Файл частини {part} збережено: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка запису у файл: {filePath}. Деталі: {ex.Message}");
        }
    }
}
