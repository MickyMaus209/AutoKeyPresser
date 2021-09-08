using AutoKeyPresser.scripts.DiscordRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoKeyPresser.scripts
{
    internal class RunMode
    {
        public Utils utils { get; }
        public Data data { get; }
        public Discord discord { get; }

        public RunMode(Utils utils)
        {
            this.utils = utils;
            this.data = new Data(utils);
            this.discord = new Discord(this);
        }

        public void run()
        {
            string[] lines = data.ReadData(data.dataFile);
            discord.UpdatePresence("Using " + utils.mode);

            switch (utils.mode)
            {
                case "AntiAFK":

                    int i = 0;

                    Task.Run(async () =>
                    {
                        while (utils.mainWindow.isRunning)
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
                            await Task.Delay(TimeSpan.FromSeconds(Double.Parse(lines[0])), utils.mainWindow.cts.Token);
                        }
                    }, utils.mainWindow.cts.Token);
                    break;

                case "AutoClicker":
                    KeyTimer(new uint[] { 0x0002 }, lines[1]);
                    break;

                case "WebRefresher":
                    KeyTimer(new uint[] { 0x0074 }, lines[2]);
                    break;

                case "Bunny":
                    KeyTimer(new uint[] { 0x0057, 0x0020, 0x00A2 }, lines[3]);
                    break;

                case "Sprint":
                    KeyTimer(new uint[] { 0x00A2, 0x0057 }, lines[4]);
                    break;

                case "Walk":
                    KeyTimer(new uint[] { 0x0057 }, lines[5]);
                    break;

                default:
                    discord.UpdatePresence("Idle / Main Menu");
                    Console.WriteLine("Error!");
                    utils.mainWindow.isRunning = false;
                    break;
            }
        }

        private void KeyTimer(uint[] keys, string time)
        {
            Task.Run(async () =>
            {
                while (utils.mainWindow.isRunning)
                {
                    for (int i = 0; i < keys.Length; i++)
                    {
                        utils.PressKey(keys[i]);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(Double.Parse(time)), utils.mainWindow.cts.Token);
                }
            }, utils.mainWindow.cts.Token);
        }
    }
}