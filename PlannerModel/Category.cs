﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerModel
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }

        public Category()
        {
        }
        public Category(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }
}
