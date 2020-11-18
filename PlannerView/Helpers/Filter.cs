using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlannerModel;
using Task = PlannerModel.Task;

namespace PlannerView.Helpers
{
    /// <summary>
    /// Класс помощник: Фильтрация задач для планировщика задач
    /// </summary>
    public class Filter
    {
        #region Свойства
        /// <summary>
        /// Поисковая строка
        /// </summary>
        public string searchString;
        
        /// <summary>
        /// Id категории в фильтре
        /// </summary>
        public int categoryIdFilter;
        /// <summary>
        /// Id приоритета в фильтре
        /// </summary>
        public int priorityIdFilter;

        /// <summary>
        /// Фильтр незавершенной задачи
        /// </summary>
        public bool isNotFinishedFilter = true;
        /// <summary>
        /// Фильтр незавершенной задачи
        /// </summary>
        public bool isFinishedFilter;
        /// <summary>
        /// Фильтр незавершенной задачи
        /// </summary>
        public bool isOverdueFilter;

        /// <summary>
        /// Фильтр начала задачи
        /// </summary>
        public DateTime startDate;
        /// <summary>
        /// Фильтр окончания задачи
        /// </summary>
        public DateTime endDate;

        /// <summary>
        /// Главный фильтр меню
        /// </summary>
        public Func<PlannerModel.Task, bool> menuFilterMain;
        /// <summary>
        /// Фильтр: все задачи
        /// </summary>
        public readonly Func<PlannerModel.Task, bool> menuFilterAllTask = (task) => true;
        /// <summary>
        /// Фильтр: Задачи на сегодня
        /// </summary>
        public readonly Func<PlannerModel.Task, bool> menuFilterTodayTask = (task) => DateTime.Today >= new DateTime(task.StartDate.Year, task.StartDate.Month, task.StartDate.Day)
                                                                               && DateTime.Today <= new DateTime(task.EndDate.Year, task.EndDate.Month, task.EndDate.Day);
        /// <summary>
        /// Фильтр: Бессрочные задачи
        /// </summary>
        public readonly Func<PlannerModel.Task, bool> menuFilterTermlessTask = (task) => task.EndDate == DateTime.Parse("2099-01-01 00:00:00");
        /// <summary>
        /// Фильтр: Предстоящие задачи
        /// </summary>
        public readonly Func<PlannerModel.Task, bool> menuFilterFutureTask = (task) => DateTime.Now < task.StartDate;
        /// <summary>
        /// Фильтр: Завершенные задачи
        /// </summary>
        public readonly Func<PlannerModel.Task, bool> menuFilterFinishedTask = (task) => task.IsFinished;
        /// <summary>
        /// Фильтр: Просроченные задачи
        /// </summary>
        public readonly Func<PlannerModel.Task, bool> menuFilterOverdueTask = (task) => task.IsOverdue;
        /// <summary>
        /// Фильтр: Задачи срочного приоритета
        /// </summary>
        public readonly Func<PlannerModel.Task, bool> menuFilterImmediateTask = (task) => task.PriorityId == 3;
        #endregion

        public Filter()
        {
            menuFilterMain = menuFilterTodayTask;
        }
        /// <summary>
        /// Фильтрация задачи
        /// </summary>
        /// <param name="tasksCollection">Список задач</param>
        public IEnumerable<Task> FilterTasks(IEnumerable<Task> tasksCollection)
        {
            tasksCollection = tasksCollection.Where(menuFilterMain);

            tasksCollection = tasksCollection.Where(task => isNotFinishedFilter && !task.IsFinished
                                                            || isFinishedFilter && task.IsFinished
                                                            || isOverdueFilter && task.IsOverdue);
            if (priorityIdFilter > 0)
                tasksCollection = tasksCollection.Where(task => task.PriorityId == priorityIdFilter);
            if (categoryIdFilter > 0)
                tasksCollection = tasksCollection.Where(task => task.CategoryId == categoryIdFilter);
            if (startDate != default)
                tasksCollection = tasksCollection.Where(task => task.StartDate >= startDate);
            if (endDate != default)
                tasksCollection = tasksCollection.Where(task => task.EndDate <= endDate);
            if (searchString != default)
            {
                Regex regex = new Regex($"{searchString}", RegexOptions.IgnoreCase);
                tasksCollection = tasksCollection.Where(task => regex.IsMatch(task.Name));
            }

            return SortTasks(tasksCollection);
        }
        /// <summary>
        /// Сортировка задач
        /// </summary>
        /// <param name="tasksCollection"></param>
        public IEnumerable<Task> SortTasks(IEnumerable<Task> tasksCollection)
        {
            return tasksCollection = tasksCollection.OrderBy(task => task.IsFinished)
                .ThenByDescending(task => task.IsOverdue)
                .ThenByDescending(task => task.PriorityId)
                .ThenBy(task => task.Category.Name)
                .ThenBy(task => task.EndDate)
                .ThenBy(task => task.Name);
        }

        public void CheckFlags(bool isNotFinished, bool isFinished, bool isOverdue)
        {
            isNotFinishedFilter = isNotFinished;
            isFinishedFilter = isFinished;
            isOverdueFilter = isOverdue;
        }
    }
}
