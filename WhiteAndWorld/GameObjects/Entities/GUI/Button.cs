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
    public class Button : ButtonBase
    {
        private Sprite button;
        private TextLabel code, caption, shadow;
        private Timer timer;

        public override string Caption 
        {
            get { return base.Caption; }
            set
            {
                base.Caption = value;

                if (caption != null)
                {
                    caption.Text = value;
                    shadow.Text = value; 
                }
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
                if (button != null)
                {
                    button.Location = value;
                    code.Location = new Vector2(value.X - 80, value.Y - 16);
                    caption.Location = new Vector2(value.X, value.Y + 8);
                    shadow.Location = new Vector2(value.X + 2, value.Y + 10);
                }
            }
        }

        public Button()
        {
            button = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\MainMenu\main_menu_cell"));
            button.Center = true;
            button.Fixed = true;

            Manager.Graphics.LoadFont(@"Fonts\segoe8");
            code = new TextLabel(@"Fonts\segoe8");
            code.Text = Guid.NewGuid().ToString().Substring(0, 16);
            code.Center = true;
            code.Fixed = true;

            Manager.Graphics.LoadFont(@"Fonts\androidNation14");
            caption = new TextLabel(@"Fonts\androidNation14");
            caption.Center = true;
            caption.Fixed = true;

            shadow = new TextLabel(@"Fonts\androidNation14");
            shadow.Center = true;
            shadow.Color = Color.Black;
            shadow.Fixed = true;

            timer = new Timer();

            Update(Manager.GameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Selected)
            {
                button.Scale = 0.9f;
                code.Scale = 1.2f;
                caption.Scale = 1.2f;
                shadow.Scale = 1.2f;

                button.Color = Color.White;
            }
            else
            {
                button.Scale = 0.8f;
                code.Scale = 0.9f;
                caption.Scale = 1;
                shadow.Scale = 1;

                button.Color = new Color(1, 1, 1, 0.5f);
            }

            if (timer.Value > (Selected ? 60 : 240))
            {
                timer.Reset();
                code.Text = Guid.NewGuid().ToString().Substring(0, 16);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            button.Draw(gameTime);
            code.Draw(gameTime);            
            shadow.Draw(gameTime);
            caption.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}