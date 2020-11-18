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
    /// Валидатор: Правильная дата или пустая строка
    /// </summary>
    class DateOrEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateTime time;
            if((value ?? "").ToString().Trim() == "")
                return ValidationResult.ValidResult;
            
            return (!DateTime.TryParse((value ?? "").ToString(), CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces, out time))
                ? new ValidationResult(false, "Неверно заполнена дата")  :
                ValidationResult.ValidResult;
        }
    }
}