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

namespace PlannerView.Windows
{
    /// <summary>
    /// Логика взаимодействия для TaskEdit.xaml
    /// </summary>
    public partial class TaskEdit : Window, INotifyPropertyChanged
    {
        private TaskModel TaskModel { get; set; }
        private CategoryController CategoryController { get; set; }
        private PriorityController PriorityController { get; set; }
        public delegate void CategoryHadler();
        public static event CategoryHadler RefreshCategoryListEvent ;

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
            PrioritiesBox.ItemsSource = PriorityController.Items.Select(item => item.Name).OrderBy(item => item);
            //Получение списка категорий
            RefreshCategoryList();
            
            PrioritiesBox.SelectedIndex = 0;
            CategoriesBox.SelectedIndex = 0;
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
        ObservableCollection<String> GetPrioritiesName(PriorityController controller)
        {
            var priorities = new ObservableCollection<string>();
            foreach (Priority priority in controller.Items)
            {
                priorities.Add(priority.Name);
            }

            return priorities;
        }

        ObservableCollection<String> GetCategoriesName(CategoryController controller)
        {
            var categories = new ObservableCollection<string>();
            foreach (Category category in controller.Items)
            {
                categories.Add(category.Name);
            }

            return categories;
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
        }

        private void EndTimeToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            EndDate.IsEnabled = true;
            EndTime.IsEnabled = true;
        }

        private void EndTimeToggle_OnUnchecked(object sender, RoutedEventArgs e)
        {
            EndDate.IsEnabled = false;
            EndTime.IsEnabled = false;
            TaskModel.StartTime = new DateTime(2099, 1, 1);
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
                var priorityId = PrioritiesBox.SelectedIndex + 1;
                var categoryId = CategoriesBox.SelectedIndex + 1;

                TaskModel.StartTime = TaskModel.StartTime.Add(TaskModel.StartTimeSpan);
                TaskModel.EndTime = TaskModel.EndTime.Add(TaskModel.EndTimeSpan);

                var taskController = new TaskController(TaskModel.Name,
                    TaskModel.StartTime,
                    TaskModel.EndTime,
                    priorityId,
                    categoryId);

                MessageBox.Show("Задача добавлена в планировщик", "Редактор задач", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                MainWindow.DoRefresh(taskController);
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
