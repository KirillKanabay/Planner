using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PlannerController;
using PlannerModel;
using PlannerView.Annotations;

namespace PlannerView.Windows
{
    /// <summary>
    /// Логика взаимодействия для Редактора категорий
    /// </summary>
    public partial class CategoryEdit : Window,INotifyPropertyChanged
    {
        #region Поля и свойства
        /// <summary>
        /// Модель категории
        /// </summary>
        private Category CategoryModel;

        /// <summary>
        /// Является ли окно открытым?
        /// </summary>
        private bool _isOpen;
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                if (_isOpen)
                {
                    Show();
                }
                else
                {
                    Close();
                }
            }
        }
        #endregion
        
        public CategoryEdit()
        {
            InitializeComponent();
            CategoryModel = new Category();
            DataContext = CategoryModel;
        }

        /// <summary>
        /// Закрываем окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Close(object sender, RoutedEventArgs e)
        {
            _isOpen = false;
        }
        /// <summary>
        /// Сохраняем категорию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var categoryController = new CategoryController(CategoryModel.Name, CategoryModel?.Color);
                Close();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Неправильный формат цвета.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
        /// <summary>
        /// Закрываем окно редактора категорий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Открываем канву с выбором цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPickerBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ColorPickerPopup.IsOpen = !ColorPickerPopup.IsOpen;
        }
        /// <summary>
        /// При закрытии редактора закрываем обертку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CategoryEdit_OnClosed(object sender, EventArgs e)
        {
            if (MainWindow._taskEdit?.IsOpen ?? false)
            {
                TaskEdit.DoRefreshCategoryList();
                MainWindow._taskEdit.Show();
            }
            else
            {
                MainWindow.CloseWrap(sender, new RoutedEventArgs());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
