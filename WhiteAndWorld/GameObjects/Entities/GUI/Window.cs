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

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class Window : Entity
    {
        private Bitmap background;
        private TextLabel label;

        public string Title 
        {
            get { return base.IsInitialized ? label.Text : ""; }
            set { if (base.IsInitialized) label.Text = value; } 
        }

        public override void Initialize()
        {
            base.Initialize();

            Manager.Graphics.LoadTexture(@"GameUI\mainWindow");
            background = new Bitmap(@"GameUI\mainWindow");
            background.Fixed = true;

            Manager.Graphics.LoadFont(@"Fonts\androidNation14");
            this.label = new TextLabel(@"Fonts\androidNation14");
            this.label.Location = new Vector2(16, -36);
            this.label.Color = Color.Black;
            
            base.ZOrder = 9;
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw(gameTime);
            label.Draw(gameTime);
        }
    }
}