using System;
using System.Globalization;
using System.Windows.Controls;
namespace PlannerView.Validators
{
    /// <summary>
    /// Валидатор: Правильно введенное время
    /// </summary>
    public class TimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateTime time;
            return DateTime.TryParse((value ?? "").ToString(),
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                out time)
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Неверно заполнено время");
        }
    }
}
