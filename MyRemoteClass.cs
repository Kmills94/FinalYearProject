using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging; 
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Runtime.InteropServices;



namespace MyRemoteObject
{
    public class MyRemoteClass : MarshalByRefObject, MyInterface
    {
        public static class User32
        {
            public const Int32 CURSOR_SHOWING = 0x00000001;
            [StructLayout(LayoutKind.Sequential)]
            public struct ICONINFO
            {
                public bool fIcon;
                public Int32 xHotspot;
                public Int32 yHotspot;
                public IntPtr hbmMask;
                public IntPtr hbmColor;
            }

            [StructLayout(LayoutKind.Sequential)] 
            public struct POINT
            {
                public Int32 x;
                public Int32 y;
            } 
            [StructLayout(LayoutKind.Sequential)]
            public struct CURSORINFO
            {
                public Int32 cbSize;
                public Int32 flags;
                public IntPtr hCursor;
                public POINT ptScreenPos;
            }
            [DllImport("user32.dll")]
            public static extern bool GetCursorInfo(out CURSORINFO pci);
            [DllImport("user32.dll")]
            public static extern IntPtr CopyIcon(IntPtr hIcon);
            [DllImport("user32.dll")]
            public static extern bool DrawIcon(IntPtr hdc, int x, int y, IntPtr hIcon);
            [DllImport("user32.dll")]
            public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);
            [DllImport("user32.dll")]
            public static extern bool SetCursorPos(int X, int Y);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
            public const int MOUSEEVENTF_LEFTDOWN = 0x02;
            public const int MOUSEEVENTF_LEFTUP = 0x04;
            public const int MOUSEEVENTF_MOVE = 0x0001;
        }
        public static string[] conversation = new string[1000];
        public static int convCounter = 0;
        public static double remoteMonitorX;
        public static double remoteMonitorY;
        public void setUpMonitorConfig(double remoteX, double remoteY)
        {
            remoteMonitorX = remoteX;
            remoteMonitorY = remoteY;
        }

