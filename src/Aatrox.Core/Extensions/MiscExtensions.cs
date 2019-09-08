using System;
using System.Collections.Generic;
using System.Text;

namespace Aatrox.Core.Extensions
{
    public static class MiscExtensions
    {
        public static string Humanize(this TimeSpan timeSpan)
        {
            var strs = new List<string>();
            var str = new StringBuilder();

            if (timeSpan.Days >= 1)
            {
                strs.Add(timeSpan.Days + " day" + (timeSpan.Days > 1 ? "s" : ""));
            }

            if (timeSpan.Hours >= 1)
            {
                strs.Add(timeSpan.Hours + " hr" + (timeSpan.Hours > 1 ? "s" : ""));
            }

            if (timeSpan.Minutes >= 1)
            {
                strs.Add(timeSpan.Minutes + " min" + (timeSpan.Minutes > 1 ? "s" : ""));
            }

            if (timeSpan.Seconds >= 1)
            {
                strs.Add(timeSpan.Seconds + " sec" + (timeSpan.Seconds > 1 ? "s" : ""));
            }

            if (timeSpan.Milliseconds > 0)
            {
                strs.Add(timeSpan.Milliseconds + "ms");
            }

            return str.AppendJoin(", ", strs).ToString();
        }
    }
}
