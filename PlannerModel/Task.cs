using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlannerModel
{
    /// <summary>
    /// Модель задачи
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Id задачи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название задачи
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Дата создания задачи
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Дата начала задачи
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Дата окончания задачи
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Дата завершения задачи
        /// </summary>
        public DateTime? FinishDate { get; set; }
        /// <summary>
        /// Является ли задача просроченной
        /// </summary>
        public bool IsOverdue { get; set; }
        /// <summary>
        /// Является ли задача завершенной
        /// </summary>
        public bool IsFinished { get; set; }
        /// <summary>
        /// Id приоритета задачи
        /// </summary>
        public int PriorityId { get; set; }
        [ForeignKey("PriorityId")]
        public virtual Priority Priority { get; set; }
        /// <summary>
        /// Id Категории задачи
        /// </summary>
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
       
        
       
    }
}
