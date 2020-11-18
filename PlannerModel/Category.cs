using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerModel
{
    /// <summary>
    /// Модель категории задачи
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Id категории
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название категории
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Цвет категории
        /// </summary>
        public string Color { get; set; }
        public ICollection<PlannerModel.Task> Tasks { get; set; }

        public Category()
        {
            //Tasks = new List<Task>();
        }
        public Category(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }
}
