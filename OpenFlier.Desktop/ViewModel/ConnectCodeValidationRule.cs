using OpenFlier.Desktop.Localization;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace OpenFlier.Desktop.ViewModel
{
    public class ConnectCodeValidationRule : ValidationRule
    {
        Regex regex = new("^\\d{4}$");
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value.ToString()) || regex.IsMatch(value.ToString()!.Trim()))
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, Backend.ConnectCodeFormatError);
        }
    }
}
