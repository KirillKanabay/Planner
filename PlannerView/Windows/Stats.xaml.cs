﻿using System;
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
    /// Логика взаимодействия для Окна статистики
    /// </summary>
    public partial class Stats : Window
    {
        #region Поля и свойства
        #region Свойства круговой диаграммы
        /// <summary>
        /// Задает название элементу графика
        /// </summary>
        public Func<ChartPoint, string> PointLabel { get; set; }
        /// <summary>
        /// Контроллер задачи
        /// </summary>
        private readonly TaskController _taskController;
        /// <summary>
        /// Список задач
        /// </summary>
        private readonly IEnumerable<PlannerModel.Task> _tasksCollection;

        /// <summary>
        /// Количество завершенных задач
        /// </summary>
        private int _isFinishedTasksCount;
        /// <summary>
        /// Количество просроченных задач
        /// </summary>
        private int _isOverdueTasksCount;
        /// <summary>
        /// Количество задач в процессе выполнения
        /// </summary>
        private int _inProcessTasksCount;
        /// <summary>
        /// Общее количество задач
        /// </summary>
        private int _allTaskCount;

        /// <summary>
        /// Значение графика: количество задач в процессе выполнения
        /// </summary>
        private ObservableValue _inProcessTasks;
        /// <summary>
        /// Значение графика: количество просроченных задач
        /// </summary>
        private ObservableValue _isOverdueTasks;
        /// <summary>
        /// Значение графика: количество завершенных задач
        /// </summary>
        private ObservableValue _isFinishedTasks;

        #endregion

        #region Свойства графика
        /// <summary>
        /// Коллекция элементов на оси
        /// </summary>
        private AxesCollection _axesCollection;
        #endregion

        #region Свойства промежутка времени
        private DateTime _startDate;
        private int _daysInPeriod;
        private DateTime _endDate;
        #endregion

        #region Свойства окна
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
                    Show();
                }
                else
                {
                    Close();
                }
            }
        }
        /// <summary>
        /// Свойство задающее заголовок окну
        /// </summary>
        private string title
        {
            set
            {
                Title.Content = value;
            }
        }

        #endregion


        #endregion

        public Stats()
        {
            InitializeComponent();
            //Получение списка задач
            _taskController = new TaskController();
            _tasksCollection = _taskController.Tasks;

            //Определение заголовка
            title = $"Статистика за {_startDate.ToString("MMMM")} месяц";

            //Определение промежутков времени
            _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _daysInPeriod = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            _endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _daysInPeriod);
            
            Chart.Series = GetChartCollection();
            
            UpdateStats();

            DataContext = this;
        }
        
        //Обновление статистики
        private void UpdateStats()
        {
            //Если на момент вызова метода, контроллер не объявлен, выходим
            if(_taskController == null)
                return;
            _axesCollection = new AxesCollection()
            {
                GetDaysOfMonthAxis()
            };
            Graphic.AxisX = _axesCollection;
            Graphic.Series = GetGraphicCollection();
        }

        //Получение круговой диаграммы
        private SeriesCollection GetChartCollection()
        {
            _allTaskCount = _tasksCollection.Count();

            _isOverdueTasksCount = _tasksCollection.Count(task => task.IsOverdue);
            _isFinishedTasksCount = _tasksCollection.Count(task => task.IsFinished);
            int isOverdueAndFinishedTasksCount = _tasksCollection.Count(task => task.IsFinished && task.IsOverdue);
            _inProcessTasksCount = _allTaskCount - (_isOverdueTasksCount + _isFinishedTasksCount - isOverdueAndFinishedTasksCount);

            _inProcessTasks = new ObservableValue(_inProcessTasksCount);
            _isOverdueTasks = new ObservableValue(_isOverdueTasksCount);
            _isFinishedTasks = new ObservableValue(_isFinishedTasksCount);

            PointLabel = chartPoint =>
                string.Format("{0} ({1:P1})", chartPoint.Y,chartPoint.Participation);

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
        //Получение диаграммы
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
                        (task.IsOverdue) 
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
                        (task.IsOverdue) && new DateTime(task.EndDate.Year, task.EndDate.Month, task.EndDate.Day) == date));
                }
            }
            
            return new SeriesCollection()
            {
                finishedTasksCount,
                overdueTasksCount
            };
        }
        //Получение данных дней месяца для оси графика
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
        //Закрыть окно
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        //Закрытие обертки
        private void Stats_OnClosed(object sender, EventArgs e)
        {
            MainWindow.CloseWrap(sender, new RoutedEventArgs());
        }
        //Определение промежутка времени
        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton) sender;
            
            switch (rb.Content.ToString())
            {
                case "За день":
                    _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    _daysInPeriod = 1;
                    _endDate = _startDate;
                    title = $"Статистика за {_startDate:d}";
                    break;

                case "За неделю":
                    _startDate = DateTime.Today;
                    _startDate = new DateTime(_startDate.Year, _startDate.Month, DateTimeExtensions.GetNumberOfMonday(_startDate));
                    _endDate = _startDate.AddDays(6);
                    _daysInPeriod = 7;
                    title = $"Статистика за {_startDate:d} - {_endDate:d}";
                    break;

                case "За месяц":
                    _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    _daysInPeriod = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    _endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, _daysInPeriod);
                    title = $"Статистика за {_startDate:MMMM} месяц";
                    break;
            }
            UpdateStats();
        }

    }
}
