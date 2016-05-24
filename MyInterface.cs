using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace MyRemoteObject
{
    interface MyInterface
    {
        bool setupCon();
        void setUpMonitorConfig(double remoteX, double remoteY);
        string[] displayProcess();
        string[] displayServices(); 
        string[] displaySpecs();
        string[][] displayDrives(); 
        string[] displayDirectories(String directoryPath); 
        byte[] TakeScreenshot();
        string[] ProcessDetails(String processName);
        bool SendFile(byte[] file, string fileName);
        byte[] RetrieveFile(string fileName);
        bool StartProcess(string processName); 
        bool EndProcess(string processId);
        void messaging(string message);
        string[] incomingMessage();
        void MoveMouse(double clientX, double clientY, Int32 newX, Int32 newY);
        void DoMouseClick();
    }
}
