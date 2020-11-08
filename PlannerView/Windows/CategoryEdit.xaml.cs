using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PlannerController;
using PlannerModel;
using PlannerView.Annotations;
using UtilityLibraries;

namespace PlannerView.Windows
{
    /// <summary>
    /// Логика взаимодействия для CategoryEdit.xaml
    /// </summary>
    public partial class CategoryEdit : Window,INotifyPropertyChanged
    {
        private Category CategoryTemp;
        public CategoryEdit()
        {
            InitializeComponent();
            CategoryTemp = new Category();
            this.DataContext = CategoryTemp;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var categoryController = new CategoryController(CategoryTemp.Name, CategoryTemp.Color);
                //MessageBox.Show("Категория добавлена в планировщик", "Редактор задач", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ColorPickerBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ColorPicker cp = new ColorPicker();
            cp.ShowDialog();
            ColorTextBox.Text = cp.Color ?? "";
            ColorTextBox.Foreground = new SolidColorBrush(ColorLibrary.GetColor(cp.Color ?? "#FF383838"));
            ColorPickerIcon.Foreground = new SolidColorBrush(ColorLibrary.GetColor(cp.Color ?? "#FF383838"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
