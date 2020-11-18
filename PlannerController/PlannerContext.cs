using PlannerModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerController
{
    /// <summary>
    /// Контекст подключения к БД
    /// </summary>
    public class PlannerContext: DbContext
    {
        public PlannerContext() : base("DbConnectionString") 
        {
            Configuration.LazyLoadingEnabled = false;
        }
        /// <summary>
        /// Таблица приоритетов
        /// </summary>
        public DbSet<Priority> Priorities { get; set; }
        /// <summary>
        /// Таблица категорий
        /// </summary>
        public DbSet<Category> Categories { get; set; }
        /// <summary>
        /// Таблица задач
        /// </summary>
        public DbSet<PlannerModel.Task> Tasks { get; set; }
        
    }
}
