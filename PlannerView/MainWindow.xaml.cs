
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using PlannerController;
using PlannerView.Windows;

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
        private CategoryEdit categoryEdit;
        
        static public TaskEdit taskEdit;

        public delegate void TaskHandler(TaskController taskController);
        public delegate void WrapHadler(object sender,RoutedEventArgs e);
        public delegate void SnackbarHadler(string message);
        
        public static event TaskHandler TaskListChanged;
        public static event WrapHadler CloseWrapEvent;
        public static event SnackbarHadler SnackbarNotifyEvent;
        public Func<PlannerModel.Task, bool> Filter;

        public MainWindow()
        {
            InitializeComponent();
            Filter = Filter = (task) => DateTime.Now >= task.StartTime && DateTime.Now <= task.EndTime;
            
            TaskListChanged += RefreshTaskList;
            CloseWrapEvent += WrapBtn_OnClick;
            SnackbarNotifyEvent += SnackbarNotify;

            taskController = new TaskController();
            categoryController = new CategoryController();
            priorityController = new PriorityController();
            
            DoRefresh(taskController);
        }

        public static void SendSnackbar(string message)
        {
            SnackbarNotifyEvent?.Invoke(message);
        }

        public static void CloseWrap(object sender, RoutedEventArgs e)
        {
            CloseWrapEvent?.Invoke(sender, e);
        }
        public static void DoRefresh(TaskController taskController)
        {
            TaskListChanged?.Invoke(taskController);
        }

        private void RefreshTaskList(TaskController taskController)
        {
            TaskList.Children.RemoveRange(0,TaskList.Children.Count);
            ObservableCollection<PlannerModel.Task> tasksCollection = taskController.Tasks;
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
            Wrap.Visibility = Visibility.Visible;

            taskEdit = new TaskEdit();
            taskEdit.ShowInTaskbar = false;
            taskEdit.IsOpen = true;
        }

        private void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            Wrap.Visibility = Visibility.Visible;
            
            categoryEdit = new CategoryEdit();
            categoryEdit.ShowInTaskbar = false;
            categoryEdit.IsOpen = true;
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

        private void WrapBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (categoryEdit?.IsOpen ?? false)
            {
                categoryEdit.IsOpen = false;
            }

            if (taskEdit?.IsOpen ?? false)
            {
                taskEdit.IsOpen = false;
            }

            Wrap.Visibility = Visibility.Collapsed;
        }

        private void SnackbarNotify(string message)
        {
            //use the message queue to send a message.
            var messageQueue = Snackbar.MessageQueue;
            //the message queue can be called from any thread
            Task.Factory.StartNew(() => messageQueue.Enqueue(message));
        }
    }
}
