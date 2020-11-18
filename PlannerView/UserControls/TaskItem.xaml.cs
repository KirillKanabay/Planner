using PlannerController;
using PlannerModel;
using PlannerView.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UtilityLibraries;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для Контроллера элемента списка задач
    /// </summary>
    public partial class TaskItem : UserControl
    {
        #region Поля и свойства
        /// <summary>
        /// Контроллер задач
        /// </summary>
        private readonly TaskController _taskController;
        /// <summary>
        /// Задача
        /// </summary>
        private readonly PlannerModel.Task _task;
        /// <summary>
        /// Категория задачи
        /// </summary>
        private readonly Category _category;
        /// <summary>
        /// Приоритет задачи
        /// </summary>
        private readonly Priority _priority;
        #endregion

        public TaskItem()
        {
            DataContext = this;
        }
        public TaskItem(Task task) : this()
        {
            InitializeComponent();
            _taskController = new TaskController();
            _task = task;
            _priority = new PriorityController().GetPriority(_task.PriorityId);
            _category = new CategoryController().GetCategoryById(_task.CategoryId);


            TaskName.Text = _task.Name;

            StartDate.Content = (_task.StartDate == DateTime.Parse("1980-01-01 00:00:00")) ? "-"
                : _task.StartDate.ToString("g");
            EndDate.Content = (_task.EndDate == DateTime.Parse("2099-01-01 00:00:00")) ? "Бессрочная"
                : _task.EndDate.ToString("g");

            if (_task.IsFinished)
            {
                TaskGrid.Opacity = 0.6;
                TaskGrid.Background = new SolidColorBrush(ColorExtensions.GetColorFromString("#3015C651"));
                TaskName.TextDecorations = TextDecorations.Strikethrough;
                FinishTaskBtn.Content = "Завершена";
                FinishTaskBtn.Background = new SolidColorBrush(ColorExtensions.GetColorFromString("#FF000000"));
                FinishTaskBtn.BorderBrush = new SolidColorBrush(ColorExtensions.GetColorFromString("#FF000000"));
                FinishTaskBtn.IsEnabled = false;
            }

            if (_task.IsOverdue)
            {
                TaskGrid.Background = new SolidColorBrush(ColorExtensions.GetColorFromString("#55FF6B6B"));
                TaskGrid.Background = new SolidColorBrush(ColorExtensions.GetColorFromString("#55FF6B6B"));
                EndDate.Content = "Просрочена";
                BorderEndDate.Background = new SolidColorBrush(ColorExtensions.GetColorFromString("#FFFF6B6B"));
            }

            PriorityBackground.Background = new SolidColorBrush(ColorExtensions.GetColorFromString(_priority.Color));

            CategoryBackground.Background = new SolidColorBrush(ColorExtensions.GetColorFromString(_category.Color));

            Priority.Content = _priority.Name;
            Category.Text = _category.Name;

        }
        /// <summary>
        /// Завершение задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishTaskButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _taskController.FinishTask(_task.Id);
            MainWindow.DoRefresh();
            MainWindow.SendSnackbar($"Задача \"{_task.Name}\" завершена.");
        }
        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTaskBtn_OnClick(object sender, RoutedEventArgs e)
        {
            PopupBox.IsPopupOpen = false;
            _taskController.DeleteTask(_task.Id);
            MainWindow.DoRefresh();
            MainWindow.SendSnackbar($"Задача \"{_task.Name}\" удалена.");
        }
        /// <summary>
        /// Редактирование задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditTaskBtn_OnClick(object sender, RoutedEventArgs e)
        {
            PopupBox.IsPopupOpen = false;
            MainWindow.ShowWrap(sender, new RoutedEventArgs());
            var taskEdit = new TaskEdit(_task);
            taskEdit.ShowInTaskbar = false;
            taskEdit.IsOpen = true;
        }
    }
}
