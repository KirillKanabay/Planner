using System;
using System.Windows;

namespace Notifications.Wpf
{
    /// <summary>
    /// Interaction logic for ToastWindow.xaml
    /// </summary>
    public partial class NotificationsOverlayWindow : Window, IDisposable
    {
        public NotificationsOverlayWindow()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
        }
    }
}
