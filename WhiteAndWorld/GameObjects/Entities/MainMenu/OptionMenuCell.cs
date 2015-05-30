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

namespace WhiteAndWorld.GameObjects.Entities.MainMenu
{
    public class OptionMenuCell : Entity 
    {
        private TextLabel code, label;
        private Sprite cell, blocked;
        private Timer timer;
        private int levelNumber = 0;
        private int alpha = 128;
        private int color = 200;

        public int LevelNumber
        {
            get { return levelNumber; } 
            set 
            { 
                levelNumber = value;
                label.Text = "Level " + levelNumber;
            }
        }

        public override Vector2 Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
                if (base.IsInitialized)
                {
                    cell.Location = value;
                    blocked.Location = value + new Vector2(36, 26);
                    code.Location = value + new Vector2(16, 8);
                    label.Location = value + new Vector2(36, 24);
                }
            }
        }

        public string Caption { get; set; }

        public bool Selected { get; set; }

        private bool disabled = false;
        public bool Disabled 
        {
            get { return disabled; }
            set
            {
                disabled = value;
                
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            cell = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\MainMenu\main_menu_cell"));

            blocked = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\LevelSelect\blocked"));
            blocked.Visible = false;

            Manager.Graphics.LoadFont(@"Fonts\androidNation20b");
            Manager.Graphics.LoadFont(@"Fonts\font8");

            code = new TextLabel(@"Fonts\font8");
            code.Text = Guid.NewGuid().ToString().Substring(1, 12);

            label = new TextLabel(@"Fonts\androidNation20b");

            timer = new Timer();

            this.ZOrder = -1;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 loc = base.Location;

            if (this.Selected && !this.Disabled)
            {
                color = 255; alpha = 255;
                cell.Scale = 1; 
                code.Scale = 1;
                label.Scale = 1;

                loc.X -= 8; loc.Y -= 2;
                cell.Location = loc;
            }
            else
            {
                color = this.Disabled ? 128 : 200; alpha = 128;
                cell.Scale = 0.925f;
                code.Scale = 0.925f;
                label.Scale = 0.925f;

                cell.Location = base.Location;
            }

            cell.Color = new Color(color, color, color, alpha);
            code.Color = new Color(0, 0, 0, alpha);
            label.Color = new Color(0, 0, 0, alpha);

            label.Text = this.Caption;

            blocked.Visible = this.disabled;

            if (timer.Value >= (this.Selected ? 100 : 500))
            {
                timer.Reset();
                code.Text = Guid.NewGuid().ToString().Substring(1,12);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            cell.Draw(gameTime);
            code.Draw(gameTime);
            label.Draw(gameTime);
            blocked.Draw(gameTime);
        }
    }
}
