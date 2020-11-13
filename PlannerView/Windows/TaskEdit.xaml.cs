using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using PlannerController;
using PlannerModel;
using PlannerView.Annotations;
using PlannerView.Helpers;
using UtilityLibraries;

namespace PlannerView.Windows
{
    /// <summary>
    /// Логика взаимодействия для TaskEdit.xaml
    /// </summary>
    public partial class TaskEdit : Window, INotifyPropertyChanged
    {
        private TaskModel TaskModel { get; set; }
        private Task Task { get; set; }
        private CategoryController CategoryController { get; set; }
        private PriorityController PriorityController { get; set; }
        public delegate void CategoryHandler();
        public static event CategoryHandler RefreshCategoryListEvent ;

        private bool _isEdit;

        private bool _isOpen;
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                if (_isOpen)
                {
                    TaskNameTextBox.Focus();
                    Show();
                }
                else
                {
                    Close();
                }
            }
        }
        public TaskEdit()
        {
            InitializeComponent();
            RefreshCategoryListEvent += RefreshCategoryList;
            //Привязка данных к модели задачи
            TaskModel = new TaskModel();
            DataContext = TaskModel;

            //Получение списка приоритетов
            PriorityController = new PriorityController();
            PrioritiesBox.ItemsSource = PriorityController.Items.Select(item => item.Name);
            //Получение списка категорий
            RefreshCategoryList();
            
            PrioritiesBox.SelectedIndex = 0;
            CategoriesBox.SelectedIndex = 0;
        }

        public TaskEdit(Task task):this()
        {
            _isEdit = true;
            TaskModel = new TaskModel(task);
            DataContext = TaskModel;

            PrioritiesBox.SelectedIndex = PriorityController.Items.IndexOf(t => t.Id == TaskModel.PriorityId);
            CategoriesBox.SelectedIndex = CategoryController.Items.IndexOf(t => t.Id == TaskModel.CategoryId);

            if (TaskModel.EndTime == new DateTime(2099, 1, 1))
            {
                EndTimeToggle.IsChecked = false;
                EndTimeToggle_OnUnchecked(null, new RoutedEventArgs());
            }
        }
        public static void DoRefreshCategoryList()
        {
            RefreshCategoryListEvent?.Invoke();
        }

        private void RefreshCategoryList()
        {
            CategoryController = new CategoryController();
            CategoriesBox.ItemsSource = CategoryController.Items.Select(item => item.Name);
        }
        private void StartTimeToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            StartDate.IsEnabled = true;
            StartTime.IsEnabled = true;
        }

        private void StartTimeToggle_OnUnchecked(object sender, RoutedEventArgs e)
        {
            StartDate.IsEnabled = false;
            StartTime.IsEnabled = false;
            TaskModel.StartTime = DateTime.Now;
            TaskModel.StartTimeSpan = new TimeSpan(0,0,0,0);
        }

        private void EndTimeToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            EndDate.IsEnabled = true;
            EndTime.IsEnabled = true;
            if (TaskModel != null)
            {
                TaskModel.EndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
                EndDate.Text = TaskModel.EndTime.ToString("dd/MM/yyyy");
            }
        }

        private void EndTimeToggle_OnUnchecked(object sender, RoutedEventArgs e)
        {
            EndDate.IsEnabled = false;
            EndTime.IsEnabled = false;
            TaskModel.EndTime = new DateTime(2099, 1, 1);
            TaskModel.EndTimeSpan = new TimeSpan(0, 0, 0, 0);
        }
        private void AddCategoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new CategoryEdit().ShowDialog();
            CategoriesBox.ItemsSource = CategoryController.Items.Select(item => item.Name);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = new Task()
                {
                    Id = TaskModel.Id,
                    Name = TaskModel.Name,
                    CreationDate = TaskModel.CreationDate,
                    StartTime = TaskModel.StartTime.Add(TaskModel.StartTimeSpan),
                    EndTime = TaskModel.EndTime.Add(TaskModel.EndTimeSpan),
                    PriorityId = PrioritiesBox.SelectedIndex + 1,
                    Priority = TaskModel.Priority,
                    CategoryId = CategoriesBox.SelectedIndex + 1,
                    Category = TaskModel.Category,
                };
                
                var taskController = new TaskController(task, _isEdit);
                MainWindow.SendSnackbar($"Задача \"{TaskModel.Name}\"" +   
                                        ((!_isEdit) ? " добавлена в планировщик.":" отредактирована."));
                MainWindow.DoRefresh();
                Close();
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TaskEdit_OnClosed(object sender, EventArgs e)
        {
            MainWindow.CloseWrap(sender, new RoutedEventArgs());
        }
    }
}
