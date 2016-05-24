using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.Sockets;
using MyRemoteObject;
using System.Net;
using System.ComponentModel;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ProjectOffline
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    
    public partial class User : Window
    {
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x00080000;
        [DllImport("user32.dll")]
        private extern static int SetWindowLong(IntPtr hwnd, int index, int value);
        [DllImport("user32.dll")]
        private extern static int GetWindowLong(IntPtr hwnd, int index);
        string remoteIP;
        bool connectionStatus = false;
        TcpChannel m_TcpChan;
        MyRemoteClass m_remote;

        public User()
        {
            SourceInitialized += MainWindow_SourceInitialized;
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            endConBTN.IsEnabled = false;
        }
            
        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper wih = new WindowInteropHelper(this);
            int style = GetWindowLong(wih.Handle, GWL_STYLE);
            SetWindowLong(wih.Handle, GWL_STYLE, style & ~WS_SYSMENU);
        } 
        private void startConBTN_Click(object sender, RoutedEventArgs e)
        {
            connectionStatus = true;
            if (connectionStatus)
            {
                int Port = Convert.ToInt32(portTXT.Text);
                m_TcpChan = new TcpChannel(Port);
                ChannelServices.RegisterChannel(m_TcpChan, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(MyRemoteClass), "FirstRemote", WellKnownObjectMode.SingleCall);
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("10.0.2.4", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    remoteIP = endPoint.Address.ToString();
                }
                m_remote = (MyRemoteClass)Activator.GetObject(typeof(MyRemoteClass), "tcp://" + remoteIP + ":8080/FirstRemote");
                m_remote.setUpMonitorConfig(System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
            }
            startConBTN.IsEnabled = false;
            endConBTN.IsEnabled = true;
        }
        private void endConBTN_Click(object sender, RoutedEventArgs e)
        {
            connectionStatus = false;
            startConBTN.IsEnabled = true;
            endConBTN.IsEnabled = false;
            ChannelServices.UnregisterChannel(m_TcpChan);
        }

        private void getIPBTN_Click(object sender, RoutedEventArgs e)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.2.4", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ipLBL.Content = "Your local IP address is: " + endPoint.Address.ToString();
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            string[] messages = m_remote.incomingMessage();
            if (messages.Length > 0)
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    messageListBox.Items.Add(messages[i]);
                }
            }
        }

        private void logoutBTN_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Show();
            this.Close();
        }
    }
}
