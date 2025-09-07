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
            Console.WriteLine("Использование: PdfSearchCoordinate.exe <путь к PDF> <слово для поиска>");
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

        try
        {
            using (PdfDocument document = PdfDocument.Open(pdfPath))
            {
                Word lastMatch = null;
                int lastPageNumber = -1;

                foreach (Page page in document.GetPages())
                {
                    var pageWidth = page.Width;
                    var middleX = pageWidth / 2;

                    foreach (Word word in page.GetWords())
                    {
                        if (word.BoundingBox.Left >= middleX)
                        {
                            if (word.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            {
                                lastMatch = word;
                                lastPageNumber = page.Number;
                            }
                        }
                    }
                }

                if (lastMatch != null)
                {
                    var box = lastMatch.BoundingBox;

                    // Записываем в файл
                    using (StreamWriter writer = new StreamWriter(outputPath))
                    {
                        writer.WriteLine($"{lastPageNumber},{box.Left},{box.Top}");
                        writer.WriteLine($"{box.Right},{box.Top}");
                        writer.WriteLine($"{box.Left},{box.Bottom}");
                        writer.WriteLine($"{box.Right},{box.Bottom}");
                    }

                    // Только сообщение об успехе в консоль
                    Console.WriteLine($"Координаты сохранены в файл: {outputPath}");
                }
                else
                {
                    Console.WriteLine($"Слово \"{keyword}\" не найдено в правой части PDF");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке PDF: {ex.Message}");
        }
    }
}