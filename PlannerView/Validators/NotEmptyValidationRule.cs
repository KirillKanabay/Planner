using System.Globalization;
using System.Windows.Controls;

namespace PlannerView.Validators
{
    public class NotEmptyValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "Поле обязательно для заполнения")
                : ValidationResult.ValidResult;
        }
    }
}
