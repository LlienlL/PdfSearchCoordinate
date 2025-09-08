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
        
        // Получаем имя файла без расширения для названия результата
        string fileName = Path.GetFileNameWithoutExtension(pdfPath);
        string outputPath = $"result_{fileName}.txt";

        if (!File.Exists(pdfPath))
        {
            Console.WriteLine($"101333101 Файл не найден: {pdfPath}");
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

                    // Округляем координаты до целых чисел
                    int left = (int)Math.Round(box.Left);
                    int bottom = (int)Math.Round(box.Bottom);

                    // Записываем в файл (полный формат с оригинальными координатами)
                    using (StreamWriter writer = new StreamWriter(outputPath))
                    {
                        writer.WriteLine($"{lastPageNumber},{box.Left},{box.Top}");
                        writer.WriteLine($"{box.Right},{box.Top}");
                        writer.WriteLine($"{box.Left},{box.Bottom}");
                        writer.WriteLine($"{box.Right},{box.Bottom}");
                    }

                    // Выводим в консоль только: страница,Left,Bottom (округленные)
                    Console.WriteLine($"{lastPageNumber},{left},{bottom}");
                }
                else
                {
                    Console.WriteLine($"101333101 Слово \"{keyword}\" не найдено в правой части PDF");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"101333101 Ошибка при обработке PDF: {ex.Message}");
        }
    }
}