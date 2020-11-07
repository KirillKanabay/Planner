using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PlannerModel;

namespace PlannerController
{
    public class PriorityController
    {
        public ObservableCollection<Priority> Items { get; private set; }

        public PriorityController()
        {
            Items = GetPriorities();
        }

        private ObservableCollection<Priority> GetPriorities()
        {
            var priorities = new ObservableCollection<Priority>();
            using (var context = new PlannerContext())
            {
                foreach (var priority in context.Priorities)
                {
                    priorities.Add(priority);
                }
            }

            return priorities;
        }

        public Priority GetPriority(int id)
        {
            return Items[id - 1];
        }
    }
}
