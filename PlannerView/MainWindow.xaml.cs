using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        /// <summary>
        /// Главный фильтр меню
        /// </summary>
        private Func<PlannerModel.Task, bool> _menuFilterMain;
        /// <summary>
        /// Фильтр: все задачи
        /// </summary>
        private readonly Func<PlannerModel.Task, bool> _menuFilterAllTask = (task) => true;
        /// <summary>
        /// Фильтр: Задачи на сегодня
        /// </summary>
        private readonly Func<PlannerModel.Task, bool> _menuFilterTodayTask = (task) => DateTime.Today >= new DateTime(task.StartDate.Year, task.StartDate.Month, task.StartDate.Day)
                                                                               && DateTime.Today <= new DateTime(task.EndDate.Year, task.EndDate.Month, task.EndDate.Day);
        /// <summary>
        /// Фильтр: Бессрочные задачи
        /// </summary>
        private readonly Func<PlannerModel.Task, bool> _menuFilterTermlessTask = (task) => task.EndDate == DateTime.Parse("2099-01-01 00:00:00");
        /// <summary>
        /// Фильтр: Предстоящие задачи
        /// </summary>
        private readonly Func<PlannerModel.Task, bool> _menuFilterFutureTask = (task) => DateTime.Now < task.StartDate;
        /// <summary>
        /// Фильтр: Завершенные задачи
        /// </summary>
        private readonly Func<PlannerModel.Task, bool> _menuFilterFinishedTask = (task) => task.IsFinished;
        /// <summary>
        /// Фильтр: Просроченные задачи
        /// </summary>
        private readonly Func<PlannerModel.Task, bool> _menuFilterOverdueTask = (task) => task.IsOverdue;
        /// <summary>
        /// Фильтр: Задачи срочного приоритета
        /// </summary>
        private readonly Func<PlannerModel.Task, bool> _menuFilterImmediateTask = (task) => task.PriorityId == 3;

        #endregion

        #region Трей
        private WindowState _prevState;
        #endregion

        #region Уведомления

        #endregion

        private IEnumerable<PlannerModel.Task> _tasksCollection;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            //Определяем стандартный фильтр меню
            _menuFilterMain = _menuFilterTodayTask;

            TaskListChanged += RefreshTaskList;
            CloseWrapEvent += WrapBtn_OnClick;
            ShowWrapEvent += ShowWrapBtn;
            SnackbarNotifyEvent += SnackbarNotify;

            _taskController = new TaskController();
            _categoryController = new CategoryController();
            _priorityController = new PriorityController();

            _tasksCollection = _taskController.Tasks;

            CountTaskForMenu();
            //Получение списка приоритетов
            
            _categoriesListFilter.Add(new Category() { Name = "Все категории",Id = 0});
            foreach(var category in _categoryController.Items)
                _categoriesListFilter.Add(category);

            _prioritiesListFilter.Add(new Priority() { Name = "Все приоритеты", Id = 0 });
            foreach (var priority in _priorityController.Items)
                _prioritiesListFilter.Add(priority);


            PrioritiesBox.ItemsSource = _prioritiesListFilter.Select(item => item.Name);
            CategoriesBox.ItemsSource = _categoriesListFilter.Select(item => item.Name);
            //Получение списка категорий

            PrioritiesBox.SelectedIndex = 0;
            CategoriesBox.SelectedIndex = 0;

            // устанавливаем метод обратного вызова
            DoRefresh();
        }

        private void CountTaskForMenu()
        {
            AllTaskMenuCount.Content = $"({_tasksCollection.Count(task => !task.IsFinished)})";
            TermlessTaskMenuCount.Content = $"({_tasksCollection.Where(task => !task.IsFinished).Count(_menuFilterTermlessTask)})";
            TodayTaskMenuCount.Content = $"({_tasksCollection.Where(task => !task.IsFinished).Count(_menuFilterTodayTask)})";
            FutureTaskMenuCount.Content = $"({_tasksCollection.Where(task => !task.IsFinished).Count(_menuFilterFutureTask)})";
            FinishedTaskMenuCount.Content = $"({_tasksCollection.Count(_menuFilterFinishedTask)})";
            OverdueTaskMenuCount.Content = $"({_tasksCollection.Where(task => !task.IsFinished).Count(_menuFilterOverdueTask)})";
            ImmediateTaskMenuCount.Content = $"({_tasksCollection.Where(task => !task.IsFinished).Count(_menuFilterImmediateTask)})";
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

        private void MarkOverdueTasks()
        {
            foreach (var task in _tasksCollection)
            {
                if (task.EndDate < DateTime.Now && !task.IsFinished)
                {
                    _taskController.OverdueTask(task.Id);
                }
                else if(task.EndDate > DateTime.Now && !task.IsFinished && task.IsOverdue)
                {
                    _taskController.UnoverdueTask(task.Id);
                }
            }
        }

        private void RefreshTaskList()
        {
            _taskController = new TaskController();
            TaskList.Children.RemoveRange(0,TaskList.Children.Count);
            _tasksCollection = _taskController.Tasks;
            CountTaskForMenu();
            Filter();
            if (!_tasksCollection.Any())
            {
                GridEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                MarkOverdueTasks();
                GridEmpty.Visibility = Visibility.Hidden;
                foreach (var task in _tasksCollection)
                {
                    //if(taskController.Tasks[i].IsFinished) continue;
                    TaskItem taskItem = new TaskItem(task);
                    TaskList.Children.Add(taskItem);
                }
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
            ShowGridMain();

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
                    _menuFilterMain = _menuFilterAllTask;
                    break;
                case "Бессрочные задачи":
                    _menuFilterMain = _menuFilterTermlessTask;
                    break;
                case "Задачи на сегодня":
                    _menuFilterMain = _menuFilterTodayTask;
                    break;
                case "Предстоящие задачи":
                    _menuFilterMain = _menuFilterFutureTask;
                    break;
                case "Выполненные задачи":
                    _menuFilterMain = _menuFilterFinishedTask;
                    FinishedCheckBox.IsChecked = true;
                    break; 
                case "Просроченные задачи":
                    _menuFilterMain = _menuFilterOverdueTask;
                        break; 
                case "Задачи срочного приоритета":
                    _menuFilterMain = _menuFilterImmediateTask;
                        break;
                case "Статистика":
                    Menu.SelectedIndex = 0;
                    ShowStatsWindow();
                    break;
                case "Диаграмма Ганта":
                    GridMain.Visibility = Visibility.Hidden;
                    GridGantt.Visibility = Visibility.Visible;
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
        /// <summary>
        /// Уведомление о событиях в приложении
        /// </summary>
        /// <param name="message"></param>
        private void SnackbarNotify(string message)
        {
            var messageQueue = Snackbar.MessageQueue;
            Task.Factory.StartNew(() => messageQueue.Enqueue(message));
        }
        /// <summary>
        /// Обработчик поиска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_OnKeyDown(object sender, KeyEventArgs e)
        {
            _searchString = SearchBox.Text.ToLower();
            DoRefresh();
        }
        #region Фильтрация
        /// <summary>
        /// Обработчик события установки фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void Filter()
        {
            _tasksCollection = _tasksCollection.Where(_menuFilterMain);
            
            _tasksCollection = _tasksCollection.Where(task => _isNotFinishedFilter && !task.IsFinished
                                                            || _isFinishedFilter && task.IsFinished
                                                            || _isOverdueFilter && task.IsOverdue);
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

            Sort();
        }
        #endregion

        private void Sort()
        {
            _tasksCollection = _tasksCollection.OrderBy(task => task.IsFinished)
                                                .ThenByDescending(task => task.IsOverdue)
                                                .ThenByDescending(task => task.PriorityId)
                                                .ThenBy(task => task.Category.Name)
                                                .ThenBy(task => task.EndDate)
                                                .ThenBy(task => task.Name);
        }

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


        private void ShowGridMain()
        {
            if(GridMain != null)
                GridMain.Visibility = Visibility.Visible;
            if(GridGantt != null)
                GridGantt.Visibility = Visibility.Hidden;
        }
    }
}
