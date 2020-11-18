using System;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace UtilityLibraries
{
    /// <summary>
    /// Расширение для работы с цветом
    /// </summary>
    public static class ColorExtensions
    { 
        /// <summary>
        /// Получаем цвет типа Color из строки
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetColorFromString(string color)
       {
            return (Color)ColorConverter.ConvertFromString(color);
       }
        /// <summary>
        /// Проверка строки на цвет
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
       public static bool IsHexColor(string color)
       {
            Regex regex = new Regex(@"^#((([0-9]|[a-f]|[A-F]){6})|(([0-9]|[a-f]|[A-F]){8}))$");
            return regex.IsMatch(color);
       }
    }
}
