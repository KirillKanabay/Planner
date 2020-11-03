using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerModel
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PriorityId { get; set; }
        public int CategoryId { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual Category Category { get; set; }
    }
}
