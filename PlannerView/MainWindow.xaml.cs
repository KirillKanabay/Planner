using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PlannerView.Windows;
using Task = PlannerModel.Task;

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskController taskController;
        private CategoryController categoryController;
        private PriorityController priorityController;

        public delegate void TaskHandler(TaskController taskController);
        public static event TaskHandler TaskListChanged;

        public MainWindow()
        {
            InitializeComponent();

            TaskListChanged += RefreshTaskList;

            taskController = new TaskController();
            categoryController = new CategoryController();
            priorityController = new PriorityController();
            
            DoRefresh(taskController);
        }
        public static void DoRefresh(TaskController taskController)
        {
            TaskListChanged?.Invoke(taskController);
        }
        private void RefreshTaskList(TaskController taskController)
        {
            TaskList.Children.RemoveRange(0,TaskList.Children.Count);
            ObservableCollection<Task> tasks= taskController.Tasks;
            foreach (var task in tasks)
            {
                TaskItem taskItem = new TaskItem(task,
                    categoryController.GetCategory(task.CategoryId),
                    priorityController.GetPriority(task.PriorityId));
                TaskList.Children.Add(taskItem);
            }
        }
        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            TaskEdit taskEdit = new TaskEdit();
            taskEdit.Show();
        }

        private void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            CategoryEdit categoryEdit = new CategoryEdit();
            categoryEdit.Show();
        }
    }
}
