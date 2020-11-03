using System;
using System.Collections.Generic;
using PlannerModel;

namespace PlannerController
{
    public class TaskController
    {
        public List<PlannerModel.Task> Tasks { get; private set; }

        public TaskController()
        {
            Tasks = GetTasks() ?? new List<Task>();
        }
        public TaskController(string name, DateTime startTime, DateTime endTime, int priorityId, int categoryId)
        {
            AddTask(name, startTime, endTime, priorityId,categoryId);
        }

        public void AddTask(string name, DateTime startTime, DateTime endTime, int priorityId, int categoryId)
        {
            #region Проверка условий

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Название задачи не может быть пустым.");
            }

            if (startTime == null)
            {
                startTime = DateTime.Now;
            }

            if (endTime == null)
            {
                startTime = DateTime.MaxValue;
            }

            if (priorityId <= 0)
            {
                throw new ArgumentException("Неправильно заполнено значение приоритета.");
            }

            if (categoryId <= 0)
            {
                throw new ArgumentException("Неправильно заполнено значение категории.");
            }
            #endregion
            var task = new PlannerModel.Task()
            {
                Name = name,
                CreationDate = DateTime.Now,
                StartTime = startTime,
                EndTime = endTime,
                PriorityId = priorityId,
                CategoryId = categoryId
            };
            AddTaskDb(task);
        }
        private void AddTaskDb(PlannerModel.Task task)
        {
            using (var context = new PlannerContext())
            {
                context.Tasks.Add(task);
                context.SaveChanges();
            }
        }
        /// <summary>
        /// Возвращает список категорий из БД
        /// </summary>
        /// <returns>Список категорий</returns>
        private List<PlannerModel.Task> GetTasks()
        {
            var tasks = new List<PlannerModel.Task>();
            using (var context = new PlannerContext())
            {
                if (context.Tasks == null)
                {
                    return null;
                }
                foreach (var task in context.Tasks)
                {
                   tasks.Add(task);
                }
            }

            return tasks;
        }
    }
}
