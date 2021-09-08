using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Interop;
using AutoKeyPresser.scripts.DiscordRpc;

namespace AutoKeyPresser.scripts
{
    internal class Utils
    {
        public MainWindow mainWindow { get; }
        public string mode { get; set; }
        public RunMode run { get; }
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        public Utils(MainWindow mainWindow)
        {
            this.run = new RunMode(this);
            this.mainWindow = mainWindow;
            this.mode = "";
        }

        public async void printWarning(Label label, string warning, int millis)
        {
            label.Content = warning;
            label.Visibility = Visibility.Visible;
            await Task.Delay(millis);
            label.Visibility = Visibility.Hidden;
            label.Content = "Warning!";
        }

        public void SwitchStatus(Button b)
        {
            if (b.Tag.Equals("off"))
            {
                foreach (Control control in mainWindow.ModeGrid.Children)
                {
                    if (control.GetType() != typeof(Button) || control.Name.Equals(b.Name) || control == null || control.Tag == null)
                    {
                        continue;
                    }
                    if (control.Tag.Equals("off"))
                    {
                        continue;
                    }
                    control.Tag = "off";
                    control.Background = new SolidColorBrush(Color.FromRgb(24, 24, 24));
                }
                b.Tag = "on";
                b.Background = Brushes.Gray;
                this.mode = (string)b.Content;
            }
            else
            {
                b.Tag = "off";
                b.Background = new SolidColorBrush(Color.FromRgb(24, 24, 24));
                this.mode = "";
            }
        }

        public void PrimaryButtonSwitchStatus()
        {
            if (mainWindow.StartButton.IsEnabled)
            {
                mainWindow.StartButton.IsEnabled = false;
                mainWindow.StopButton.IsEnabled = true;
            }
            else
            {
                mainWindow.StartButton.IsEnabled = true;
                mainWindow.StopButton.IsEnabled = false;
            }
        }

        public void PressKey(uint keyCode)
        {
            MainWindow.keybd_event(keyCode, 0, 0, 0);
        }

        public int GetSavePoint(String content)
        {
            int i = 0;

            switch (content)
            {
                case "AntiAFK":
                    i = 0;
                    break;

                case "AutoClicker":
                    i = 1;
                    break;

                case "WebRefresher":
                    i = 2;
                    break;

                case "Bunny":
                    i = 3;
                    break;

                case "Sprint":
                    i = 4;
                    break;
                case "Walk":
                    i = 5;
                    break;
                case "START":
                    i = 6;
                    break;
                case "STOP":
                    i = 6;
                    break;
                case "Discord":
                    i = 7;
                    break;

                default:
                    Console.WriteLine("Error!!!!!");
                    Environment.Exit(0);
                    break;
            }
            return i;
        }

        public void OpenGitHubRepository()
        {
            Process.Start("https://github.com");
        }

        public void OpenDiscord()
        {
            Process.Start("https://discord.gg/GQvWebK8D3");
        }

        public void Stop()
        {
            if (mainWindow.isRunning)
            {
                PrimaryButtonSwitchStatus();
                mainWindow.cts.Cancel();
                mainWindow.cts = null;
                mainWindow.cts = new CancellationTokenSource();
                mainWindow.isRunning = false;
                run.discord.UpdatePresence("Idle / Main Menu");
            }
        }

        public void Start()
        {
            if (mainWindow.isRunning)
            {
                printWarning(mainWindow.WarningLabel, "Warning!" + "\n" + "Already" + "\n" + "running!" + "\n" + "Stop first.", 3500);
                return;
            }
            if (mode.Equals(""))
            {
                printWarning(mainWindow.WarningLabel, "Warning!" + "\n" + "No Mode" + "\n" + "selected!", 3500);
                return;
            }

            mainWindow.isRunning = true;
            run.run();
            PrimaryButtonSwitchStatus();
        }
    }
}