using PlannerController;
using PlannerModel;
using PlannerView.Annotations;
using PlannerView.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using UtilityLibraries;

namespace PlannerView.Windows
{
    /// <summary>
    /// Логика взаимодействия для Редактора задач
    /// </summary>
    public partial class TaskEdit : Window, INotifyPropertyChanged
    {
        #region Поля и свойства
        /// <summary>
        /// Расширенная модель задачи
        /// </summary>
        private ExtendedTaskModel ExtendedTaskModel { get; set; }
        /// <summary>
        /// Контроллер категорий
        /// </summary>
        private CategoryController CategoryController { get; set; }
        /// <summary>
        /// Контроллер приоритета
        /// </summary>
        private PriorityController PriorityController { get; set; }
        /// <summary>
        /// Делегат обновления списка категорий
        /// </summary>
        public delegate void CategoryHandler();
        /// <summary>
        /// Событие обновления списка категорий
        /// </summary>
        public static event CategoryHandler RefreshCategoryListEvent;

        /// <summary>
        /// Флаг: является ли задача редактируемой
        /// </summary>
        private readonly bool _isEdit;
        /// <summary>
        /// Является ли окно открытым
        /// </summary>
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


        #endregion

        /// <summary>
        /// Конструктор для создания новой задачи
        /// </summary>
        public TaskEdit()
        {
            InitializeComponent();
            RefreshCategoryListEvent += RefreshCategoryList;
            //Привязка данных к модели задачи
            ExtendedTaskModel = new ExtendedTaskModel();
            DataContext = ExtendedTaskModel;

            //Получение списка приоритетов
            PriorityController = new PriorityController();
            PrioritiesBox.ItemsSource = PriorityController.Priorities.Select(item => item.Name);
            //Получение списка категорий
            RefreshCategoryList();

            PrioritiesBox.SelectedIndex = 0;
            CategoriesBox.SelectedIndex = 0;
        }
        /// <summary>
        /// Конструктор для редактирования существующей задачи
        /// </summary>
        /// <param name="task">Задача</param>
        public TaskEdit(Task task) : this()
        {
            _isEdit = true;
            ExtendedTaskModel = new ExtendedTaskModel(task);
            DataContext = ExtendedTaskModel;

            //Устанавливаем значения задачи в поля формы
            PrioritiesBox.SelectedIndex = PriorityController.Priorities.IndexOf(t => t.Id == ExtendedTaskModel.PriorityId);
            CategoriesBox.SelectedIndex = CategoryController.Categories.IndexOf(t => t.Id == ExtendedTaskModel.CategoryId);

            if (ExtendedTaskModel.EndDate == new DateTime(2099, 1, 1))
            {
                EndTimeToggle.IsChecked = false;
                EndTimeToggle_OnUnchecked(null, new RoutedEventArgs());
            }
        }
        /// <summary>
        /// Статический метод вызывающий событие обновления списка категорий
        /// </summary>
        public static void DoRefreshCategoryList()
        {
            RefreshCategoryListEvent?.Invoke();
        }
        /// <summary>
        /// Обновление списка категорий
        /// </summary>
        private void RefreshCategoryList()
        {
            CategoryController = new CategoryController();
            CategoriesBox.ItemsSource = CategoryController.Categories.Select(item => item.Name);
        }
        /// <summary>
        /// Включение поля ввода даты начала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTimeToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            StartDate.IsEnabled = true;
            StartTime.IsEnabled = true;
        }
        /// <summary>
        /// Отключение поля ввода даты начала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTimeToggle_OnUnchecked(object sender, RoutedEventArgs e)
        {
            StartDate.IsEnabled = false;
            StartTime.IsEnabled = false;
            ExtendedTaskModel.StartDate = DateTime.Now;
            ExtendedTaskModel.StartTimeSpan = new TimeSpan(0, 0, 0, 0);
        }
        /// <summary>
        /// Включение поля ввода даты окончания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndTimeToggle_OnChecked(object sender, RoutedEventArgs e)
        {
            EndDate.IsEnabled = true;
            EndTime.IsEnabled = true;
            if (ExtendedTaskModel != null)
            {
                ExtendedTaskModel.EndDate = ExtendedTaskModel.StartDate.AddDays(1);
                EndDate.Text = ExtendedTaskModel.EndDate.ToString("dd/MM/yyyy");
            }
        }
        /// <summary>
        /// Отключение поля ввода даты окончания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndTimeToggle_OnUnchecked(object sender, RoutedEventArgs e)
        {
            EndDate.IsEnabled = false;
            EndTime.IsEnabled = false;
            ExtendedTaskModel.EndDate = new DateTime(2099, 1, 1);
            ExtendedTaskModel.EndTimeSpan = new TimeSpan(0, 0, 0, 0);
        }
        /// <summary>
        /// Открытие формы редактора категорий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCategoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
            new CategoryEdit().ShowDialog();
            CategoriesBox.ItemsSource = CategoryController.Categories.Select(item => item.Name);
        }
        /// <summary>
        /// Сохранение задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = new Task()
                {
                    Id = ExtendedTaskModel.Id,
                    Name = ExtendedTaskModel.Name,
                    CreationDate = ExtendedTaskModel.CreationDate,
                    StartDate = ExtendedTaskModel.StartDate.Add(ExtendedTaskModel.StartTimeSpan),
                    EndDate = ExtendedTaskModel.EndDate.Add(ExtendedTaskModel.EndTimeSpan),
                    PriorityId = PrioritiesBox.SelectedIndex + 1,
                    Priority = ExtendedTaskModel.Priority,
                    CategoryId = CategoryController.GetCategoryByName(CategoriesBox.Text).Id,
                    Category = ExtendedTaskModel.Category,
                };

                var taskController = new TaskController(task, _isEdit);
                MainWindow.SendSnackbar($"Задача \"{ExtendedTaskModel.Name}\"" +
                                        ((!_isEdit) ? " добавлена в планировщик." : " отредактирована."));
                MainWindow.DoRefresh();
                Close();
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// При закрытии окна закрываем обертку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskEdit_OnClosed(object sender, EventArgs e)
        {
            MainWindow.CloseWrap(sender, new RoutedEventArgs());
        }
    }
}
