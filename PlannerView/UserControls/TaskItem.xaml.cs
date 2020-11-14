using System;
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
            var category = categoryController.GetCategoryById(Task.CategoryId);
           
            
            TaskName.Text = Task.Name;

            StartDate.Content = (Task.StartDate == DateTime.Parse("1980-01-01 00:00:00")) ? "-"
                : Task.StartDate.ToString("g");
            EndDate.Content = (Task.EndDate == DateTime.Parse("2099-01-01 00:00:00")) ? "Бессрочная"
                : Task.EndDate.ToString("g");

            if (Task.IsFinished)
            {
                TaskGrid.Opacity = 0.6;
                TaskGrid.Background = new SolidColorBrush(ColorExtensions.GetColor("#3015C651"));
                TaskName.TextDecorations = TextDecorations.Strikethrough;
                FinishTaskBtn.Content = "Завершена";
                FinishTaskBtn.Background = new SolidColorBrush(ColorExtensions.GetColor("#FF000000"));
                FinishTaskBtn.BorderBrush = new SolidColorBrush(ColorExtensions.GetColor("#FF000000"));
                FinishTaskBtn.IsEnabled = false;
            }

            if (Task.IsOverdue)
            {
                TaskGrid.Background = new SolidColorBrush(ColorExtensions.GetColor("#55FF6B6B"));
                TaskGrid.Background = new SolidColorBrush(ColorExtensions.GetColor("#55FF6B6B"));
                EndDate.Content = "Просрочена";
                BorderEndDate.Background = new SolidColorBrush(ColorExtensions.GetColor("#FFFF6B6B"));
            }

            PriorityBackground.Background = new SolidColorBrush(ColorExtensions.GetColor(priority.Color));
            
            CategoryBackground.Background = new SolidColorBrush(ColorExtensions.GetColor(category.Color));

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
