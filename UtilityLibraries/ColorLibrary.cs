using System;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace UtilityLibraries
{
    public static class ColorLibrary
    {
        /// <summary>
        /// Генерирует случайный цвет в HEX-формате.
        /// </summary>
        /// <returns> Цвет в HEX формате. </returns>
        public static string GenerateRGBColor()
        {
            string color = "";
            Random rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 6; i++)
            {
                int hue = rnd.Next(0, 16);
                color += hue.ToString("X");
            }
            return color;
        }
       public static Color GetColor(string color)
       {
            return (Color)ColorConverter.ConvertFromString(color);
       }
       public static bool IsHexColor(string color)
       {
            Regex regex = new Regex(@"^#((([0-9]|[a-f]|[A-F]){6})|(([0-9]|[a-f]|[A-F]){8}))$");
            return regex.IsMatch(color);
       }
    }
}
