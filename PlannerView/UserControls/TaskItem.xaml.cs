using System;
using System.Collections.Generic;
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
using PlannerController;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для TaskItem.xaml
    /// </summary>
    public partial class TaskItem : UserControl
    {
        public TaskItem(PlannerModel.Task task)
        {
            var categoryController = new CategoryController();
            var priorityController = new PriorityController();
            InitializeComponent();
            TaskName.Content = task.Name;
            StartDate.Content = task.StartTime.ToString();
            EndDate.Content = task.EndTime.ToString();
            Priority.Content = task.Priority.Name;
            Category.Content = categoryController.GetCategory(task.CategoryId).Name;
        }
    }
}
