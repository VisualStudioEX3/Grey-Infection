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
using TLSA.Engine.Scene;
using TLSA.Engine.Tools;

namespace WhiteAndWorld.GameObjects.States
{
    public class SplashScreen : StateComponent 
    {
        private Timer timer, timer2;
        private Sprite logo, logo2;
        private TextLabel label, shadow;

        public override void Initialize()
        {
            base.Initialize();
            
            timer = new Timer();
            timer2 = new Timer();

            Manager.Graphics.ClearColor = Color.White;

            logo = new Sprite(Manager.Graphics.LoadTexture(@"SplashScreen\UndeadCode"));
            logo.Scale = 0.90f;
            logo.Location = Helper.PointToVector2(Manager.Graphics.ScreenBounds.Center);
            logo.Center = true;

            logo2 = new Sprite(Manager.Graphics.LoadTexture(@"SplashScreen\UndeadCode"));
            logo2.Scale = 3f;
            logo2.Color = new Color(1, 1, 1, 0.5f);
            logo2.Location = Helper.PointToVector2(Manager.Graphics.ScreenBounds.Center);
            logo2.Center = true;

            Manager.Graphics.LoadFont(@"Fonts\arial14");
            label = new TextLabel(@"Fonts\arial14");
            label.Text = "Undead Code 2009 - 2011";
            label.Color = Color.Black;
            label.Center = true;
            label.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X, 600);

            shadow = new TextLabel(@"Fonts\arial14");
            shadow.Text = "Undead Code 2009 - 2011";
            shadow.Color = Color.Gray;
            shadow.Center = true;
            shadow.Location = new Vector2(Manager.Graphics.ScreenBounds.Center.X + 2, 602);
        }

        public override void Update(GameTime gameTime)
        {
            if (timer.Value >= 25)
            {
                timer.Reset();
                if (logo.Scale < 1) logo.Scale += 0.0005f;
                if (logo2.Scale > 1) logo2.Scale -= 0.025f;
            }
                
            label.Visible = logo2.Scale < 1;
            shadow.Visible = label.Visible;

            if (timer2.Value > 5000)
            {
                Manager.Graphics.ClearColor = Color.Black;
                Manager.GameStates.ChangeState("gametitle");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            logo2.Draw(gameTime);
            logo.Draw(gameTime);            
        }
    }
}