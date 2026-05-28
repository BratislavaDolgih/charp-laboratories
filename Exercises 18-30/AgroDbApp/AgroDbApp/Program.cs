using System;
using System.Windows.Forms;
using AgroDbApp.Forms;

namespace AgroDbApp;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}