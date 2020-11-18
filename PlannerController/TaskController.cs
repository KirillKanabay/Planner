using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PlannerModel;

namespace PlannerController
{
    /// <summary>
    /// Контроллер задачи
    /// </summary>
    public class TaskController
    {
        /// <summary>
        /// Хранит список задач
        /// </summary>
        public ObservableCollection<Task> Tasks { get; private set; }

        /// <summary>
        /// Конструктор заполняющий список задач
        /// </summary>
        public TaskController()
        {
            Tasks = GetTasks() ?? new ObservableCollection<Task>();
        }

        /// <summary>
        /// Конструктор принимающий на входе задачу, и в зависимости от флага редактируется или добавляется в бд
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="isEdit">Флаг редактирования</param>
        public TaskController(Task task, bool isEdit = false) : this()
        {
            #region Проверка условий

            if (string.IsNullOrWhiteSpace(task.Name))
            {
                throw new ArgumentException("Название задачи не может быть пустым.");
            }

            if (task.StartDate == null)
            {
                task.StartDate = DateTime.Now;
            }

            if (task.EndDate == null)
            {
                task.EndDate = DateTime.MaxValue;
            }

            if (task.EndDate <= task.StartDate)
            {
                throw new ArgumentException("Время начала задачи не может быть позже даты окончания");
            }

            if (task.PriorityId <= 0)
            {
                throw new ArgumentException("Неправильно заполнено значение приоритета.");
            }

            if (task.CategoryId <= 0)
            {
                throw new ArgumentException("Неправильно заполнено значение категории.");
            }

            var tempTask =
                Tasks.SingleOrDefault(t => t.Name == task.Name && t.CategoryId == task.CategoryId && !isEdit);
            if (tempTask != null)
            {
                throw new ArgumentException("Такая задача уже существует.");
            }

            #endregion

            task.CreationDate = DateTime.Now;
            if (isEdit)
            {
                EditTask(task);
            }
            else
            {
                AddTaskDb(task);
            }

            Tasks = GetTasks();
        }


        #region Взаимодействие с БД

        /// <summary>
        /// Добавляет задачу в БД
        /// </summary>
        /// <param name="task"></param>
        private void AddTaskDb(Task task)
        {
            using (var context = new PlannerContext())
            {
                context.Tasks.Add(task);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Возвращает список задач из БД
        /// </summary>
        /// <returns>Список задач</returns>
        private ObservableCollection<Task> GetTasks()
        {
            PriorityController priorityController = new PriorityController();
            CategoryController categoryController = new CategoryController();
            var tasks = new ObservableCollection<Task>();
            using (var context = new PlannerContext())
            {
                if (context.Tasks == null)
                {
                    return null;
                }

                foreach (var taskContext in context.Tasks)
                {
                    var task = new Task()
                    {
                        Id = taskContext.Id,
                        Name = taskContext.Name,
                        CategoryId = taskContext.CategoryId,
                        Category = categoryController.GetCategoryById(taskContext.CategoryId),
                        PriorityId = taskContext.PriorityId,
                        Priority = priorityController.GetPriority(taskContext.PriorityId),
                        CreationDate = taskContext.CreationDate,
                        EndDate = taskContext.EndDate,
                        IsFinished = taskContext.IsFinished,
                        StartDate = taskContext.StartDate,
                        FinishDate = taskContext.FinishDate,
                        IsOverdue = taskContext.IsOverdue
                    };
                    tasks.Add(task);
                }
            }

            return tasks;
        }

        /// <summary>
        /// Завершает задачу
        /// </summary>
        /// <param name="taskId">Id задачи</param>
        public void FinishTask(int taskId)
        {
            using (var context = new PlannerContext())
            {
                var task = context.Tasks.FirstOrDefault(x => x.Id == taskId);
                task.FinishDate = DateTime.Now;
                task.IsFinished = true;
                context.SaveChanges();
            }

            Tasks = GetTasks();
        }

        /// <summary>
        /// Помечает задачу просроченной
        /// </summary>
        /// <param name="taskId">Id задачи</param>
        public void OverdueTask(int taskId)
        {
            using (var context = new PlannerContext())
            {
                var task = context.Tasks.FirstOrDefault(x => x.Id == taskId);
                task.IsOverdue = true;
                context.SaveChanges();
            }

            Tasks = GetTasks();
        }

        /// <summary>
        /// Снимает пометку просроченности у задачи
        /// </summary>
        /// <param name="taskId">Id задачи</param>
        public void UnOverdueTask(int taskId)
        {
            using (var context = new PlannerContext())
            {
                var task = context.Tasks.FirstOrDefault(x => x.Id == taskId);
                task.IsOverdue = false;
                context.SaveChanges();
            }

            Tasks = GetTasks();
        }

        /// <summary>
        /// Редактирует задачу
        /// </summary>
        /// <param name="editedTask">Задача</param>
        public void EditTask(Task editedTask)
        {
            using (var context = new PlannerContext())
            {
                var task = context.Tasks.FirstOrDefault(x => x.Id == editedTask.Id);
                task.Name = editedTask.Name;
                task.CreationDate = editedTask.CreationDate;
                task.StartDate = editedTask.StartDate;
                task.EndDate = editedTask.EndDate;
                task.PriorityId = editedTask.PriorityId;
                task.CategoryId = editedTask.CategoryId;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Удаляет задачу
        /// </summary>
        /// <param name="id">Id задачи</param>
        public void DeleteTask(int id)
        {
            using (var context = new PlannerContext())
            {
                var task = context.Tasks.FirstOrDefault(x => x.Id == id);
                context.Tasks.Remove(task);
                context.SaveChanges();
            }

            Tasks = GetTasks();
        }

        #endregion

        #region Взаимодействие с задачей
        /// <summary>
        /// Пометка просроченных задач
        /// </summary>
        public void MarkOverdueTasks()
        {
            foreach (var task in Tasks)
            {
                if (task.EndDate < DateTime.Now && !task.IsFinished)
                {
                    OverdueTask(task.Id);
                }
                else if (task.EndDate > DateTime.Now && !task.IsFinished && task.IsOverdue)
                {
                    UnOverdueTask(task.Id);
                }
            }
        }


        #endregion
    }
}

