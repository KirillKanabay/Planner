using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EO.Internal;
using PlannerController;
using PlannerModel;
using PlannerView.Windows;
using Task = System.Threading.Tasks.Task;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskController _taskController;
        private CategoryController categoryController;
        private PriorityController priorityController;
        private CategoryEdit categoryEdit;
        
        static public TaskEdit taskEdit;

        public delegate void TaskHandler();
        public delegate void WrapHadler(object sender,RoutedEventArgs e);
        public delegate void SnackbarHadler(string message);
        
        public static event TaskHandler TaskListChanged;
        public static event WrapHadler CloseWrapEvent;
        public static event WrapHadler ShowWrapEvent;
        public static event SnackbarHadler SnackbarNotifyEvent;

        public Func<PlannerModel.Task, bool> MainFilter;

        private IEnumerable<PlannerModel.Task> _tasksCollection;
        public MainWindow()
        {
            InitializeComponent();
            MainFilter = (task) => DateTime.Now >= task.StartTime && DateTime.Now <= task.EndTime;
            
            TaskListChanged += RefreshTaskList;
            CloseWrapEvent += WrapBtn_OnClick;
            ShowWrapEvent += ShowWrapBtn;
            SnackbarNotifyEvent += SnackbarNotify;

            _taskController = new TaskController();
            categoryController = new CategoryController();
            priorityController = new PriorityController();

            //Получение списка приоритетов
            
            _categoriesListFilter.Add(new Category() { Name = "Все категории",Id = 0});
            foreach(var category in categoryController.Items)
                _categoriesListFilter.Add(category);

            _prioritiesListFilter.Add(new Priority() { Name = "Все приоритеты", Id = 0 });
            foreach (var priority in priorityController.Items)
            {
                _prioritiesListFilter.Add(priority);
            }


            PrioritiesBox.ItemsSource = _prioritiesListFilter.Select(item => item.Name);
            CategoriesBox.ItemsSource = _categoriesListFilter.Select(item => item.Name);
            //Получение списка категорий

            PrioritiesBox.SelectedIndex = 0;
            CategoriesBox.SelectedIndex = 0;

            DoRefresh();
        }

        public static void SendSnackbar(string message)
        {
            SnackbarNotifyEvent?.Invoke(message);
        }

        public static void ShowWrap(object sender, RoutedEventArgs e)
        {
            ShowWrapEvent?.Invoke(sender, e);
        }
        public static void CloseWrap(object sender, RoutedEventArgs e)
        {
            CloseWrapEvent?.Invoke(sender, e);
        }
        public static void DoRefresh()
        {
            TaskListChanged?.Invoke();
        }

        private void RefreshTaskList()
        {
            _taskController = new TaskController();
            TaskList.Children.RemoveRange(0,TaskList.Children.Count);
            _tasksCollection = _taskController.Tasks;
            Filter();
            foreach (var task in _tasksCollection)
            {
                //if(taskController.Tasks[i].IsFinished) continue;
                TaskItem taskItem = new TaskItem(_taskController,task,categoryController,priorityController);
                TaskList.Children.Add(taskItem);
            }
        }
        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowWrap(sender, e);
            taskEdit = new TaskEdit();
            taskEdit.ShowInTaskbar = false;
            taskEdit.IsOpen = true;
        }

        private void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            Wrap.Visibility = Visibility.Visible;
            
            categoryEdit = new CategoryEdit();
            categoryEdit.ShowInTaskbar = false;
            categoryEdit.IsOpen = true;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox) sender;
            var lbi = (ListBoxItem) item.SelectedItem;
            var stackPanel = (StackPanel) lbi?.Content;
            var label = (Label) stackPanel?.Children[1];
            
            if(label == null)
                return;
            TaskTitle.Content = label.Content.ToString();
            switch (label.Content.ToString())
            { 
                case "Бессрочные задачи":
                    MainFilter = (task) => task.EndTime == DateTime.Parse("2099-01-01 00:00:00");
                    break;
                case "Задачи на сегодня":
                    MainFilter = (task) => DateTime.Now >= task.StartTime && DateTime.Now <= task.EndTime;
                    break;
                case "Предстоящие задачи":
                    MainFilter = (task) => DateTime.Now < task.StartTime;
                    break;
                case "Выполненные задачи":
                    MainFilter = (task) => task.IsFinished;
                    break; 
                case "Просроченные задачи":
                    MainFilter = (task) => DateTime.Now > task.EndTime && !task.IsFinished;
                        break; 
                case "Задачи срочного приоритета":
                    MainFilter = (task) => task.PriorityId == 3;
                        break;
            }
            DoRefresh();
        }

        private void ShowWrapBtn(object sender, RoutedEventArgs e)
        {
            Wrap.Visibility = Visibility.Visible;
        }

        private void WrapBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (categoryEdit?.IsOpen ?? false)
            {
                categoryEdit.IsOpen = false;
            }

            if (taskEdit?.IsOpen ?? false)
            {
                taskEdit.IsOpen = false;
            }

            Wrap.Visibility = Visibility.Collapsed;
        }

        private void SnackbarNotify(string message)
        {
            var messageQueue = Snackbar.MessageQueue;
            Task.Factory.StartNew(() => messageQueue.Enqueue(message));
        }

        private void Search_OnKeyDown(object sender, KeyEventArgs e)
        {
            _searchString = SearchBox.Text.ToLower();
            DoRefresh();
        }
        #region Фильтрация

        private string _searchString;
        
        private ObservableCollection<Category> _categoriesListFilter = new ObservableCollection<Category>();
        private ObservableCollection<Priority> _prioritiesListFilter = new ObservableCollection<Priority>();
        private int _categoryIdFilter;
        private int _priorityIdFilter;

        private bool _isNotFinishedFilter = false;
        private bool _isFinishedFilter = false;
        private bool _isOverdueFilter = false;

        private DateTime? _startDate = null;
        private DateTime? _endDate = null;
       

        private void AcceptFilter_OnClick(object sender, RoutedEventArgs e)
        {
            FilterPopupBox.IsPopupOpen = false;

            _categoryIdFilter = _categoriesListFilter.FirstOrDefault(item => item.Name == CategoriesBox.Text).Id;
            _priorityIdFilter = _prioritiesListFilter.FirstOrDefault(item => item.Name == PrioritiesBox.Text).Id;

            _isNotFinishedFilter = NotFinishedCheckBox?.IsChecked ?? false;
            _isFinishedFilter = FinishedCheckBox?.IsChecked ?? false;
            _isOverdueFilter = OverdueCheckBox?.IsChecked ?? false;

            if (StartDate.Text != "")
            {
                DateTime.TryParse(StartDate.Text, out DateTime _startDate);
            }

            if (EndDate.Text != "")
            {
                DateTime.TryParse(EndDate.Text, out DateTime _endDate);
            }
            
            DoRefresh();
        }

        public void Filter()
        {
            _tasksCollection = _tasksCollection.Where(MainFilter);
            if (_isNotFinishedFilter)
                _tasksCollection = _tasksCollection.Where(task => !task.IsFinished);
            if (_isFinishedFilter)
                _tasksCollection = _tasksCollection.Where(task => task.IsFinished);
            if (_isOverdueFilter)
                _tasksCollection = _tasksCollection.Where(task => DateTime.Now > task.EndTime);
            if (_priorityIdFilter > 0)
                _tasksCollection = _tasksCollection.Where(task => task.PriorityId == _priorityIdFilter);
            if (_categoryIdFilter > 0)
                _tasksCollection = _tasksCollection.Where(task => task.CategoryId == _categoryIdFilter);
            if (_startDate != null)
                _tasksCollection = _tasksCollection.Where(task => task.StartTime >= _startDate);
            if (_endDate != null)
                _tasksCollection = _tasksCollection.Where(task => task.EndTime <= _endDate);
            if (_searchString != default)
            {
                Regex regex = new Regex($"{_searchString}", RegexOptions.IgnoreCase);
                _tasksCollection = _tasksCollection.Where(task => regex.IsMatch(task.Name));
            }
               
        }
        #endregion

    }
}
