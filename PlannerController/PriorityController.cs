using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PlannerModel;

namespace PlannerController
{
    /// <summary>
    /// Контроллер приоритета
    /// </summary>
    public class PriorityController
    {
        /// <summary>
        /// Список приоритетов
        /// </summary>
        public ObservableCollection<Priority> Priorities { get; private set; }

        public PriorityController()
        {
            Priorities = GetPriorities() ?? new ObservableCollection<Priority>();
        }
        /// <summary>
        /// Получение приоритетов из БД
        /// </summary>
        /// <returns>Список приоритетов</returns>
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
        /// <summary>
        /// Получение приоритета по его id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Приоритет</returns>
        public Priority GetPriority(int id)
        {
            return Priorities[id - 1];
        }
    }
}
