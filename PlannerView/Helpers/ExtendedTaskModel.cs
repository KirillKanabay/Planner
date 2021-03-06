﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerView.Helpers
{
    /// <summary>
    /// Класс помощник: Расширенная модель задачи
    /// </summary>
    class ExtendedTaskModel : PlannerModel.Task
    {
        /// <summary>
        /// Время начала задачи
        /// </summary>
        public TimeSpan StartTimeSpan { get; set; }
        /// <summary>
        /// Время окончания задачи
        /// </summary>
        public TimeSpan EndTimeSpan { get; set; }

        public ExtendedTaskModel()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StartTimeSpan = new TimeSpan(DateTime.Now.Hour, 0, 0).Add(new TimeSpan(0,1,0,0));

            EndTimeSpan = new TimeSpan(0,0,0,0).Add(StartTimeSpan);
            EndDate = StartDate.AddDays(1);
        }

        public ExtendedTaskModel(PlannerModel.Task task)
        {
            Id = task.Id;
            Name = task.Name;
            CreationDate = task.CreationDate;
            StartDate = task.StartDate;
            EndDate = task.EndDate;
            PriorityId = task.PriorityId;
            Priority = task.Priority;
            CategoryId = task.CategoryId;
            Category = task.Category;

            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StartTimeSpan = new TimeSpan(DateTime.Now.Hour, 0, 0).Add(new TimeSpan(0, 1, 0, 0));

            EndTimeSpan = new TimeSpan(0, 0, 0, 0).Add(StartTimeSpan);
            EndDate = StartDate.AddDays(1);
        }
    }
}