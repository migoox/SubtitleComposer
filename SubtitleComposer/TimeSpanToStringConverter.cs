using System.Globalization;
using System.Windows.Data;
using System;
using System.Windows;

namespace SubtitleComposer
{
public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            string format = "h\\:mm\\:ss\\.fff";
            if (timeSpan.Hours == 0)
            {
                format = "mm\\:ss\\.fff";

                if (timeSpan.Minutes == 0)
                {
                    format = "ss\\.fff";
                }
            }

            return timeSpan.ToString(format, culture);
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string stringValue = value as string;
        if (string.IsNullOrEmpty(stringValue))
        {
            return TimeSpan.Zero;
        }

        TimeSpan timeSpan;
        if (TimeSpan.TryParseExact(stringValue, new[] { "c", "G", "g", "hh\\:mm\\:ss\\.fff", "hh\\:mm\\:ss", "m\\:ss\\.fff", "s\\.f", "s\\:f", "s", "m\\:s\\.f", "m\\:s", "h\\:m\\:s\\.f", "h\\:m\\:s", "mm\\:ss\\.fff", "ss\\.fff" }, culture, out timeSpan))
        {
            return timeSpan;
        }

        return Binding.DoNothing;
    }
}

}
