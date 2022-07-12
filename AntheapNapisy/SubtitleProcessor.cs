using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AntheapNapisy
{
    public class SubtitleProcessor
    {
        public List<SubtitleItem> StreamToSubtitleItems(Stream subStream)
        {
            subStream.Position = 0;

            TextReader subReader = new StreamReader(subStream, Encoding.UTF8, true);

            List<SubtitleItem> items = new List<SubtitleItem>();

            var subLines = DivideSubLines(subReader).ToList();

            if (subLines.Any())
            {
                foreach (var subLine in subLines)
                {
                    var lines = subLine.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                            .Select(s => s.Trim())
                            .Where(l => !string.IsNullOrEmpty(l))
                            .ToList();

                    var item = new SubtitleItem();
                    foreach(var line in lines)
                    {
                        if (item.StartTime == TimeSpan.Zero && item.EndTime == TimeSpan.Zero)
                        {
                            TimeSpan startTime;
                            TimeSpan endTime;
                            var success = TryParseTimecodeLine(line, out startTime, out endTime);
                            if (success)
                            {
                                item.StartTime = startTime;
                                item.EndTime = endTime;
                            }
                        }
                        else
                        {
                            item.TextLines.Add(line);
                        }
                    }

                    if ((item.StartTime != TimeSpan.Zero || item.EndTime != TimeSpan.Zero) && item.TextLines.Any())
                    {
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        private bool TryParseTimecodeLine(string line, out TimeSpan startTime, out TimeSpan endTime)
        {
            var parts = line.Split("-->", StringSplitOptions.None);
            if (parts.Length != 2)
            {
                startTime = TimeSpan.Zero;
                endTime = TimeSpan.Zero;
                return false;
            }
            else
            {
                startTime = ParseSrtTimecode(parts[0]);
                endTime = ParseSrtTimecode(parts[1]);
                return true;
            }
        }
        private static TimeSpan ParseSrtTimecode(string s)
        {
            TimeSpan result = TimeSpan.Zero;
            var match = Regex.Match(s, "[0-9]+:[0-9]+:[0-9]+([,\\.][0-9]+)?");
            if (match.Success)
            {
                s = match.Value;
                result = TimeSpan.ParseExact(s, @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
            }
            return result;
        }

        private IEnumerable<string> DivideSubLines(TextReader reader)
        {
            var strBuilder = new StringBuilder();
            string singleLine = String.Empty;

            while((singleLine = reader.ReadLine()) != null)
            {
                if (String.IsNullOrEmpty(singleLine.Trim()))
                {
                    var res = strBuilder.ToString().TrimEnd();
                    if (!string.IsNullOrEmpty(res))
                    {
                        yield return res;
                    }
                    strBuilder = new StringBuilder();
                }
                else
                {
                    strBuilder.AppendLine(singleLine);
                }
            }

            if (strBuilder.Length > 0)
            {
                yield return strBuilder.ToString();
            }
        }
    }
}
