using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using PlannerController;
using PlannerModel;

namespace PlannerView.Windows
{
    /// <summary>
    /// Логика взаимодействия для TaskEdit.xaml
    /// </summary>
    public partial class TaskEdit : Window
    {
        public TaskEdit()
        {
            InitializeComponent();
            //Добавление placeholder для ввода названия задачи
            TaskNameTextBox.Text = "Введите название задачи";
            TaskNameTextBox.GotFocus += RemoveText;
            TaskNameTextBox.LostFocus += AddText;
            
            //Получение списка приоритетов
            var priorityController = new PriorityController();
            var priorities = GetPrioritiesName(priorityController);
            PrioritiesBox.ItemsSource = priorities;

            //Получение списка категорий
            var categoryController = new CategoryController();
            var categories = GetCategoriesName(categoryController);
            CategoryBox.ItemsSource = categories;
        }

        ObservableCollection<String> GetPrioritiesName(PriorityController controller)
        {
            var priorities = new ObservableCollection<string>();
            foreach (Priority priority in controller.Items)
            {
                priorities.Add(priority.Name);
            }

            return priorities;
        }
        ObservableCollection<String> GetCategoriesName(CategoryController controller)
        {
            var categories = new ObservableCollection<string>();
            foreach (Category category in controller.Items)
            {
                categories.Add(category.Name);
            }

            return categories;
        }
        void RemoveText(object sender, EventArgs e)
        {
            if (TaskNameTextBox.Text == "Введите название задачи")
            {
                TaskNameTextBox.Text = "";
            }
        }

        void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text))
                TaskNameTextBox.Text = "Введите название задачи";
        }
        /// <summary>
        /// Возвращает дату из строк даты и времени
        /// </summary>
        /// <param name="date">Строка даты</param>
        /// <param name="time">Строка времени</param>
        /// <param name="EndTheTime">Если строки пусты возвращает максимальную дату</param>
        /// <returns> Дата </returns>
        private DateTime GetDate(string date, string time, bool isMaxTime = true)
        {
            var maxValue = new DateTime(2099, 1,1);
            var minValue = new DateTime(1980, 1,1);
            DateTime dt;
            if (!DateTime.TryParse(date, out dt))
            {
                dt = (isMaxTime) ? maxValue:minValue;
            }

            TimeSpan ts;
            if (!TimeSpan.TryParse(time, out ts))
            {
                ts = new TimeSpan(0,0,0);
            }

            dt = dt.Add(ts);
            
            return dt;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = TaskNameTextBox.Text;
                var startTime = GetDate(StartDate.Text, StartTime.Text, false);
                var endTime = GetDate(EndDate.Text, EndTime.Text, true);
                ;
                var priorityId = PrioritiesBox.SelectedIndex + 1;
                var categoryId = CategoryBox.SelectedIndex + 1;
                var taskController = new TaskController(name, startTime, endTime, priorityId, categoryId);
                MessageBox.Show("Задача добавлена в планировщик", "Редактор задач", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                MainWindow.DoRefresh(taskController);
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
