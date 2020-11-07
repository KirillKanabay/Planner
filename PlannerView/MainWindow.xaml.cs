
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public Func<PlannerModel.Task, bool> Filter;

        public MainWindow()
        {
            InitializeComponent();
            Filter = Filter = (task) => DateTime.Now >= task.StartTime && DateTime.Now <= task.EndTime;
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
            ObservableCollection<Task> tasksCollection = taskController.Tasks;
            var tasks= tasksCollection.Where(Filter);
            foreach (var task in tasks)
            {
                //if(taskController.Tasks[i].IsFinished) continue;
                TaskItem taskItem = new TaskItem(taskController,task,categoryController,priorityController);
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox) sender;
            var lbi = (ListBoxItem) item.SelectedItem;
            var stackPanel = (StackPanel) lbi?.Content;
            var label = (Label) stackPanel?.Children[1];
            
            if(label == null)
                return;
            TaskTitle.Content = label.Content.ToString();
            switch (label.Content.ToString())
            { 
                case "Бессрочные задачи":
                    Filter = (task) => task.EndTime == DateTime.Parse("2099-01-01 00:00:00");
                    break;
                case "Задачи на сегодня":
                    Filter = (task) => DateTime.Now >= task.StartTime && DateTime.Now <= task.EndTime;
                    break;
                case "Предстоящие задачи":
                    Filter = (task) => DateTime.Now < task.StartTime;
                    break;
                case "Выполненные задачи":
                    Filter = (task) => task.IsFinished;
                    break; 
                case "Просроченные задачи":
                    Filter = (task) => DateTime.Now > task.EndTime && !task.IsFinished;
                        break; 
                case "Задачи срочного приоритета":
                    Filter = (task) => task.PriorityId == 3;
                        break;
            }
            DoRefresh(taskController);
        }
    }
}
