using System;
using System.Collections.Generic;
using PlannerModel;

namespace PlannerController
{
    public class PriorityController
    {
        public List<Priority> Priorities { get; private set; }

        public PriorityController()
        {
            Priorities = GetPriorities();
        }

        private List<Priority> GetPriorities()
        {
            var priorities = new List<Priority>();
            using (var context = new PlannerContext())
            {
                foreach (var priority in context.Priorities)
                {
                    priorities.Add(priority);
                }
            }

            return priorities;
        }
    }
}
