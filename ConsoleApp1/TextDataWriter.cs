using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class TextDataWriter
    {
        public static void SaveAnnotationsAsText(List<AnnotatedSentence> annotatedData, string filePath, int rowsPerFile = 20000)
        {
            int fileIndex = 1;
            int currentRowCount = 0;
            List<AnnotatedSentence> currentBatch = new List<AnnotatedSentence>();

            foreach (var sentence in annotatedData)
            {
                int sentenceRows = sentence.Tokens.Count + 1; // кількість рядків для речення + порожній рядок

                // Якщо додавання цього речення перевищить кількість рядків на файл, зберігаємо поточний пакет
                if (currentRowCount + sentenceRows > rowsPerFile && currentBatch.Count > 0)
                {
                    WriteBatchToFile(currentBatch, $"{filePath}_part{fileIndex}.txt");
                    fileIndex++;
                    currentBatch.Clear();
                    currentRowCount = 0;
                }

                currentBatch.Add(sentence);
                currentRowCount += sentenceRows;
            }

            // Зберегти залишок даних
            if (currentBatch.Count > 0)
            {
                WriteBatchToFile(currentBatch, $"{filePath}_part{fileIndex}.txt");
            }

            Console.WriteLine("Файли успішно збережено у форматі для навчання.");
        }

        // Допоміжний метод для запису пакету речень у файл
        private static void WriteBatchToFile(List<AnnotatedSentence> batch, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var sentence in batch)
                {
                    foreach (var token in sentence.Tokens)
                    {
                        writer.WriteLine($"{token.Word} {token.Tag}");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
