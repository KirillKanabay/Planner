using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PlannerController;
using PlannerModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PlannerView.UserControls
{
    /// <summary>
    /// Логика взаимодействия для контроллера диаграммы Ганта
    /// </summary>
    public partial class GanttUserControl : UserControl
    {
        #region Поля и свойства
        /// <summary>
        /// Начало значения оси X
        /// </summary>
        private double _from;
        /// <summary>
        /// Конец значения оси X
        /// </summary>
        private double _to;
        /// <summary>
        /// Значения графика
        /// </summary>
        private ChartValues<GanttPoint> _values;

        /// <summary>
        /// Список категорий
        /// </summary>
        private IEnumerable<Category> _categoryList;

        /// <summary>
        /// Список задач
        /// </summary>
        private IEnumerable<PlannerModel.Task> _tasksCollection;
        /// <summary>
        /// Надписи элементов графика
        /// </summary>
        public Func<ChartPoint, string> PointLabel { get; set; }

        /// <summary>
        /// Свойства заголовка графика
        /// </summary>
        private string title
        {
            get => title;
            set => Title.Content = value;
        }
        //Элементы графика
        public SeriesCollection Series { get; set; }
        //Форматирование вывода элемента графика
        public Func<double, string> Formatter { get; set; }
        //Заголовки элементов задачи
        public string[] Labels { get; set; }

        public double From
        {
            get { return _from; }
            set
            {
                _from = value;
                OnPropertyChanged("From");
            }
        }

        public double To
        {
            get { return _to; }
            set
            {
                _to = value;
                OnPropertyChanged("To");
            }
        }
        #endregion

        public GanttUserControl()
        {
            InitializeComponent();

            //Определение списка категорий
            _categoryList = new CategoryController().Categories.Skip(1);

            foreach (var category in _categoryList)
            {
                CategoriesBox.Items.Add(category.Name);
            }
            //По умолчанию скрываем таблицу
            Gantt.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Получение задач по выбранной категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptCategoryBtn(object sender, RoutedEventArgs e)
        {
            var taskController = new TaskController();
            //Определение списка задач
            _tasksCollection = taskController.Tasks.Where(task => task.Category.Name == CategoriesBox.Text &&
                                                                  !task.IsFinished && task.EndDate != DateTime.Parse("2099-01-01 00:00:00"));
            //Скрываем вспомогательную картинку
            HelpImage.Visibility = Visibility.Hidden;
            InitGantt();
        }
        /// <summary>
        /// Инициализация диаграммы Ганта
        /// </summary>
        private void InitGantt()
        {
            PointLabel = chartPoint =>
                $"Начало: {new DateTime((long)chartPoint.XStart).ToString("dd MMM")} " +
                $",конец: {new DateTime((long)chartPoint.X).ToString("dd MMM")}";

            Formatter = value => new DateTime((long)value).ToString("dd MMM");

            //При отсутствии бессрочных ошибок показываем ошибку
            if (_tasksCollection.Any())
            {
                Gantt.Visibility = Visibility.Visible;
                GetGantt(_tasksCollection);
                title = $"Диаграмма Ганта для: \"{CategoriesBox.Text}\"";
            }
            else
            {
                Gantt.Visibility = Visibility.Collapsed;
                HelpImage.Visibility = Visibility.Visible;
                title = "Диаграмма Ганта";
                MessageBox.Show("В данной категории отсутствуют задачи, удовлетворяющие для построения графика Ганта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Получение диаграммы Ганта по списку задач
        private void GetGantt(IEnumerable<PlannerModel.Task> tasksCollection)
        {
            _values = new ChartValues<GanttPoint>();

            foreach (var task in _tasksCollection)
            {
                _values.Add(new GanttPoint(task.StartDate.Ticks, task.EndDate.Ticks));
            }


            Series = new SeriesCollection
            {
                new RowSeries
                {
                    Values = _values,
                    Title = "",
                    LabelPoint = PointLabel,
                    DataLabels = true
                }
            };

            var labels = new List<string>();
            foreach (var task in _tasksCollection)
            {
                labels.Add(task.Name);
            }
            Labels = labels.ToArray();
            ResetZoomOnClick(null, null);

            DataContext = this;
        }
        //Сброс приближения графика 
        private void ResetZoomOnClick(object sender, RoutedEventArgs e)
        {
            From = _values.First().StartPoint;
            To = _values.Last().EndPoint;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
