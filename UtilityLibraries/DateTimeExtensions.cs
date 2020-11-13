using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLibraries
{
    static public class DateTimeExtensions
    {
        static public int GetNumberOfMonday(DateTime dt)
        {
            while (dt.DayOfWeek != DayOfWeek.Monday)
               dt = dt.AddDays(-1);

            return dt.Day;
        }
    }
}
