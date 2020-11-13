using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerView.Helpers
{
    class TaskModel : PlannerModel.Task
    {
        public TimeSpan StartTimeSpan { get; set; }
        public TimeSpan EndTimeSpan { get; set; }

        public TaskModel()
        {
            StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StartTimeSpan = new TimeSpan(19, 0, 0);

            EndTimeSpan = new TimeSpan(19, 0, 0);
            EndTime = StartTime.AddDays(1);
        }

        public TaskModel(PlannerModel.Task task)
        {
            Id = task.Id;
            Name = task.Name;
            CreationDate = task.CreationDate;
            StartTime = task.StartTime;
            EndTime = task.EndTime;
            PriorityId = task.PriorityId;
            Priority = task.Priority;
            CategoryId = task.CategoryId;
            Category = task.Category;

            StartTimeSpan = new TimeSpan(StartTime.Hour, StartTime.Minute, StartTime.Second);
            StartTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day);

            EndTimeSpan = new TimeSpan(EndTime.Hour, EndTime.Minute, EndTime.Second);
            EndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day);
        }
    }
}