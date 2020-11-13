using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PlannerController;
using UtilityLibraries;

namespace PlannerView.Windows
{
    /// <summary>
    /// Логика взаимодействия для Stats.xaml
    /// </summary>
    public partial class Stats : Window
    {
        #region Свойства круговой диаграммы
        public Func<ChartPoint, string> PointLabel { get; set; }

        private TaskController _taskController;
        private IEnumerable<PlannerModel.Task> _tasksCollection;

        private int _addedTasksCount;
        private int _isFinishedTasksCount;
        private int _isOverdueTasksCount;
        private int _inProcessTasksCount;
        private int _allTaskCount;

        private ObservableValue _inProcessTasks;
        private ObservableValue _isOverdueTasks;
        private ObservableValue _isFinishedTasks;

        private SeriesCollection _chartCollection;
        #endregion

        #region Свойства графика

        private AxesCollection _axesCollection;
        private SeriesCollection _graphicCollection;
        #endregion

        #region Свойства промежутка времени
        private DateTime _startDate;
        private int _daysInPeriod;
        private DateTime _endDate;
        #endregion

        #region Свойства окна
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

        private string _title;
        #endregion

        public Stats()
        {
            InitializeComponent();
            _taskController = new TaskController();
            _tasksCollection = _taskController.Tasks;

            _title = $"Статистика за {_startDate.ToString("MMMM")} месяц";
            _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _daysInPeriod = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            _endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _daysInPeriod);

            UpdateStats();

            DataContext = this;
        }

        private void UpdateStats()
        {
            //Если на момент вызова метода, контроллер не объявлен, выходим
            if(_taskController == null)
                return;
            Title.Content = _title;
            Chart.Series = GetChartCollection();
            _axesCollection = new AxesCollection()
            {
                GetDaysOfMonthAxis()
            };
            Graphic.AxisX = _axesCollection;
            Graphic.Series = GetGraphicCollection();
        }

        private SeriesCollection GetChartCollection()
        {
            _allTaskCount = _tasksCollection.Count();

            _isOverdueTasksCount = _tasksCollection.Count(task => task.IsOverdue ?? false);
            _isFinishedTasksCount = _tasksCollection.Count(task => task.IsFinished);
            _inProcessTasksCount = _allTaskCount - (_isOverdueTasksCount + _isFinishedTasksCount);

            _inProcessTasks = new ObservableValue(_inProcessTasksCount);
            _isOverdueTasks = new ObservableValue(_isOverdueTasksCount);
            _isFinishedTasks = new ObservableValue(_isFinishedTasksCount);

            PointLabel = chartPoint =>
                string.Format("{0}", (chartPoint.Y>0) ? chartPoint.Y.ToString(): "");

            return new SeriesCollection()
            {
                new PieSeries {Title = "Задачи в процессе",
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    Values = new ChartValues<ObservableValue> {_inProcessTasks}},
                new PieSeries {Title = "Просроченные задачи",
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    Values = new ChartValues<ObservableValue> {_isOverdueTasks}},
                new PieSeries {Title = "Выполненные задачи",
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    Values = new ChartValues<ObservableValue> {_isFinishedTasks}}
            };
        }
        private SeriesCollection GetGraphicCollection()
        {
            LineSeries finishedTasksCount = new LineSeries()
            {
                Title = "Завершенные задачи",
                Values = new ChartValues<int>()
            };
            LineSeries overdueTasksCount = new LineSeries()
            {
                Title = "Просроченные задачи",
                Values = new ChartValues<int>()
            };

            if (_daysInPeriod == 1)
            {
                for (var date = _startDate; date <= _endDate; date = date.AddHours(1))
                {
                    finishedTasksCount.Values.Add(_tasksCollection.Count(task =>
                    {
                        DateTime finishDate = task.FinishDate ?? new DateTime(1980, 1, 1);
                        return new DateTime(finishDate.Year, finishDate.Month, finishDate.Day, finishDate.Hour, 0, 0) == date;
                    }));

                    overdueTasksCount.Values.Add(_tasksCollection.Count(task =>
                        (task.IsOverdue ?? false) 
                        && new DateTime(task.EndDate.Year, task.EndDate.Month, task.EndDate.Day, task.EndDate.Hour,0,0) == date));
                }
            }
            else
            {
                for (var date = _startDate; date <= _endDate; date = date.AddDays(1))
                {
                    finishedTasksCount.Values.Add(_tasksCollection.Count(task =>
                    {
                        DateTime finishDate = task.FinishDate ?? new DateTime(1980, 1, 1);
                        return new DateTime(finishDate.Year, finishDate.Month, finishDate.Day) == date;
                    }));

                    overdueTasksCount.Values.Add(_tasksCollection.Count(task =>
                        (task.IsOverdue ?? false) && new DateTime(task.EndDate.Year, task.EndDate.Month, task.EndDate.Day) == date));
                }
            }
            

            return new SeriesCollection()
            {
                finishedTasksCount,
                overdueTasksCount
            };
        }
        private Axis GetDaysOfMonthAxis()
        {
            Axis axis = new Axis();
            List<String> daysString = new List<string>();

            if (_daysInPeriod == 1)
            {
                _endDate = _endDate.AddHours(23);
                for(var date = _startDate; date <= _endDate; date = date.AddHours(1))
                {
                    daysString.Add(date.ToString("HH:mm"));
                }
            }
            else
            {
                for (var date = _startDate; date <= _endDate; date = date.AddDays(1))
                {
                    daysString.Add(date.ToString("dd MMMM"));
                }
            }
            axis.Labels = daysString;

            return axis;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Stats_OnClosed(object sender, EventArgs e)
        {
            MainWindow.CloseWrap(sender, new RoutedEventArgs());
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton) sender;
            
            switch (rb.Content.ToString())
            {
                case "За день":
                    _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    _daysInPeriod = 1;
                    _endDate = _startDate;
                    break;

                case "За неделю":
                    _startDate = DateTime.Today;
                    _startDate = new DateTime(_startDate.Year, _startDate.Month, DateTimeExtensions.GetNumberOfMonday(_startDate));
                    _endDate = _startDate.AddDays(6);
                    _daysInPeriod = 7;
                    break;

                case "За месяц":
                    _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    _daysInPeriod = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    _endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _daysInPeriod);
                    break;
            }
            UpdateStats();
        }
    }
}
