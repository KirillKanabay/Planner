using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UtilityLibraries;

namespace PlannerView.Validators
{
    /// <summary>
    /// Валидатор: Правильный формат цвета
    /// </summary>
    class CorrectlyColorValidationRule: ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return !ColorExtensions.IsHexColor((value ?? "").ToString())
                ? new ValidationResult(false, "Неправильно заполнено поле цвета")
                : ValidationResult.ValidResult;
        }
    }
}
