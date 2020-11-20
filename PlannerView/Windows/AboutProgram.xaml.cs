using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
using EO.Internal;

namespace PlannerView.Windows
{
    /// <summary>
    /// Логика взаимодействия для AboutProgram.xaml
    /// </summary>
    public partial class AboutProgram : Window
    {
        #region Свойства окна
        /// <summary>
        /// Является ли окно открытым
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
        /// <summary>
        /// Свойство версии программы
        /// </summary>
        private string version
        {
            set
            {
                ProdVer.Content = value;
            }
        }
        /// <summary>
        /// Свойство имени разработчика
        /// </summary>
        private string developerName
        {
            set
            {
                DevName.Content = value;
            }
        }
        /// <summary>
        /// Свойство e-mail'a разработчика
        /// </summary>
        private string eMail
        {
            set
            {
                Email.Content = value;
            }
        }
        #endregion

        public AboutProgram()
        {
            InitializeComponent();
            version = "Product version: 0.1 Alpha";
            developerName = "Developer: Kirill Kanabay";
            eMail = "E-mail: kanabay.dev@gmail.com";
        }
        /// <summary>
        /// Закрытие приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       /// <summary>
       /// Закрытие обертки
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void AboutProgram_OnClosed(object sender, EventArgs e)
        {
            MainWindow.CloseWrap(sender, new RoutedEventArgs());
        }
       /// <summary>
       /// Открытие справки
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("help.chm");
        }
    }
}
