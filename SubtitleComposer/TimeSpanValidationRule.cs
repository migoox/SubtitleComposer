using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SubtitleComposer
{
    public class TimeSpanValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(Regex.IsMatch((string)value, 
                @"^([0-9]{1,2})?(:[0-9]{1,2}){0,2}(\.[0-9]{1,3})?$"), $"Incorrect format");
        }
    }
}
