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
using System.Management;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using MyRemoteObject;
using System.ComponentModel;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ProjectOffline
{
    /// <summary>
    /// Interaction logic for Root.xaml
    /// </summary>
    public partial class Root : Window
    {
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x00080000;
        [DllImport("user32.dll")]
        private extern static int SetWindowLong(IntPtr hwnd, int index, int value);
        [DllImport("user32.dll")]
        private extern static int GetWindowLong(IntPtr hwnd, int index);
        public static class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Win32Point
            {
                public Int32 X;
                public Int32 Y;
            }
            [DllImport("user32.dll")]
            public static extern bool GetCursorPos(ref Win32Point pt);
        }
        string destinationIP;
        string portSetting;
        MyRemoteClass m_RemoteObject;
        string[] fileNames = new string[100];
        int fileNamesCounter = 0;
        StreamWriter w;
        bool successfulSendFile;
        bool successfulStartProcess;
        bool successfulEndProcess;
        private BackgroundWorker _worker = null;
        int fileStreamName = 0;
        int streamCounter = 0;
        string newStreamFileName;
        string[] streamFiles = new string[10000];

        public Root()
        {
            SourceInitialized += MainWindow_SourceInitialized;
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ListBox.MouseDoubleClick += new MouseButtonEventHandler(ListBox_MouseDoubleClick);
            directoryListBox.MouseDoubleClick += new MouseButtonEventHandler(directoryListBox_MouseDoubleClick); 
            
            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;
           
            closeConMI.IsEnabled = false;
            detailMI.IsEnabled = false;
            captureMI.IsEnabled = false;
            streamMI.IsEnabled = false;
            commandsMI.IsEnabled = false;
            startProcessMI.IsEnabled = false;
            settingsMI.IsEnabled = false;
            ChatMI.IsEnabled = false;
        }

        void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper wih = new WindowInteropHelper(this);
            int style = GetWindowLong(wih.Handle, GWL_STYLE);
            SetWindowLong(wih.Handle, GWL_STYLE, style & ~WS_SYSMENU);
        } 
            
        private void KeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            if(e.Key == Key.Space)
            {
                m_RemoteObject.DoMouseClick();
            }
        }
        private void loadConnectionScreen_Click(object sender, RoutedEventArgs e)
        {
            openConMI.IsEnabled = false;

            connectionGrid.Visibility = Visibility.Visible;
            conLBL.Visibility = Visibility.Hidden;
        }
        private void conBTN_Click(object sender, RoutedEventArgs e)
        {
            destinationIP = ipTXT.Text;
            portSetting = portTXT.Text;
            try
            {
                m_RemoteObject = (MyRemoteClass)Activator.GetObject(typeof(MyRemoteClass), "tcp://" + destinationIP + ":" + portSetting + "/FirstRemote");
                if (m_RemoteObject.setupCon() == true)
                {
                    closeConMI.IsEnabled = true;
                    detailMI.IsEnabled = true;
                    captureMI.IsEnabled = true;
                    streamMI.IsEnabled = true;
                    commandsMI.IsEnabled = true;
                    startProcessMI.IsEnabled = true;
                    settingsMI.IsEnabled = true;
                    ChatMI.IsEnabled = true;
                    openConMI.IsEnabled = false;
                    connectionGrid.Visibility = Visibility.Hidden;
                    conLBL.Visibility = Visibility.Visible;
                    conLBL.Content = "Connection has been successful!";
                    w = File.AppendText("" + DateTime.Now.ToLongDateString() + " " + destinationIP + "log.txt");
                    w.WriteLine(DateTime.Now.ToLongTimeString() + ": Connection to remote client has been successfully established. IP:"+destinationIP+" Port:"+portSetting+".");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("A error has occurred: " + ex.Message + "\n" + "Check you've entered the correct IP address or selected the correct port of the computer you want to connect to.", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
            };
        }

        private void disconBTN_Click(object sender, RoutedEventArgs e)
        {
            openConMI.IsEnabled = true;
            closeConMI.IsEnabled = false;
            detailMI.IsEnabled = false;
            captureMI.IsEnabled = false;
            commandsMI.IsEnabled = false;
            settingsMI.IsEnabled = false;

            conLBL.Visibility = Visibility.Visible;
            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;                
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            driveGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;

            conLBL.Content = "Connection has been terminated!";
            w.WriteLine(DateTime.Now.ToLongTimeString() + ": Connection has been terminated.");
            w.Close();
            m_RemoteObject = null;
        }
        private void logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Connection has been terminated.");
                w.Close();
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Nothing was logged, as a session was never started", "Message:"); }
            MainWindow win = new MainWindow();
            win.Show();
            this.Close();
        }

        private void systemSpecBTN_Click(object sender, RoutedEventArgs e)
        {
            pcSpecsMI.IsEnabled = false;
            driversMI.IsEnabled = true;
            directoriesMI.IsEnabled = true;
            processesMI.IsEnabled = true;
            ServicesMI.IsEnabled = true;
            streamMI.IsEnabled = true;
            sendFileMI.IsEnabled = true;
            startProcessMI.IsEnabled = true;
            endProcessMI.IsEnabled = true;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Visible;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            driveGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;
            try
            {
                string[] Specs = m_RemoteObject.displaySpecs();
                osLBL.Content = "Operating System: " + Specs[0];
                osArchitectureLBL.Content = "OS Architecture: " + Specs[1];
                cpuLBL.Content = "CPU: " + Specs[2];
                ramLBL.Content = "RAM: " + Specs[3];
                usernameLBL.Content = "Logged In As: " + Specs[4];
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully displayed PC specs.");
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error Message:"); w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error has occured attempting to display remote PC specs."); }
        }

        private void DisplayDriversBTN_Click(object sender, RoutedEventArgs e)
        {
            driveGrid.Children.Clear();

            pcSpecsMI.IsEnabled = true;
            driversMI.IsEnabled = false;
            directoriesMI.IsEnabled = true;
            processesMI.IsEnabled = true;
            ServicesMI.IsEnabled = true;
            streamMI.IsEnabled = true;
            sendFileMI.IsEnabled = true;
            startProcessMI.IsEnabled = true;
            endProcessMI.IsEnabled = true;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            driveGrid.Visibility = Visibility.Visible;
            directoryListBox.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;

            int yAxis = 0;
            int yAxisInc = 20;
            int xAxis = 0;
            int xAxisInc = 200;
            int columnCounter = 0;
            string[] labelContent = { "Name: ", "Drive Type: ", "Volume Label: ", "File System: ", "Available Space To User: ", "Total Available Space: ", "Total Size Of Drive: " };
            try
            {
                string[][] driveJagArr;
                driveJagArr = m_RemoteObject.displayDrives();

                for (int j = 0; j < driveJagArr.Length; j++)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                        if (i > 3)
                        {
                            label.Content = labelContent[i] + driveJagArr[j][i] + "GB";
                        }
                        else if (driveJagArr[j][i] == "")
                        {
                            label.Content = labelContent[i] + "N/A";
                        }
                        else { label.Content = labelContent[i] + driveJagArr[j][i]; }
                        label.VerticalAlignment = VerticalAlignment.Top;
                        if (columnCounter == 1)
                        {
                            label.Margin = new Thickness(xAxis, yAxis + 20, 0, 0);
                        }
                        else { label.Margin = new Thickness(xAxis, yAxis, 0, 0); }
                        driveGrid.Children.Add(label);
                        yAxis = yAxis + yAxisInc;
                    }
                    if (columnCounter == 1)
                    {
                        xAxis = xAxis + xAxisInc;
                        yAxis = 0;
                        columnCounter = 0;
                    }
                    else
                        columnCounter++; 
                }
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully displayed remote drive details.");
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error Message:"); w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error has occured attempting to display drive details."); }
        }

        private void loadDisplayDirectoriesScreen_Click(object sender, RoutedEventArgs e)
        {
            pcSpecsMI.IsEnabled = true;
            driversMI.IsEnabled = true;
            directoriesMI.IsEnabled = false;
            processesMI.IsEnabled = true;
            ServicesMI.IsEnabled = true;
            streamMI.IsEnabled = true;
            sendFileMI.IsEnabled = true;
            startProcessMI.IsEnabled = true;
            endProcessMI.IsEnabled = true;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            driveGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Visible;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;
        }
        string previousDirectory = "C:\\";
        private void DisplayDirectoriesBTN_Click(object sender, RoutedEventArgs e)
        {
           directoryListBox.Items.Clear();
           directoryListBox.Visibility = Visibility.Visible;
           string searchDirectory = directoryTXT.Text;
           try
           {
               string[] directories = m_RemoteObject.displayDirectories(searchDirectory);
               for (int i = 0; i < directories.Length; i++)
               {
                   directoryListBox.Items.Add(directories[i]);
               }
               w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully displayed remote directories.");
           }
           catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error Message:"); w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error has occured attempting to display remote directories."); }
        }
        private void directoryListBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (directoryListBox.SelectedItem != null)
                {
                    if (directoryListBox.SelectedItem.ToString().EndsWith(".txt") || directoryListBox.SelectedItem.ToString().EndsWith(".png") || directoryListBox.SelectedItem.ToString().EndsWith(".jpg"))
                    {
                        string tmp = directoryTXT.Text;
                        directoryTXT.Text += directoryListBox.SelectedItem.ToString();
                        retrieveFileLBL.Content = "File: " + directoryListBox.SelectedItem.ToString() + " was succesfully retrieved.";
                        MemoryStream stream = new MemoryStream(m_RemoteObject.RetrieveFile(directoryTXT.Text));
                        directoryTXT.Text = tmp;
                        FileStream convertedFile = new FileStream(saveLocationTXT.Text + "\\" + directoryListBox.SelectedItem.ToString(), FileMode.Create, FileAccess.Write);
                        stream.WriteTo(convertedFile);
                        convertedFile.Close();
                        convertedFile.Close();
                        w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully retrieved file: "+directoryListBox.SelectedItem.ToString()+" From remote client.");
                    }
                    else
                    {
                        previousDirectory = directoryTXT.Text;
                        directoryTXT.Text += directoryListBox.SelectedItem.ToString() + "\\";
                        string searchDirectory = directoryTXT.Text;
                        string[] directories = m_RemoteObject.displayDirectories(searchDirectory);
                        directoryListBox.Items.Clear();
                        for (int i = 0; i < directories.Length; i++)
                        {
                            directoryListBox.Items.Add(directories[i]);
                        }
                    }
                }
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error Message:"); directoryTXT.Text = previousDirectory; }
        }
        private void directoryBackBTN_Click(object sender, RoutedEventArgs e)
        {
            
            int index = previousDirectory.LastIndexOf("/");
            if (index > 0)
            {
                previousDirectory = previousDirectory.Substring(0, index); // or index + 1 to keep slash
            }
           directoryListBox.Items.Clear();
           directoryTXT.Text = previousDirectory;
           try
           {
               string[] directories = m_RemoteObject.displayDirectories(previousDirectory);
               for (int i = 0; i < directories.Length; i++)
               {
                   directoryListBox.Items.Add(directories[i]);
               }
           }
           catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error Message:"); };
        }
       
        private void processesBTN_Click(object sender, RoutedEventArgs e)
        {
            ListBox.Items.Clear();

            pcSpecsMI.IsEnabled = true;
            driversMI.IsEnabled = true;
            directoriesMI.IsEnabled = true;
            processesMI.IsEnabled = false;
            ServicesMI.IsEnabled = true;
            streamMI.IsEnabled = true;
            sendFileMI.IsEnabled = true;
            startProcessMI.IsEnabled = true;
            endProcessMI.IsEnabled = true;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Visible;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;      
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            driveGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;
            try
            {
                string[] processes = m_RemoteObject.displayProcess();
                for (int i = 0; i < processes.Length; i++)
                {
                    ListBox.Items.Add(processes[i]);
                }
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully displayed remote processes.");
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error Message:"); w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error has occured attempting to display remote processes."); }
        }
        private void ServicesBTN_Click(object sender, RoutedEventArgs e)
        {
            serviceBox.Items.Clear();
            pcSpecsMI.IsEnabled = true;
            driversMI.IsEnabled = true;
            directoriesMI.IsEnabled = true;
            processesMI.IsEnabled = true;
            ServicesMI.IsEnabled = false;
            streamMI.IsEnabled = true;
            sendFileMI.IsEnabled = true;
            startProcessMI.IsEnabled = true;
            endProcessMI.IsEnabled = true;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Visible;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            driveGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;
            try
            {
                string[] services = m_RemoteObject.displayServices();
                for (int i = 0; i < services.Length; i++)
                {
                    serviceBox.Items.Add(services[i]);
                }
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully displayed remote services.");
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error Message:"); w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error has occured attempting to display remote services."); }
        }

        private void ListBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (ListBox.SelectedItem != null)
            {

                String processName = ListBox.SelectedItem.ToString();
                try
                {
                    string[] processInfo = m_RemoteObject.ProcessDetails(processName);
                    processDetailsGrid.Visibility = Visibility.Visible;
                    processNameLbl.Content = "Process Name: " + processInfo[0];
                    windowTitleLbl.Content = "Window Title: " + processInfo[1];
                    idLbl.Content = "ID: " + processInfo[2];
                    appStartTimeLbl.Content = "Application Start Time: " + processInfo[3];
                    userProcTimeLbl.Content = "User Processing Time: " + processInfo[4];
                    priorityLbl.Content = "Priority: " + processInfo[5];
                    respondingLbl.Content = "Responding: " + processInfo[6];
                    virtualMemoryLbl.Content = "Virtual Memory: " + processInfo[7] + " bytes";
                    w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully displayed process: "+processName+" details.");
                }
                catch (Exception d) { System.Windows.Forms.MessageBox.Show(d.Message, "Error Message:"); w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error has ocurred attempting to display process details."); }
            }
        } 

        private void screenshotBTN_Click(object sender, RoutedEventArgs e)  
        {
            string newFileName = generateFileName();
            bool nextStep = false;
            try
            {
                byte[] bitmapPre = m_RemoteObject.TakeScreenshot();
                ImageConverter ic = new ImageConverter();
                System.Drawing.Image img = (System.Drawing.Image)ic.ConvertFrom(bitmapPre);
                Bitmap bitmap = new Bitmap(img);
                bitmap.Save(newFileName, ImageFormat.Jpeg);
                bitmap.Dispose();
                nextStep = true;
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Error when saving picture", "Error Message:"); w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error when saving screenshot.");}
            if (nextStep)
            {
                try
                {
                    ImageSource imageSource = new BitmapImage(new Uri(newFileName));
                    pcSpecsMI.IsEnabled = true;
                    driversMI.IsEnabled = true;
                    directoriesMI.IsEnabled = true;
                    processesMI.IsEnabled = true;
                    ServicesMI.IsEnabled = true;
                    streamMI.IsEnabled = true;
                    sendFileMI.IsEnabled = true;
                    startProcessMI.IsEnabled = true;
                    endProcessMI.IsEnabled = true;

                    screenShotImg.Visibility = Visibility.Visible;
                    pcspecsGrid.Visibility = Visibility.Hidden;
                    ListBox.Visibility = Visibility.Hidden;
                    serviceBox.Visibility = Visibility.Hidden;
                    connectionGrid.Visibility = Visibility.Hidden;
                    conLBL.Visibility = Visibility.Hidden;
                    settingsGrid.Visibility = Visibility.Hidden;
                    processDetailsGrid.Visibility = Visibility.Hidden;
                    startStreamBTN.Visibility = Visibility.Hidden;
                    endStreamBTN.Visibility = Visibility.Hidden;
                    moveMouseBTN.Visibility = Visibility.Hidden;
                    endMouseBTN.Visibility = Visibility.Hidden;
                    driveGrid.Visibility = Visibility.Hidden;
                    directoryListBox.Visibility = Visibility.Hidden;
                    directoryGrid.Visibility = Visibility.Hidden;
                    sendFileGrid.Visibility = Visibility.Hidden;
                    startProcessGrid.Visibility = Visibility.Hidden;
                    endProcessGrid.Visibility = Visibility.Hidden;
                    screenShotImg.Source = imageSource;
                    w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully taken a screenshot.");
                }
                catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Can't find picture", "Error Message:"); w.WriteLine(DateTime.Now.ToLongTimeString() + ": error occured when displaying screenshot"); }
            }
        }

        private void loadStreamScreen_Click(object sender, RoutedEventArgs e)
        {
            pcSpecsMI.IsEnabled = true;
            driversMI.IsEnabled = true;
            directoriesMI.IsEnabled = true;
            processesMI.IsEnabled = true;
            ServicesMI.IsEnabled = true;
            streamMI.IsEnabled = false;
            sendFileMI.IsEnabled = true;
            startProcessMI.IsEnabled = true;
            endProcessMI.IsEnabled = true;
            endStreamBTN.IsEnabled = false;
            endMouseBTN.IsEnabled = false;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Visible;
            endStreamBTN.Visibility = Visibility.Visible;
            moveMouseBTN.Visibility = Visibility.Visible;
            endMouseBTN.Visibility = Visibility.Visible;
            driveGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;
        }
        private void startStreamBTN_Click(object sender, RoutedEventArgs e)
        {
            w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully started to stream remote computer's screen.");
            endStreamBTN.IsEnabled = true;
            connectionMI.IsEnabled = false;
            detailMI.IsEnabled = false;
            captureMI.IsEnabled = false;
            commandsMI.IsEnabled = false;
            settingsMI.IsEnabled = false;
            
            screenShotImg.Visibility = Visibility.Visible;

            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler((state, args) =>
            {
                do
                {
                    if (_worker.CancellationPending)
                        break;
                        Stream();
                } while (true);
            });

            _worker.RunWorkerAsync();
            startStreamBTN.IsEnabled = false;
            endStreamBTN.IsEnabled = true;
        }
        private void endStreamBTN_Click(object sender, RoutedEventArgs e)
        {
            startStreamBTN.IsEnabled = false;
            connectionMI.IsEnabled = true;
            detailMI.IsEnabled = true;
            captureMI.IsEnabled = true;
            commandsMI.IsEnabled = true;
            settingsMI.IsEnabled = true;
            startStreamBTN.IsEnabled = true;
            endStreamBTN.IsEnabled = false;
            _worker.CancelAsync();
            w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully stopped to stream remote computer's screen.");
        }

        private void Stream()
        {
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
           delegate()
           {
               byte[] byteArray = new byte[0];
               byteArray = m_RemoteObject.TakeScreenshot();
               ImageConverter ic = new ImageConverter();
               System.Drawing.Image img = (System.Drawing.Image)ic.ConvertFrom(byteArray);
               Bitmap bitmap1 = new Bitmap(img);
               string filePath = saveLocationTXT.Text + "Stream";
               if(!Directory.Exists(filePath))
               {
                   Directory.CreateDirectory(filePath);
               }
               bitmap1.Save(saveLocationTXT.Text + "Stream\\" + fileStreamName + ".jpg", ImageFormat.Jpeg);
               bitmap1.Dispose();
               newStreamFileName = saveLocationTXT.Text + "Stream\\" + fileStreamName + ".jpg";
               streamFiles[streamCounter] = newStreamFileName;
               ImageSource imageSource = new BitmapImage(new Uri(newStreamFileName));
               screenShotImg.Source = imageSource;
               fileStreamName = fileStreamName + 1;
               streamCounter = streamCounter + 1;
           }));
        }
        private void loadSendFileScreen_Click(object sender, RoutedEventArgs e)
        {
            pcSpecsMI.IsEnabled = true;
            driversMI.IsEnabled = true;
            directoriesMI.IsEnabled = true;
            processesMI.IsEnabled = true;
            ServicesMI.IsEnabled = true;
            streamMI.IsEnabled = true;
            sendFileMI.IsEnabled = false;
            startProcessMI.IsEnabled = true;
            endProcessMI.IsEnabled = true;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Visible;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;
            sendFileLocationTXT.Text = "C:\\Filename";
            sendFileTXT.Text = "C:\\Filename";
            successLbl.Content = "";
        }

        private void sendFileBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream fs = new FileStream(sendFileTXT.Text, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] bin = br.ReadBytes(Convert.ToInt32(fs.Length));
                fs.Close();
                br.Close();
                successfulSendFile = m_RemoteObject.SendFile(bin, sendFileLocationTXT.Text);
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully sent file: "+sendFileLocationTXT.Text+" to remote computer.");
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error Message:"); successLbl.Content = "Problem sending file!"; w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error occured attempting to send file:" + sendFileLocationTXT.Text + " to remote computer."); }
            if (successfulSendFile)
            {
                successLbl.Content = "Successfully sent file!";
            }
            else
                successLbl.Content = "Problem sending file!";
        }
        private void findFileBTN_Click(object sender, RoutedEventArgs e)
        {
            successLbl.Content = "";
            FileDialog file = new OpenFileDialog();
            if (file.ShowDialog().ToString().Equals("OK"))
            {
                sendFileTXT.Text = file.FileName;
            }
        }

        private void loadStartProcessScreen_Click(object sender, RoutedEventArgs e)
        {
            pcSpecsMI.IsEnabled = true;
            driversMI.IsEnabled = true;
            directoriesMI.IsEnabled = true;
            processesMI.IsEnabled = true;
            ServicesMI.IsEnabled = true;
            streamMI.IsEnabled = true;
            sendFileMI.IsEnabled = true;
            startProcessMI.IsEnabled = false;
            endProcessMI.IsEnabled = true;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Visible;
            endProcessGrid.Visibility = Visibility.Hidden;
        }

        private void startProcessBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                successfulStartProcess = m_RemoteObject.StartProcess(startProcessNameTXT.Text);
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully started process: " + startProcessNameTXT.Text + " on remote computer.");
            }
            catch (Exception d) { System.Windows.Forms.MessageBox.Show(d.Message, "Error Message:"); startProcessSuccLbl.Content = "Problem starting process!"; w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error occured when attempting to start process: " + startProcessNameTXT.Text + " on remote computer."); }
            if (successfulStartProcess)
            {
                startProcessSuccLbl.Content = "Successfully started process!";
            }
            else
                startProcessSuccLbl.Content = "Problem starting process!";
        }

        private void loadEndProcessScreen_Click(object sender, RoutedEventArgs e)
        {
            pcSpecsMI.IsEnabled = true;
            driversMI.IsEnabled = true;
            directoriesMI.IsEnabled = true;
            processesMI.IsEnabled = true;
            ServicesMI.IsEnabled = true;
            streamMI.IsEnabled = true;
            sendFileMI.IsEnabled = true;
            startProcessMI.IsEnabled = true;
            endProcessMI.IsEnabled = false;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Hidden;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Visible;
        }

        private void endProcessBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                successfulEndProcess = m_RemoteObject.EndProcess(endProcessNameTXT.Text);
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully stopped process: " + endProcessNameTXT.Text + " on remote computer.");
            }
            catch (Exception d) { System.Windows.Forms.MessageBox.Show(d.Message, "Error Message:"); endProcessSuccLbl.Content = "Problem ending process!"; w.WriteLine(DateTime.Now.ToLongTimeString() + ": Error occured when attempting to end process: " + startProcessNameTXT.Text + " on remote computer."); }
            if (successfulStartProcess)
            {
                endProcessSuccLbl.Content = "Successfully ended process!";
            }
            else
                endProcessSuccLbl.Content = "Problem ending process!";
        }

        private void loadSettingsScreen_Click(object sender, RoutedEventArgs e)
        {
            successfulSaveLocationLbl.Content = "";

            connectionMI.IsEnabled = false;
            detailMI.IsEnabled = false;
            captureMI.IsEnabled = false;
            commandsMI.IsEnabled = false;
            settingsMI.IsEnabled = false;

            screenShotImg.Visibility = Visibility.Hidden;
            pcspecsGrid.Visibility = Visibility.Hidden;
            ListBox.Visibility = Visibility.Hidden;
            serviceBox.Visibility = Visibility.Hidden;
            connectionGrid.Visibility = Visibility.Hidden;
            conLBL.Visibility = Visibility.Hidden;
            settingsGrid.Visibility = Visibility.Visible;
            processDetailsGrid.Visibility = Visibility.Hidden;
            startStreamBTN.Visibility = Visibility.Hidden;
            endStreamBTN.Visibility = Visibility.Hidden;
            moveMouseBTN.Visibility = Visibility.Hidden;
            endMouseBTN.Visibility = Visibility.Hidden;
            driveGrid.Visibility = Visibility.Hidden;
            directoryListBox.Visibility = Visibility.Hidden;
            directoryGrid.Visibility = Visibility.Hidden;
            sendFileGrid.Visibility = Visibility.Hidden;
            startProcessGrid.Visibility = Visibility.Hidden;
            endProcessGrid.Visibility = Visibility.Hidden;
        }
        private void selectDirectoryBTN_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog().ToString().Equals("OK"))
            {
                saveLocationTXT.Text = fbd.SelectedPath;
            }
        }
        private void confirmBTN_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(saveLocationTXT.Text))
            {
                connectionMI.IsEnabled = true;
                detailMI.IsEnabled = true;
                captureMI.IsEnabled = true;
                commandsMI.IsEnabled = true;
                settingsMI.IsEnabled = true;

                pcSpecsMI.IsEnabled = true;
                driversMI.IsEnabled = true;
                directoriesMI.IsEnabled = true;
                processesMI.IsEnabled = true;
                ServicesMI.IsEnabled = true;
                streamMI.IsEnabled = true;
                sendFileMI.IsEnabled = true;
                startProcessMI.IsEnabled = true;
                endProcessMI.IsEnabled = true;

                settingsGrid.Visibility = Visibility.Hidden;
                w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully changed settings.");
            }
            else
            {
                successfulSaveLocationLbl.Content = "Directory does not exist!";
            }
        }

        private string generateFileName()
        {
            Random rndm = new Random();
            int fileNameNumber = rndm.Next(1, 1000);
            string directory = saveLocationTXT.Text;
            string fileName = fileNameNumber.ToString();
            fileName = directory + fileName + ".jpg";
            fileNames[fileNamesCounter] = fileName;
            fileNamesCounter++;
            return fileName;
        }

        private void startChat_Click(object sender, RoutedEventArgs e)
        {
            w.WriteLine(DateTime.Now.ToLongTimeString() + ": Starting to chat with remote client.");
            Chat startchat = new Chat(m_RemoteObject);
            startchat.Show();
        }

        private void moveMouseBTN_Click(object sender, RoutedEventArgs e)
        {
            w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully started to control remote computer's mouse.");
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler((state, args) =>
            {
                do
                {
                    if (_worker.CancellationPending)
                        break;

                    moveMouse();
                } while (true);
            });

            _worker.RunWorkerAsync();
            moveMouseBTN.IsEnabled = false;
            endMouseBTN.IsEnabled = true;
        }

        private void endMouseBTN_Click(object sender, RoutedEventArgs e)
        {
            moveMouseBTN.IsEnabled = true;
            endMouseBTN.IsEnabled = false;
            _worker.CancelAsync();
            w.WriteLine(DateTime.Now.ToLongTimeString() + ": Successfully stopped controlling remote computer's mouse.");
        }
        private void moveMouse()
        {
            double monitorX = System.Windows.SystemParameters.PrimaryScreenWidth;
            double monitorY = System.Windows.SystemParameters.PrimaryScreenHeight;
            User32.Win32Point w32mouse = new User32.Win32Point();
            User32.GetCursorPos(ref w32mouse);
            m_RemoteObject.MoveMouse(monitorX, monitorY, w32mouse.X, w32mouse.Y);
        }
    }
}
