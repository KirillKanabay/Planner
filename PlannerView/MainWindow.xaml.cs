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
        /// <summary>
        /// Контроллер задачи
        /// </summary>
        private TaskController _taskController;
        /// <summary>
        /// Контроллер категории
        /// </summary>
        private CategoryController _categoryController;
        /// <summary>
        /// Контроллер приоритета
        /// </summary>
        private PriorityController _priorityController;
        #endregion

        #region Окна
        /// <summary>
        /// Окно редактора задач
        /// </summary>
        public static TaskEdit _taskEdit;
        /// <summary>
        /// Окно статистики
        /// </summary>
        private Stats _statsWindow;
        /// <summary>
        /// Окно о программе
        /// </summary>
        private AboutProgram _aboutProgram;
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
        /// <summary>
        /// Коллекция задач
        /// </summary>
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
        /// <summary>
        /// Статический метод вызывающий событие вызова уведомления
        /// </summary>
        /// <param name="message"></param>
        public static void SendSnackbar(string message)
        {
            SnackbarNotifyEvent?.Invoke(message);
        }
        /// <summary>
        /// Статический метод вызывающий событие открытие обертки
        /// </summary>
        public static void ShowWrap(object sender, RoutedEventArgs e)
        {
            ShowWrapEvent?.Invoke(sender, e);
        }
        /// <summary>
        /// Статический метод вызывающий событие закрытие обертки
        /// </summary>
        public static void CloseWrap(object sender, RoutedEventArgs e)
        {
            CloseWrapEvent?.Invoke(sender, e);
        }
        /// <summary>
        /// Статический метод вызывающий событие обновления списка задач
        /// </summary>
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
        /// Открытие окна о программе
        /// </summary>
        private void ShowAboutProgramWindow()
        {
            Wrap.Visibility = Visibility.Visible;

            _aboutProgram = new AboutProgram();
            _aboutProgram.ShowInTaskbar = false;
            _aboutProgram.IsOpen = true;
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

            if (_aboutProgram?.IsOpen ?? false)
            {
                _aboutProgram.IsOpen = false;
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

                if (_aboutProgram?.IsOpen ?? false)
                {
                    _aboutProgram.Hide();
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

                if (_aboutProgram?.IsOpen ?? false)
                {
                    _aboutProgram.Show();
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
        /// <summary>
        /// Пункт меню: все задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 0;
            _filter.menuFilterMain = _filter.menuFilterAllTask;

            _filter.CheckFlags(true, false, false);

            title = "Все задачи";

            DoRefresh();
        }
        /// <summary>
        /// Пункт меню: бессрочные задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TermlessTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 1;
            _filter.menuFilterMain = _filter.menuFilterTermlessTask;

            _filter.CheckFlags(true, false, false);

            title = "Бессрочные задачи";

            DoRefresh();
        }
        /// <summary>
        /// Пункт меню: задачи на сегодня
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TodayTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 2;
            _filter.menuFilterMain = _filter.menuFilterTodayTask;

            _filter.CheckFlags(true, false, true);

            title = "Задачи на сегодня";

            DoRefresh();
        }
        /// <summary>
        /// Пункт меню: предстоящие задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FutureTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 3;
            _filter.menuFilterMain = _filter.menuFilterFutureTask;

            _filter.CheckFlags(true, false, false);

            title = "Предстоящие задачи";

            DoRefresh();
        }
        /// <summary>
        /// Пункт меню: выполненные задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishedTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 4;
            _filter.menuFilterMain = _filter.menuFilterFinishedTask;

            _filter.CheckFlags(false, true, false);

            title = "Выполненные задачи";

            DoRefresh();
        }
        /// <summary>
        /// Пункт меню: просроченные задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OverdueTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            _filter.menuFilterMain = _filter.menuFilterOverdueTask;
            Menu.SelectedIndex = 5;

            _filter.CheckFlags(true, false, true);

            title = "Просроченные задачи";

            DoRefresh();
        }
        /// <summary>
        /// Пункт меню: задачи срочные приоритета 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImmediateTaskMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            title = "Задачи cрочного приоритета";

            Menu.SelectedIndex = 6;

            _filter.menuFilterMain = _filter.menuFilterImmediateTask;
            _filter.CheckFlags(true, false, true);

            DoRefresh();
        }
        /// <summary>
        /// Пункт меню: статистика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatsMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowStatsWindow();
        }
        /// <summary>
        /// Пункт меню: диаграмма Ганта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GanttMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.SelectedIndex = 9;
            GridEmpty.Visibility = Visibility.Hidden;
            GridMain.Visibility = Visibility.Hidden;
            GridGantt.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Пункт меню: о программе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowAboutProgramWindow();
        }
        #endregion
    }
}
