using PlannerController;
using PlannerModel;
using PlannerView.Helpers;
using PlannerView.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Task = System.Threading.Tasks.Task;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия планировщика задач
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Поля и свойства

        #region Контроллеры
        private TaskController _taskController;
        private CategoryController _categoryController;
        private PriorityController _priorityController;
        #endregion

        #region Окна
        public static TaskEdit _taskEdit;
        private Stats _statsWindow;

        //Свойство заголовка планировщика задач
        private string title
        {
            get => title;
            set => TaskTitle.Content = value;
        }
        #endregion

        #region Делегаты
        /// <summary>
        /// Делегат обновления задачи
        /// </summary>
        public delegate void TaskHandler();
        /// <summary>
        /// Делегат фоновой обертки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void WrapHandler(object sender, RoutedEventArgs e);
        /// <summary>
        /// Делегат уведомления
        /// </summary>
        /// <param name="message"></param>
        public delegate void SnackbarHadler(string message);
        #endregion

        #region События
        /// <summary>
        /// Событие изменения списка задач
        /// </summary>
        public static event TaskHandler TaskListChanged;
        /// <summary>
        /// Событие закрытия фоновой обертки
        /// </summary>
        public static event WrapHandler CloseWrapEvent;
        /// <summary>
        /// Cобытие открытия фоновой обертки
        /// </summary>
        public static event WrapHandler ShowWrapEvent;
        /// <summary>
        /// Событие уведомления
        /// </summary>
        public static event SnackbarHadler SnackbarNotifyEvent;
        #endregion

        #region Фильтрация
        /// <summary>
        /// Фильтр
        /// </summary>
        private readonly Filter _filter;
        // private string _searchString;
        /// <summary>
        /// Список категорий для фильтра
        /// </summary>
        private readonly ObservableCollection<Category> _categoriesListFilter = new ObservableCollection<Category>();
        /// <summary>
        /// Список приоритетов для фильтра
        /// </summary>
        private readonly ObservableCollection<Priority> _prioritiesListFilter = new ObservableCollection<Priority>();

        #endregion

        private IEnumerable<PlannerModel.Task> _tasksCollection;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            //Определяем стандартный фильтр меню
            _filter = new Filter();
            //_menuFilterMain = _menuFilterTodayTask;

            //Подписка событий
            TaskListChanged += RefreshTaskList;
            CloseWrapEvent += WrapBtn_OnClick;
            ShowWrapEvent += ShowWrapBtn;
            SnackbarNotifyEvent += SnackbarNotify;

            //Инициализация контроллеров
            _taskController = new TaskController();
            _categoryController = new CategoryController();
            _priorityController = new PriorityController();

            //Инициализация списка задач
            _tasksCollection = _taskController.Tasks;

            //Определяем списки задач для фильтров категории и приоритета
            _categoriesListFilter.Add(new Category() { Name = "Все категории", Id = 0 });
            foreach (var category in _categoryController.Categories)
                _categoriesListFilter.Add(category);

            _prioritiesListFilter.Add(new Priority() { Name = "Все приоритеты", Id = 0 });
            foreach (var priority in _priorityController.Priorities)
                _prioritiesListFilter.Add(priority);

            //Заполнение списков в фильтре
            PrioritiesBox.ItemsSource = _prioritiesListFilter.Select(item => item.Name);
            CategoriesBox.ItemsSource = _categoriesListFilter.Select(item => item.Name);

            PrioritiesBox.SelectedIndex = 0;
            CategoriesBox.SelectedIndex = 0;

            // Обновление списка задач
            DoRefresh();
        }

        #region Методы взаимодействия с главным окном из других окон
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

        #endregion

        #region Вспомогательные методы
        /// <summary>
        /// Обновление списка задач
        /// </summary>
        private void RefreshTaskList()
        {
            GridMain.Visibility = Visibility.Visible;
            GridGantt.Visibility = Visibility.Hidden;

            _taskController = new TaskController();
            TaskList.Children.RemoveRange(0, TaskList.Children.Count);
            _tasksCollection = _filter.FilterTasks(_taskController.Tasks);

            if (!_tasksCollection.Any())
            {
                GridEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                _taskController.MarkOverdueTasks();
                GridEmpty.Visibility = Visibility.Hidden;
                foreach (var task in _tasksCollection)
                {
                    //if(taskController.Tasks[i].IsFinished) continue;
                    TaskItem taskItem = new TaskItem(task);
                    TaskList.Children.Add(taskItem);
                }
            }

        }

        /// <summary>
        /// Событие нажатия кнопки добавления задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowWrap(sender, e);
            _taskEdit = new TaskEdit();
            _taskEdit.ShowInTaskbar = false;
            _taskEdit.IsOpen = true;
        }
        /// <summary>
        /// Открытие окна статистики
        /// </summary>
        private void ShowStatsWindow()
        {
            Wrap.Visibility = Visibility.Visible;

            _statsWindow = new Stats();
            _statsWindow.ShowInTaskbar = false;
            _statsWindow.IsOpen = true;
        }
        /// <summary>
        /// Показ обертки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWrapBtn(object sender, RoutedEventArgs e)
        {
            Wrap.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Закрытие обертки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WrapBtn_OnClick(object sender, RoutedEventArgs e)
        {
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
        /// Взаимодействие вспомогательных и главной форм при сворачивании приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                if (_statsWindow?.IsOpen ?? false)
                {
                    _statsWindow.Hide();
                }

                if (_taskEdit?.IsOpen ?? false)
                {
                    _taskEdit.Hide();
                }
            }
            else
            {
                if (_statsWindow?.IsOpen ?? false)
                {
                    _statsWindow.Show();
                }

                if (_taskEdit?.IsOpen ?? false)
                {
                    _taskEdit.Show();
                }
            }
        }

        #endregion


        #region Фильтрация
        /// <summary>
        /// Обработчик поиска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_OnKeyDown(object sender, KeyEventArgs e)
        {
            _filter.searchString = SearchBox.Text.ToLower();
            DoRefresh();
        }

        /// <summary>
        /// Обработчик события установки фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptFilter_OnClick(object sender, RoutedEventArgs e)
        {
            FilterPopupBox.IsPopupOpen = false;

            _filter.categoryIdFilter = _categoriesListFilter.FirstOrDefault(item => item.Name == CategoriesBox.Text).Id;
            _filter.priorityIdFilter = _prioritiesListFilter.FirstOrDefault(item => item.Name == PrioritiesBox.Text).Id;

            _filter.isNotFinishedFilter = NotFinishedCheckBox?.IsChecked ?? false;
            _filter.isFinishedFilter = FinishedCheckBox?.IsChecked ?? false;
            _filter.isOverdueFilter = OverdueCheckBox?.IsChecked ?? false;

            if (StartDate.Text != "")
            {
                DateTime.TryParse(StartDate.Text, out _filter.startDate);
            }

            if (EndDate.Text != "")
            {
                DateTime.TryParse(EndDate.Text, out _filter.endDate);
            }

            DoRefresh();
        }
        #endregion

        
        #region Меню
        private void AllTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 0;
            _filter.menuFilterMain = _filter.menuFilterAllTask;

            _filter.CheckFlags(true, false, false);

            title = "Все задачи";

            DoRefresh();
        }

        private void TermlessTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 1;
            _filter.menuFilterMain = _filter.menuFilterTermlessTask;

            _filter.CheckFlags(true, false, false);

            title = "Бессрочные задачи";

            DoRefresh();
        }

        private void TodayTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 2;
            _filter.menuFilterMain = _filter.menuFilterTodayTask;

            _filter.CheckFlags(true, false, true);

            title = "Задачи на сегодня";

            DoRefresh();
        }

        private void FutureTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 3;
            _filter.menuFilterMain = _filter.menuFilterFutureTask;

            _filter.CheckFlags(true, false, false);

            title = "Предстоящие задачи";

            DoRefresh();
        }

        private void FinishedTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 4;
            _filter.menuFilterMain = _filter.menuFilterFinishedTask;

            _filter.CheckFlags(false, true, false);

            title = "Выполненные задачи";

            DoRefresh();
        }

        private void OverdueTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            _filter.menuFilterMain = _filter.menuFilterOverdueTask;
            Menu.SelectedIndex = 5;

            _filter.CheckFlags(true, false, true);

            title = "Просроченные задачи";

            DoRefresh();
        }

        private void ImmediateTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            title = "Задачи cрочного приоритета";

            Menu.SelectedIndex = 6;

            _filter.menuFilterMain = _filter.menuFilterImmediateTask;
            _filter.CheckFlags(true, false, true);

            DoRefresh();
        }

        private void StatsMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowStatsWindow();
        }

        private void GanttMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 9;
            GridEmpty.Visibility = Visibility.Hidden;
            GridMain.Visibility = Visibility.Hidden;
            GridGantt.Visibility = Visibility.Visible;
        }

        private void InfoMenuBtn_Click(object sender, RoutedEventArgs e)
        {
        }
        #endregion

    }
}
