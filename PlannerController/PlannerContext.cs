using PlannerModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerController
{
    public class PlannerContext: DbContext
    {
        public PlannerContext() : base("DbConnectionString") 
        { 
        }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PlannerModel.Task> Tasks { get; set; }
        
    }
}
