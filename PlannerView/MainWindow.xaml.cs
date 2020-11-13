using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EO.Internal;
using MaterialDesignThemes.Wpf;
using Notifications.Wpf;
using PlannerController;
using PlannerModel;
using PlannerView.Helpers;
using PlannerView.Windows;
using Task = System.Threading.Tasks.Task;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Свойства

        #region Контроллеры
        private TaskController _taskController;
        private CategoryController _categoryController;
        private PriorityController _priorityController;
        #endregion

        #region Окна
        private CategoryEdit _categoryEdit;
        public static TaskEdit _taskEdit;
        private Stats _statsWindow;
        #endregion

        #region Делегаты
        public delegate void TaskHandler();
        public delegate void WrapHandler(object sender, RoutedEventArgs e);
        public delegate void SnackbarHadler(string message);
        #endregion

        #region События
        public static event TaskHandler TaskListChanged;
        public static event WrapHandler CloseWrapEvent;
        public static event WrapHandler ShowWrapEvent;
        public static event SnackbarHadler SnackbarNotifyEvent;
        #endregion

        #region Фильтрация
        private string _searchString;

        private ObservableCollection<Category> _categoriesListFilter = new ObservableCollection<Category>();
        private ObservableCollection<Priority> _prioritiesListFilter = new ObservableCollection<Priority>();
        private int _categoryIdFilter;
        private int _priorityIdFilter;

        private bool _isNotFinishedFilter = true;
        private bool _isFinishedFilter = false;
        private bool _isOverdueFilter = false;

        private DateTime _startDate = default;
        private DateTime _endDate = default;

        public Func<PlannerModel.Task, bool> MenuFilter;
        #endregion

        #region Трей
        private WindowState _prevState;
        #endregion

        #region Уведомления
        private NotificationManager _notificationManager;


        #endregion

        private IEnumerable<PlannerModel.Task> _tasksCollection;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            MenuFilter = (task) => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) >= new DateTime(task.StartDate.Year, task.StartDate.Month, task.StartDate.Day) 
                                   && new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) <= new DateTime(task.EndDate.Year, task.EndDate.Month, task.EndDate.Day);

            TaskListChanged += RefreshTaskList;
            CloseWrapEvent += WrapBtn_OnClick;
            ShowWrapEvent += ShowWrapBtn;
            SnackbarNotifyEvent += SnackbarNotify;

            _taskController = new TaskController();
            _categoryController = new CategoryController();
            _priorityController = new PriorityController();

            //Получение списка приоритетов
            
            _categoriesListFilter.Add(new Category() { Name = "Все категории",Id = 0});
            foreach(var category in _categoryController.Items)
                _categoriesListFilter.Add(category);

            _prioritiesListFilter.Add(new Priority() { Name = "Все приоритеты", Id = 0 });
            foreach (var priority in _priorityController.Items)
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
                TaskItem taskItem = new TaskItem(_taskController,task,_categoryController,_priorityController);
                TaskList.Children.Add(taskItem);
            }
        }
        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowWrap(sender, e);
            _taskEdit = new TaskEdit();
            _taskEdit.ShowInTaskbar = false;
            _taskEdit.IsOpen = true;
        }

        private void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            Wrap.Visibility = Visibility.Visible;
            
            _categoryEdit = new CategoryEdit();
            _categoryEdit.ShowInTaskbar = false;
            _categoryEdit.IsOpen = true;
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
                case "Все задачи":
                    MenuFilter = (task) => true;
                    break;
                case "Бессрочные задачи":
                    MenuFilter = (task) => task.EndDate == DateTime.Parse("2099-01-01 00:00:00");
                    break;
                case "Задачи на сегодня":
                    MenuFilter = (task) => DateTime.Now >= new DateTime(task.StartDate.Year, task.StartDate.Month, task.StartDate.Day)
                                           && DateTime.Now <= new DateTime(task.EndDate.Year, task.EndDate.Month, task.EndDate.Day);
                    break;
                case "Предстоящие задачи":
                    MenuFilter = (task) => DateTime.Now < task.StartDate;
                    break;
                case "Выполненные задачи":
                    MenuFilter = (task) => task.IsFinished;
                    break; 
                case "Просроченные задачи":
                    MenuFilter = (task) => DateTime.Now > task.EndDate && !task.IsFinished;
                        break; 
                case "Задачи срочного приоритета":
                    MenuFilter = (task) => task.PriorityId == 3;
                        break;
                case "Статистика":
                    Menu.SelectedIndex = 0;
                    ShowStatsWindow();
                    break;
            }
            DoRefresh();
        }

        private void ShowStatsWindow()
        {
            Wrap.Visibility = Visibility.Visible;

            _statsWindow = new Stats();
            _statsWindow.ShowInTaskbar = false;
            _statsWindow.IsOpen = true;
        }

        private void ShowWrapBtn(object sender, RoutedEventArgs e)
        {
            Wrap.Visibility = Visibility.Visible;
        }

        private void WrapBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_categoryEdit?.IsOpen ?? false)
            {
                _categoryEdit.IsOpen = false;
            }

            if (_taskEdit?.IsOpen ?? false)
            {
                _taskEdit.IsOpen = false;
            }

            if (_statsWindow?.IsOpen ?? false)
            {
                _statsWindow.IsOpen = false;
            }
            Wrap.Visibility = Visibility.Collapsed;
        }

        private void SnackbarNotify(string message)
        {
            //TODO: Перенести в отдельную функцию
            _notificationManager = new NotificationManager();
            _notificationManager.Show(new NotificationContent
            {
                Title = "Задача добавлена",
                Message = $"{message}",
                Type = NotificationType.Information
            });

            var messageQueue = Snackbar.MessageQueue;
            Task.Factory.StartNew(() => messageQueue.Enqueue(message));
        }

        private void Search_OnKeyDown(object sender, KeyEventArgs e)
        {
            _searchString = SearchBox.Text.ToLower();
            DoRefresh();
        }
        #region Фильтрация

        
       

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
                DateTime.TryParse(StartDate.Text, out _startDate);
            }

            if (EndDate.Text != "")
            {
                DateTime.TryParse(EndDate.Text, out _endDate);
            }
            
            DoRefresh();
        }

        public void Filter()
        {
            _tasksCollection = _tasksCollection.Where(MenuFilter);
            if (_isNotFinishedFilter)
                _tasksCollection = _tasksCollection.Where(task => !task.IsFinished);
            if (_isFinishedFilter)
                _tasksCollection = _tasksCollection.Where(task => task.IsFinished);
            if (_isOverdueFilter)
                _tasksCollection = _tasksCollection.Where(task => DateTime.Now > task.EndDate);
            if (_priorityIdFilter > 0)
                _tasksCollection = _tasksCollection.Where(task => task.PriorityId == _priorityIdFilter);
            if (_categoryIdFilter > 0)
                _tasksCollection = _tasksCollection.Where(task => task.CategoryId == _categoryIdFilter);
            if (_startDate != default)
                _tasksCollection = _tasksCollection.Where(task => task.StartDate >= _startDate);
            if (_endDate != default)
                _tasksCollection = _tasksCollection.Where(task => task.EndDate <= _endDate);
            if (_searchString != default)
            {
                Regex regex = new Regex($"{_searchString}", RegexOptions.IgnoreCase);
                _tasksCollection = _tasksCollection.Where(task => regex.IsMatch(task.Name));
            }
               
        }
        #endregion

        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
            else
                _prevState = WindowState;
        }

        private void NotifyIcon_OnTrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = _prevState;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _notificationManager?.Dispose();
        }
    }
}
