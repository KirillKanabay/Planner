using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLibraries
{
    public class ColorLibrary
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
    }
}
