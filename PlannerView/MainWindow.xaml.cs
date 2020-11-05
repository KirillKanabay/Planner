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

namespace PlannerView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var taskController = new TaskController();
            ObservableCollection<PlannerModel.Task> tasks = taskController.Tasks;
            foreach (var task in tasks)
            {
                TaskItem taskItem = new TaskItem(task); 
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
