using System;
using System.Collections.Generic;
using System.IO;

namespace AntheapNapisy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var fileStream = File.OpenRead("napisy do filmu.srt");

            var processor = new SubtitleProcessor();
            var dividedItems = processor.StreamToSubtitleItems(fileStream);

            List<SubtitleItem> originalItems = new List<SubtitleItem>(dividedItems);

            List<SubtitleItem> itemsToMove = new List<SubtitleItem>();

            foreach(var item in dividedItems)
            {
                item.StartTime += new TimeSpan(0, 0, 0, 5, 880);
                item.EndTime += new TimeSpan(0, 0, 0, 5, 880);

                if (item.StartTime.Milliseconds == 0)
                {
                    itemsToMove.Add(item);
                    originalItems.Remove(item);
                }
            }

            using (TextWriter firstFileWriter = new StreamWriter("Napisy1.srt"))
            {
                for (int i = 0; i < originalItems.Count; i++)
                {
                    SubtitleItem item = originalItems[i];

                    firstFileWriter.WriteLine(string.Format("{0}", i + 1));
                    firstFileWriter.WriteLine(item.ToString());
                    firstFileWriter.WriteLine();
                }
            }

            using (TextWriter secondFileWriter = new StreamWriter("Napisy2.srt"))
            {
                for (int i = 0; i < itemsToMove.Count; i++)
                {
                    SubtitleItem item = itemsToMove[i];

                    secondFileWriter.WriteLine(string.Format("{0}", i + 1));
                    secondFileWriter.WriteLine(item.ToString());
                    secondFileWriter.WriteLine();
                }
            }
        }
    }
}
