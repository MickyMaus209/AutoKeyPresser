using AutoKeyPresser.scripts;
using AutoKeyPresser.scripts.DiscordRpc;
using DesktopWPFAppLowLevelKeyboardHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AutoKeyPresser
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CancellationTokenSource cts = new CancellationTokenSource();
        public bool isRunning;
        private Utils utils;
        private const string version = "3.0";
        public string rMode { get; set; }
        private string key;
        private LowLevelKeyboardListener listener;
        public Key toggleKey { get; set; }
        private bool canChangeHotKey;

        public MainWindow()
        {
            InitializeComponent();
            this.utils = new Utils(this);

            //Element Tags
            AutoClickerButton.Tag = "off";
            AntiAfkButton.Tag = "off";
            WalkButton.Tag = "off";
            WebRefresherButton.Tag = "off";
            WalkButton.Tag = "off";

            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;

            CurrentVersionLabel.Content = CurrentVersionLabel.Content + version;
            this.utils.SetHotKey();

            this.isRunning = false;
            this.rMode = "";
            this.key = "";
            this.canChangeHotKey = true;

            Console.WriteLine("AutoKeyPresser v.3.0 started.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.listener = new LowLevelKeyboardListener();
            this.listener.OnKeyPressed += Listener_OnKeyPressed;

            this.listener.HookKeyboard();
        }

        private void Listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed.ToString().Equals(File.ReadAllLines(this.utils.run.data.dataFile)[4]) && ModeSettingsGrid.Visibility != Visibility.Visible && PrimaryButtonSettingsGrid.Visibility != Visibility.Visible)
            {
                this.utils.Toggle();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.listener.UnHookKeyboard();
            this.cts = null;
        }

        private void AutoClicker_LeftClick(object sender, RoutedEventArgs e)
        {
            this.utils.SwitchStatus(AutoClickerButton);
        }

        private void AntiAfk_LeftClick(object sender, RoutedEventArgs e)
        {
            this.utils.SwitchStatus(AntiAfkButton);
        }

        private void StartButton_LeftClick(object sender, RoutedEventArgs e)
        {
            this.utils.Start();
        }

        private void StopButton_LeftClick(object sender, RoutedEventArgs e)
        {
            this.utils.Stop();
        }

        private void WebRefresher_LeftClick(object sender, RoutedEventArgs e)
        {
            this.utils.SwitchStatus(WebRefresherButton);
        }

        private void Walk_LeftClick(object sender, RoutedEventArgs e)
        {
            this.utils.SwitchStatus(WalkButton);
        }

        private void PrimaryButton_RightClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
            {
                return;
            }
            Button button = (Button)sender;

            string[] lines = File.ReadAllLines(utils.run.data.dataFile);

            this.rMode = button.Content.ToString();
            HotKeyButton.Content = File.ReadAllLines(utils.run.data.dataFile)[utils.GetSavePoint(this.rMode)];

            PrimaryButtonSettingsGrid.Visibility = Visibility.Visible;
            DefaultGrid.IsEnabled = false;
            this.key = File.ReadAllLines(this.utils.run.data.dataFile)[4];
            this.utils.Stop();
        }

        private void ModeButton_RightClick(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() != typeof(Button))
            {
                return;
            }
            Button button = (Button)sender;

            this.rMode = button.Content.ToString();
            DelayText.Text = File.ReadAllLines(this.utils.run.data.dataFile)[this.utils.GetSavePoint(this.rMode)].Replace(",", ".");
            ModeSettingsGrid.Visibility = Visibility.Visible;
            DefaultGrid.IsEnabled = false;
            this.utils.Stop();
        }

        private void ModeSaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.utils.run.data.WriteDataFile(this.utils.GetSavePoint(rMode), DelayText.Text);
            CloseSettings();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.CloseSettings();
        }

        private void CloseSettings()
        {
            this.rMode = "";
            this.key = "";
            PrimaryButtonSettingsGrid.Visibility = Visibility.Hidden;
            ModeSettingsGrid.Visibility = Visibility.Hidden;
            DefaultGrid.IsEnabled = true;
            this.utils.run.discord.UpdatePresence("Idle / Main Menu");
        }

        private void DelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DelayText.Text.Contains(","))
            {
                DelayText.Text = DelayText.Text.Replace(",", ".");
            }
        }

        private void PrimarySaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.utils.run.data.WriteDataFile(this.utils.GetSavePoint(rMode), key);
            this.utils.SetHotKey();
            this.CloseSettings();
        }

        private void HotKeyButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (canChangeHotKey)
            {
                return;
            }
            canChangeHotKey = true;
            HotKeyButton.Content = e.Key;
            this.key = e.Key.ToString();
            foreach (Control c in PrimaryButtonSettingsGrid.Children)
            {
                if (c != HotKeyButton)
                {
                    c.IsEnabled = true;
                }
            }
        }

        private void HotKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!canChangeHotKey)
            {
                return;
            }
            canChangeHotKey = false;
            HotKeyButton.Content = "Click a key!";
            foreach (Control c in PrimaryButtonSettingsGrid.Children)
            {
                if (c != HotKeyButton)
                {
                    c.IsEnabled = false;
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.utils.Stop();
            DefaultGrid.Visibility = Visibility.Hidden;
            MainSettingsGrid.Visibility = Visibility.Visible;
            DiscordCheckBox.IsChecked = utils.run.discord.IsDiscordActivityOn();
            this.utils.run.discord.UpdatePresence("Settings");
        }

        private void DataResetButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(this.utils.run.data.dataFile);
            Directory.Delete(this.utils.run.data.dirName);
            Application.Current.Shutdown();
            System.Windows.Forms.Application.Restart();
        }

        private void DiscordCheckBox_Click(object sender, RoutedEventArgs e)
        {
            utils.run.data.WriteDataFile(this.utils.GetSavePoint("Discord"), DiscordCheckBox.IsChecked.ToString());
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            this.utils.OpenGitHubRepository();
        }

        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            if (CreditsGrid.Visibility == Visibility.Visible)
            {
                CreditsGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                CreditsGrid.Visibility = Visibility.Visible;
            }
        }

        private void MoreAppsButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/MickyMaus209");
        }

        private void GeneralSettingsCloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainSettingsGrid.Visibility = Visibility.Hidden;
            DefaultGrid.Visibility = Visibility.Visible;
            this.utils.run.discord.UpdatePresence("Idle / Main Menu");
        }

        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {
            this.utils.OpenDiscord();
        }

        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            this.utils.OpenDiscord();
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            this.utils.OpenGitHubRepository();
        }
    }
}