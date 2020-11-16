using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PlannerController;
using PlannerModel;

namespace PlannerView.UserControls
{
    /// <summary>
    /// Логика взаимодействия для GanttUserControl.xaml
    /// </summary>
    public partial class GanttUserControl : UserControl
    {
        private double _from;
        private double _to;
        
        private ChartValues<GanttPoint> _values;

        private IEnumerable<Category> _categoryList;

        private IEnumerable<PlannerModel.Task> _tasksCollection;
        public Func<ChartPoint, string> PointLabel { get; set; }

        public GanttUserControl()
        {
            InitializeComponent();
            
            _categoryList = new CategoryController().Categories.Skip(1);

            foreach (var category in _categoryList)
            {
                CategoriesBox.Items.Add(category.Name);
            }

            

            Gantt.Visibility = Visibility.Hidden;
        }
        private void AcceptCategoryBtn(object sender, RoutedEventArgs e)
        {
            var taskController = new TaskController();
            _tasksCollection = taskController.Tasks.Where(task => task.Category.Name == CategoriesBox.Text &&
                                                                  !task.IsFinished && task.EndDate != DateTime.Parse("2099-01-01 00:00:00"));
            HelpImage.Visibility = Visibility.Hidden;
            InitGantt();
        }

        private void InitGantt()
        {
            PointLabel = chartPoint =>
                $"Начало: {new DateTime((long)chartPoint.XStart).ToString("dd MMM")} ,конец: {new DateTime((long)chartPoint.X).ToString("dd MMM")}";
            Formatter = value => new DateTime((long)value).ToString("dd MMM");

            if (_tasksCollection.Any())
            {
                Gantt.Visibility = Visibility.Visible;
                GetGantt(_tasksCollection);
            }
            else
            {
                Gantt.Visibility = Visibility.Collapsed;
                HelpImage.Visibility = Visibility.Visible;
            }
        }

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

        public SeriesCollection Series { get; set; }
        public Func<double, string> Formatter { get; set; }
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
