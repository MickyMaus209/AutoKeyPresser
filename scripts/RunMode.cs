using AutoKeyPresser.scripts.DiscordRpc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoKeyPresser.scripts
{
    internal class RunMode
    {
        public Utils utils { get; }
        public Data data { get; }
        public Discord discord { get; }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public RunMode(Utils utils)
        {
            this.utils = utils;
            this.data = new Data(utils);
            this.discord = new Discord(this);
        }

        public void run()
        {
            string[] lines = File.ReadAllLines(utils.run.data.dataFile);
            this.discord.UpdatePresence("Using " + this.utils.mode);

            switch (utils.mode)
            {
                case "AntiAFK":

                    int i = 0;

                    Task.Run(async () =>
                    {
                        while (this.utils.mainWindow.isRunning)
                        {
                            switch (i)
                            {
                                case 1:

                                    utils.PressKey(0x0057);
                                    break;

                                case 2:
                                    utils.PressKey(0x0041);
                                    break;

                                case 3:
                                    utils.PressKey(0x0053);
                                    break;

                                case 4:
                                    utils.PressKey(0x0044);
                                    i = 0;
                                    break;
                            }
                            i++;
                            await Task.Delay(TimeSpan.FromSeconds(Double.Parse(lines[0])), this.utils.mainWindow.cts.Token);
                        }
                    }, this.utils.mainWindow.cts.Token);
                    break;

                case "AutoClicker":

                    Task.Run(async () =>
                    {
                        while (utils.mainWindow.isRunning)
                        {
                            mouse_event(dwFlags: 0x0003, dx: 0, dy: 0, cButtons: 0, dwExtraInfo: 0);
                            Thread.Sleep(1);
                            mouse_event(dwFlags: 0x0001, dx: 0, dy: 0, cButtons: 0, dwExtraInfo: 0);
                        }

                        await Task.Delay(TimeSpan.FromSeconds(Double.Parse(lines[1])), this.utils.mainWindow.cts.Token);
                    }, this.utils.mainWindow.cts.Token);

                    break;

                case "WebRefresher":
                    KeyTimer(new uint[] { 0x0074 }, lines[2]);
                    break;

                case "Walk":
                    KeyTimer(new uint[] { 0x0057 }, lines[3]);
                    break;

                default:
                    this.discord.UpdatePresence("Idle / Main Menu");
                    Console.WriteLine("Error!");
                    this.utils.mainWindow.isRunning = false;
                    break;
            }
        }

        private void KeyTimer(uint[] keys, string time)
        {
            Task.Run(async () =>
            {
                while (this.utils.mainWindow.isRunning)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        utils.PressKey(keys[i]);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(Double.Parse(time)), utils.mainWindow.cts.Token);
                }
            }, this.utils.mainWindow.cts.Token);
        }
    }
}