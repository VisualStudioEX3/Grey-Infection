using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TLSA.Engine;
using TLSA.Engine.Graphics;
using TLSA.Engine.Input;
using TLSA.Engine.Physics.V1Engine;
using TLSA.Engine.Scene;
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;

namespace WhiteAndWorld.GameObjects.Entities
{    
    public class TileGroupQuery : Entity 
    {
        private Timer timer;
        private int timerCount;
        private const int timerDelay = 10;

        public Dictionary<string, bool> Query { get; set; }

        public override void Initialize()
        {
            this.Query = new Dictionary<string, bool>();
            this.timer = new Timer();
            this.timerCount = -1;

            base.Initialize();
        }

        public override void ReciveMessage(string message, params object[] values)
        {
            if (message == "show")
            {
                foreach (KeyValuePair<string, bool> target in this.Query)
                    Manager.Messages.SendMessage(target.Key, "hit", target.Value);
                
                this.timer.Reset();
                this.timerCount = timerDelay;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if ((bool)Manager.Vars["showMessagePause"] == true) return;

            if (this.timer.Value >= 1000 && this.timerCount > 0)
            {
                this.timer.Reset();
                this.timerCount--;
            }

            if (this.timerCount == 0)
            {
                foreach (KeyValuePair<string, bool> target in this.Query)
                    Manager.Messages.SendMessage(target.Key, "reset");

                this.timerCount = -1;
            }
        }
    }
}