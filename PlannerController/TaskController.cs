using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PlannerModel;

namespace PlannerController
{
    public class TaskController
    {
        public ObservableCollection<Task> Tasks { get; private set; }

        public TaskController()
        {
            Tasks = GetTasks() ?? new ObservableCollection<Task>();
        }
        public TaskController(string name, DateTime startTime, DateTime endTime, int priorityId, int categoryId):this()
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

            var tempTask = Tasks.SingleOrDefault(t => t.Name == name);
            if (tempTask != null)
            {
                throw new ArgumentException("Такая задача уже существует.");
            }
            #endregion
            var task = new Task()
            {
                Name = name,
                CreationDate = DateTime.Now,
                StartTime = startTime,
                EndTime = endTime,
                PriorityId = priorityId,
                CategoryId = categoryId
            };
            AddTaskDb(task);
            Tasks = GetTasks();
        }
        private void AddTaskDb(Task task)
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
        private ObservableCollection<Task> GetTasks()
        {
            var tasks = new ObservableCollection<Task>();
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

        public void FinishTask(int taskId)
        {
            using (var context = new PlannerContext())
            {
                var task = context.Tasks.FirstOrDefault(x => x.Id == taskId);
                task.IsFinished = true;
                context.SaveChanges();
            }

            Tasks = GetTasks();
        }
    }
}
