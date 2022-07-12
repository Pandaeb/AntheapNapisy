using System;
using System.Collections.Generic;

namespace AntheapNapisy
{
    public class SubtitleItem
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<string> TextLines { get; set; }

        public SubtitleItem()
        {
            this.TextLines = new List<string>();
        }
        public override string ToString()
        {
            var timeStamps = string.Format("{0} --> {1}", StartTime.ToString(@"hh\:mm\:ss\,fff"), EndTime.ToString(@"hh\:mm\:ss\,fff"));
            var result = string.Join(Environment.NewLine,
                                        timeStamps,
                                        string.Join(Environment.NewLine, TextLines));
            return result;
        }
    }
}
