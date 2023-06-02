using System.Globalization;
using System.Windows.Data;
using System;
using System.Text;
using System.Windows;

namespace SubtitleComposer
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan timeSpan = (TimeSpan)value;
            StringBuilder bd = new StringBuilder();

            if (timeSpan.Hours != 0)
                bd.Append(timeSpan.Hours.ToString());
            if (timeSpan.Minutes != 0)
                bd.Append(':').Append(timeSpan.Minutes.ToString());
            if (timeSpan.Seconds != 0)
                bd.Append(':').Append(timeSpan.Seconds.ToString());
            if (timeSpan.Milliseconds != 0)
                bd.Append('.').Append(timeSpan.Milliseconds.ToString().PadLeft(3, '0').TrimEnd('0'));

            return bd.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            if (string.IsNullOrEmpty(stringValue))
            {
                return TimeSpan.Zero;
            }

            Span<int> numbers = stackalloc int[4] { 0, 0, 0, 0 };

            var parts = stringValue.Split(':');

            int dotInd = parts[^1].IndexOf('.');
            if (dotInd != -1)
            {
                string ms = parts[^1][(dotInd+1)..parts[^1].Length].PadRight(3, '0');
                if (!int.TryParse(ms, out numbers[3]))
                {
                    numbers[3] = 0;
                }
                parts[^1] = parts[^1][0..dotInd];
            }

            for (int i = parts.Length - 1, j = 2; i >= 0; --i, --j)
            {
                if (!int.TryParse(parts[i], out numbers[j]))
                {
                    numbers[j] = 0;
                }
            }

            return new TimeSpan(0, numbers[0], numbers[1], numbers[2], numbers[3]);
        }
    }
}
