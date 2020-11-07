using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlannerModel
{
    public class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        [ForeignKey("PriorityId")]
        public virtual ICollection<PlannerModel.Task> Tasks { get; set; }

        public Priority()
        {
            //Tasks = new List<Task>();
        }

        public Priority(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }
}
