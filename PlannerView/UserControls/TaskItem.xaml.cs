using System.Windows.Controls;
using System.Windows.Media;
using PlannerModel;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для TaskItem.xaml
    /// </summary>
    public partial class TaskItem : UserControl
    {
        private PlannerModel.Task Task { get; set; }

        public TaskItem(PlannerModel.Task task, Category category, Priority priority)
        {
            InitializeComponent();
            Task = task;

            TaskName.Content = Task.Name;
            StartDate.Content = Task.StartTime.ToString("g");
            EndDate.Content = Task.EndTime.ToString("g");
            
            PriorityBackground.Background = new SolidColorBrush(GetColor(priority.Color));
            
            CategoryBackground.Background = new SolidColorBrush(GetColor(category.Color));

            Priority.Content = priority.Name;
            Category.Content = category.Name;
        }

        private Color GetColor(string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FinishTask(Task.Id);
        }
    }
}
