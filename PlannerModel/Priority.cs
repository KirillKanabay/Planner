using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlannerModel
{
    /// <summary>
    /// Модель приоритета
    /// </summary>
    public class Priority
    {
        /// <summary>
        /// Id приоритета
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название приоритета
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Цвет приоритета
        /// </summary>
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
