using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLibraries
{
    /// <summary>
    /// Расширение для работы с датой
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Получение дня понедельника на неделе
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetNumberOfMonday(DateTime dt)
        {
            while (dt.DayOfWeek != DayOfWeek.Monday)
               dt = dt.AddDays(-1);

            return dt.Day;
        }
    }
}
