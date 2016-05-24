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
using System.Windows.Shapes;

using MyRemoteObject; 

namespace ProjectOffline
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        MyRemoteClass m_RemoteObject;
        public Chat()
        {
            InitializeComponent();
        }
        public Chat(MyRemoteClass connection)
        {
            m_RemoteObject = connection;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }
        private void messageBTN_Click(object sender, RoutedEventArgs e)
        {
            string message = DateTime.Now.ToShortTimeString() + ": " + MessageTxtBox.Text;
            m_RemoteObject.messaging(message);
        }
    }
}