        public bool setupCon()
        {
            return true;
        }
        public string[] displayProcess()
        {
            Process[] processlist = Process.GetProcesses();
            int processCounter = 0;
            int totalProcesses = 0;
            foreach (Process theprocess in processlist)
            {
                totalProcesses++;
            }
            string[] processName = new string[totalProcesses];
            foreach (Process theprocess in processlist)
            {
                processName[processCounter] = theprocess.ProcessName;
                processCounter++;
            }
            return processName;
        }
        public string[] displayServices() 
        {
            int serviceCounter = 0;
            int totalServices = 0;
            ServiceController[] ser = ServiceController.GetServices();
            foreach (ServiceController sc in ser)
            {
                totalServices++;
            }
            string[] services = new string[totalServices];
            foreach (ServiceController sc in ser)
            {
                services[serviceCounter] = ""+sc.ServiceName+" | "+sc.DisplayName+" | " +sc.Status+"";
                serviceCounter++;
            }
            return services;
        }
        public string[] displaySpecs()
        {
            string osQuery = "SELECT * FROM Win32_OperatingSystem";
            string cpuQuery = "SELECT * FROM Win32_Processor";
            string[] specs = new string[10];
            ManagementObjectSearcher osSearcher = new ManagementObjectSearcher(osQuery);
            foreach (ManagementObject os in osSearcher.Get())
            {
                specs[0] = os["Caption"].ToString();
                specs[1] = os["OSArchitecture"].ToString();
            }
            ManagementObjectSearcher cpuSearcher = new ManagementObjectSearcher("root\\CIMV2", cpuQuery);
            foreach (ManagementObject bs in cpuSearcher.Get())
            {
                specs[2] = bs["Name"].ToString();
            }

            ulong i = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            double SizeinKB = Convert.ToInt64(i);
            double SizeinMB = SizeinKB / 1024;
            double SizeinGB = SizeinMB / 1024;
            specs[3] = Convert.ToInt32(SizeinGB / 1000) + "GB";
            specs[4] = Environment.UserName;
            return specs;
        }
        public string[][] displayDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            long conversionKb;
            long conversionMb;
            long totalFreeGb;
            long availableFreeGb;
            long totalGb;
            int driveCounter = 0;
            int totalDrives = 0;
            foreach (DriveInfo d in allDrives) 
            {
                if (d.IsReady == true)
                {
                    totalDrives++;
                }
            }
            string[][] driveJagArr = new string[totalDrives][];
            foreach (DriveInfo d in allDrives)
            {
                    conversionKb = d.AvailableFreeSpace / 1024;
                    conversionMb = conversionKb / 1024;
                    availableFreeGb = conversionMb / 1024;
                   
                    conversionKb = d.TotalFreeSpace / 1024;
                    conversionMb = conversionKb / 1024;
                    totalFreeGb = conversionMb / 1024;
                    
                    conversionKb = d.TotalSize / 1024;
                    conversionMb = conversionKb / 1024;
                    totalGb = conversionMb / 1024;
                    if (d.IsReady == true)
                    {
                        driveJagArr[driveCounter] = new string[] { d.Name, d.DriveType.ToString(), d.VolumeLabel, d.DriveFormat, availableFreeGb.ToString(), totalFreeGb.ToString(), totalGb.ToString() };
                    }
                    driveCounter++;
             }
            return driveJagArr;
        }
        public string[] displayDirectories(String directoryPath)
        {
            string[] directoryArr = new string[2000];
            string[] folderArr = new string[1000];
            string[] fileArr = new string[1000];
            int folderCounter = 0;
            int fileCounter = 0;
            DirectoryInfo di = new DirectoryInfo(directoryPath);
            DirectoryInfo[] diArr = di.GetDirectories();
            FileInfo[] diFiles = di.GetFiles();
            foreach (DirectoryInfo dir in diArr)
            {
                folderArr[folderCounter] = dir.Name;
                folderCounter++;
            }
            foreach (FileInfo fi in diFiles)
            {
                fileArr[fileCounter] = fi.Name;
                fileCounter++;
            }
            for (int i = 0; i < folderCounter; i++ )
            {
                directoryArr[i] = folderArr[i];
            }
            for (int i = folderCounter-1; i < folderCounter+fileCounter; i++)
            {
                directoryArr[i] = fileArr[i];
            }
                return directoryArr;
        }
        public byte[] TakeScreenshot()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                User32.CURSORINFO cursorInfo;
                cursorInfo.cbSize = Marshal.SizeOf(typeof(User32.CURSORINFO));

                if (User32.GetCursorInfo(out cursorInfo))
                {
                    // if the cursor is showing draw it on the screen shot
                    if (cursorInfo.flags == User32.CURSOR_SHOWING)
                    {
                        // we need to get hotspot so we can draw the cursor in the correct position
                        var iconPointer = User32.CopyIcon(cursorInfo.hCursor);
                        User32.ICONINFO iconInfo;
                        int iconX, iconY;

                        if (User32.GetIconInfo(iconPointer, out iconInfo))
                        {
                            // calculate the correct position of the cursor
                            iconX = cursorInfo.ptScreenPos.x - ((int)iconInfo.xHotspot);
                            iconY = cursorInfo.ptScreenPos.y - ((int)iconInfo.yHotspot);

                            // draw the cursor icon on top of the captured screen image
                            User32.DrawIcon(g.GetHdc(), iconX, iconY, cursorInfo.hCursor);

                            // release the handle created by call to g.GetHdc()
                            g.ReleaseHdc();
                        }
                    }
                }
            }
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        public string[] ProcessDetails(String processName)
        {
            string[] processDetails = new string[8];
            Process[] my_process = Process.GetProcessesByName(processName);
            processDetails[0] = my_process[0].ProcessName.ToString();
            processDetails[1] = my_process[0].MainWindowTitle.ToString();
            processDetails[2] = my_process[0].Id.ToString();
            processDetails[3] = my_process[0].StartTime.ToString();
            processDetails[4] = my_process[0].UserProcessorTime.ToString();
            processDetails[5] = my_process[0].PriorityClass.ToString();
            processDetails[6] = my_process[0].Responding.ToString();
            processDetails[7] = my_process[0].VirtualMemorySize64.ToString();
            return processDetails;
        }
        public bool SendFile(byte[] file, string fileName)
        {
                MemoryStream stream = new MemoryStream(file);
                FileStream convertedFile = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                stream.WriteTo(convertedFile);
                convertedFile.Close();
                convertedFile.Close();
                return true;
        }
        public byte[] RetrieveFile(string fileName)
        {
            byte[] file;
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            file = br.ReadBytes(Convert.ToInt32(fs.Length));
            fs.Close();
            br.Close();
            return file;
        }
        public bool StartProcess(string processName)
        {
                Process process = new Process();
                ProcessStartInfo start = new ProcessStartInfo();
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.FileName = "cmd";
                start.Arguments = "/C start " + processName;
                process.StartInfo = start;
                process.Start();
                return true;
        }
        public bool EndProcess(string processId)
        {
                Process process = new Process();
                ProcessStartInfo start = new ProcessStartInfo();
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.FileName = "cmd";
                start.Arguments = "/C taskkill /PID "+ processId +" /F"; 
                process.StartInfo = start;
                process.Start();
                return true;
        }
        public void MoveMouse(double clientX, double clientY, Int32 newX, Int32 newY)
        {
            float multiplerX = (float)(remoteMonitorX) / (float)(clientX);
            float multiplerY = (float)(remoteMonitorY) / (float)(clientY);
            float setX = (float)(newX) * multiplerX;
            float setY = (float)(newY) * multiplerY;
            User32.SetCursorPos(Convert.ToInt32(setX), Convert.ToInt32(setY));
        }
        public void messaging(string message)
        {
                conversation[convCounter] = message;
                convCounter++;
        }
        public string[] incomingMessage() // SEND TIMESTAMP AND USERNAME?
        {
            string[] messages = new string[convCounter];
            for (int i = 0; i < convCounter; i++)
            {
                messages[i] = conversation[i];
                conversation[i] = "";
            }
            convCounter = 0;
            return messages;
        }
        public void DoMouseClick()
        {
            int X = Cursor.Position.X;
            int Y = Cursor.Position.Y;
            User32.mouse_event(User32.MOUSEEVENTF_LEFTDOWN | User32.MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }
    }
}
