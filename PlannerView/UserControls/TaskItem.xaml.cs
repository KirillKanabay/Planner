﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PlannerController;
using PlannerModel;
using PlannerView.Windows;
using UtilityLibraries;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для TaskItem.xaml
    /// </summary>
    public partial class TaskItem : UserControl
    {
        private readonly TaskController TaskController;
        private readonly PlannerModel.Task Task;

        public TaskItem()
        {
            
        }
        public TaskItem(TaskController taskController,Task task, CategoryController categoryController, PriorityController priorityController)
        {
            InitializeComponent();
            TaskController = taskController;
            Task = task;
            var priority = priorityController.GetPriority(Task.PriorityId);
            var category = categoryController.GetCategory(Task.CategoryId);
           
            
            TaskName.Text = Task.Name;
            if (Task.IsFinished)
            {
                TaskGrid.Opacity = 0.6;
                TaskName.TextDecorations = TextDecorations.Strikethrough;
                FinishTaskBtn.Content = "Завершена";
                FinishTaskBtn.Background = new SolidColorBrush(ColorLibrary.GetColor("#FF000000"));
                FinishTaskBtn.BorderBrush = new SolidColorBrush(ColorLibrary.GetColor("#FF000000"));
                FinishTaskBtn.IsEnabled = false;
            }
            StartDate.Content = (Task.StartTime == DateTime.Parse("1980-01-01 00:00:00")) ? "-" 
                : Task.StartTime.ToString("g");
            EndDate.Content = (Task.EndTime == DateTime.Parse("2099-01-01 00:00:00")) ? "Бессрочная"
                : Task.EndTime.ToString("g");

            PriorityBackground.Background = new SolidColorBrush(ColorLibrary.GetColor(priority.Color));
            
            CategoryBackground.Background = new SolidColorBrush(ColorLibrary.GetColor(category.Color));

            Priority.Content = priority.Name;
            Category.Content = category.Name;

        }


        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TaskController.FinishTask(Task.Id);
            MainWindow.DoRefresh();
        }

        private void DeleteTaskBtn_OnClick(object sender, RoutedEventArgs e)
        {
            PopupBox.IsPopupOpen = false;
            TaskController.DeleteTask(Task.Id);
            MainWindow.DoRefresh();
        }

        private void EditTaskBtn_OnClick(object sender, RoutedEventArgs e)
        {
            PopupBox.IsPopupOpen = false;
            MainWindow.ShowWrap(sender, new RoutedEventArgs());
            var taskEdit = new TaskEdit(Task);
            taskEdit.ShowInTaskbar = false;
            taskEdit.IsOpen = true;
        }
    }
}
