using System;
using System.IO;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Использование: PdfSearcher.exe <путь к PDF> <слово для поиска>");
            return;
        }

        string pdfPath = args[0];
        string keyword = args[1];
        string outputPath = "results.txt";

        if (!File.Exists(pdfPath))
        {
            Console.WriteLine($"Файл не найден: {pdfPath}");
            return;
        }
        Console.WriteLine($"Файл найден: {pdfPath}");

        try
        {
            using (PdfDocument document = PdfDocument.Open(pdfPath))
            {
                Console.WriteLine("PDF успешно открыт");

                Word lastMatch = null;
                int lastPageNumber = -1;

                foreach (Page page in document.GetPages())
                {
                    foreach (Word word in page.GetWords())
                    {
                        if (word.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        {
                            lastMatch = word;
                            lastPageNumber = page.Number;
                        }
                    }
                }

                if (lastMatch != null)
                {
                    var box = lastMatch.BoundingBox;

                    using (StreamWriter writer = new StreamWriter(outputPath))
                    {
                        writer.WriteLine($"{box.Left},{box.Top}");    // Левый верхний
                        writer.WriteLine($"{box.Right},{box.Top}");   // Правый верхний
                        writer.WriteLine($"{box.Left},{box.Bottom}"); // Левый нижний
                        writer.WriteLine($"{box.Right},{box.Bottom}");// Правый нижний
                    }

                    Console.WriteLine($"Последнее вхождение слова \"{keyword}\" найдено на странице {lastPageNumber}");
                    Console.WriteLine($"Координаты записаны в файл: {outputPath}");
                }
                else
                {
                    Console.WriteLine($"Слово \"{keyword}\" не найдено в PDF");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке PDF: {ex.Message}");
        }
    }
}