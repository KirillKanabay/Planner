using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerController
{
    interface IController
    {
        List<object> Items { get; set; }
        List<object> GetItems();
        List<object> SetItems();

    }
}
