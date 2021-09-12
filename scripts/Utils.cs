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

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);

        public Utils(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.run = new RunMode(this);
            this.mode = "";
        }

        public async void printWarning(Label label, string warning, int millis)
        {
            label.Content = warning;
            label.Visibility = Visibility.Visible;
            await Task.Delay(millis);
            label.Visibility = Visibility.Hidden;
        }

        public void SwitchStatus(Button b)
        {
            if (b.Tag.Equals("off"))
            {
                foreach (Control control in this.mainWindow.ModeGrid.Children)
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

        public void PressKey(uint keyCode)
        {
            keybd_event(keyCode, 0, 0, 0);
        }

        public int GetSavePoint(string content)
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

                case "Walk":
                    i = 3;
                    break;

                case "START":
                    i = 4;
                    break;

                case "STOP":
                    i = 4;
                    break;

                case "Discord":
                    i = 5;
                    break;

                default:
                    Console.WriteLine("Error!");
                    Environment.Exit(0);
                    break;
            }
            return i;
        }

        public void OpenGitHubRepository()
        {
            Process.Start("https://github.com/MickyMaus209/AutoKeyPresser");
        }

        public void OpenDiscord()
        {
            Process.Start("https://discord.gg/GQvWebK8D3");
        }

        public void Start()
        {
            if (this.mainWindow.isRunning)
            {
                this.printWarning(mainWindow.WarningLabel, "Warning!" + "\n" + "Already" + "\n" + "running!" + "\n" + "Stop first.", 3500);
                return;
            }
            if (this.mode.Equals(""))
            {
                this.printWarning(mainWindow.WarningLabel, "Warning!" + "\n" + "No Mode" + "\n" + "selected!", 3500);
                return;
            }

            this.mainWindow.StartButton.IsEnabled = false;
            this.mainWindow.StopButton.IsEnabled = true;
            this.mainWindow.isRunning = true;
            this.run.run();
        }

        public void Stop()
        {
            this.mainWindow.isRunning = false;
            this.mainWindow.cts.Cancel();
            this.mainWindow.cts = null;
            this.mainWindow.cts = new CancellationTokenSource();
            this.mainWindow.StartButton.IsEnabled = true;
            this.mainWindow.StopButton.IsEnabled = false;

            this.run.discord.UpdatePresence("Idle / Main Menu");
        }

        public void Toggle()
        {
            if (this.mainWindow.isRunning)
            {
                this.Stop();
            }
            else
            {
                this.Start();
            }
        }

        public void SetHotKey()
        {
            this.mainWindow.toggleKey = (Key)Enum.Parse(typeof(Key), File.ReadAllLines(this.run.data.dataFile)[4], false);
        }
    }
}