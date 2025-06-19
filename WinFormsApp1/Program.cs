using System;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Security.Policy;
using Microsoft.VisualBasic.ApplicationServices;
using Opc;
using OpcRcw.Da;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //var _url = new URL("opcda://192.168.20.102/pSafetyLink.OPCServer/{333517ba-2832-43ee-a2ea-8a3fa7d27749}\r\n");
            //var _server = new Opc.Da.Server(new OpcCom.Factory(), _url);

            //var credential = new NetworkCredential("admin", password, domain);
            //var connectData = new ConnectData(credential, null);
            //_server.Connect(connectData);



            Application.Run(new Form1());
        }
    }
}