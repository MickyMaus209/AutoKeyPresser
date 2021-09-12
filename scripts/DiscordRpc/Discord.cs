using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRpcDemo;

namespace AutoKeyPresser.scripts.DiscordRpc
{
    internal class Discord
    {
        private DiscordRpcDemo.DiscordRpc.EventHandlers handlers;
        private DiscordRpcDemo.DiscordRpc.RichPresence presence;
        private RunMode run;

        public Discord(RunMode run)
        {
            this.run = run;
            if (!this.IsDiscordActivityOn())
            {
                return;
            }

            this.handlers = default(DiscordRpcDemo.DiscordRpc.EventHandlers);
            DiscordRpcDemo.DiscordRpc.Initialize("885196864117407754", ref this.handlers, true, null);
            this.presence.details = "Running AutoKeyPresser";
            this.presence.state = "Idle / Main Menu";
            this.presence.largeImageKey = "image";
            DiscordRpcDemo.DiscordRpc.UpdatePresence(ref this.presence);
        }

        public void UpdatePresence(string state)
        {
            if (!this.IsDiscordActivityOn())
            {
                return;
            }
            this.presence.state = state;
            DiscordRpcDemo.DiscordRpc.UpdatePresence(ref this.presence);
        }

        public bool IsDiscordActivityOn()
        {
            return bool.Parse(File.ReadAllLines(run.data.dataFile)[5]);
        }
    }
}