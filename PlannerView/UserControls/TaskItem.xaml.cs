using System.Windows.Controls;
using System.Windows.Media;
using PlannerController;
using PlannerModel;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для TaskItem.xaml
    /// </summary>
    public partial class TaskItem : UserControl
    {
        private readonly TaskController TaskController;
        private readonly PlannerModel.Task Task; 

        public TaskItem(TaskController taskController,int taskId, CategoryController categoryController, PriorityController priorityController)
        {
            InitializeComponent();
            TaskController = taskController;
            Task = taskController.Tasks[taskId];
            var priority = priorityController.GetPriority(Task.PriorityId);
            var category = categoryController.GetCategory(Task.CategoryId);
           
            
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
            TaskController.FinishTask(Task.Id);
            MainWindow.DoRefresh(TaskController);
        }
    }
}
