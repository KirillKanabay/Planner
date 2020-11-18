using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PlannerView.Validators
{
    /// <summary>
    /// Валидатор: Правильная дата
    /// </summary>
    class DateValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateTime time;
            return (!DateTime.TryParse((value ?? "").ToString(), CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces, out time)) 
                ? new ValidationResult(false, "Неверно заполнена дата"):
                ValidationResult.ValidResult;
        }
    }
}
