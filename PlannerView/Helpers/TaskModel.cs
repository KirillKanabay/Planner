﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerView.Helpers
{
    class TaskModel: PlannerModel.Task
    {
        public TimeSpan StartTimeSpan { get; set; }
        public TimeSpan EndTimeSpan { get; set; }

        public TaskModel()
        {
            StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            StartTimeSpan = new TimeSpan(19, 0, 0);
            
            EndTimeSpan = new TimeSpan(19,0,0);
            EndTime = StartTime.AddDays(1);
        }
    }
}
